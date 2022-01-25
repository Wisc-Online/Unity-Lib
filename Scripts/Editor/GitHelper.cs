using FVTC.LearningInnovations.Unity.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.Editor
{
    public static class GitHelper
    {
        public static bool IsGitInstalled
        {
            get
            {
                return EnvironmentHelpers.GetExecutablePath("git.exe") != null;
            }
        }

        public static bool IsProjectGitRepository
        {
            get
            {
                // enable InitializeGitRepository only if the .git dir does not exist already

                var assetsDir = new DirectoryInfo(Application.dataPath);

                if (assetsDir.Exists)
                {
                    var projectDir = assetsDir.Parent;

                    if (projectDir.Exists)
                    {
                        var dotGitDir = projectDir.GetDirectories(".git");

                        return dotGitDir != null && dotGitDir.Length == 1 && dotGitDir[0].Exists;
                    }
                }

                return false;
            }
        }

        public static void AddModule(string path, Uri uri)
        {
            float lastProgress = 0f;

            Action<string> stdErrCallback = msg =>
            {
                float? gitProgress = GitHelper.ParseProgress(msg);

                if (gitProgress.HasValue)
                    lastProgress = gitProgress.Value;

                EditorUtility.DisplayProgressBar("Adding Submodule", msg, lastProgress);
            };

            try
            {
                ProcessHelper.StartAndWaitForExit(GetGitPath(), string.Format("submodule add {0} {1}", uri, path), null, stdErrCallback);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

        }

        internal static Remote[] GetRemotes()
        {
            List<Remote> remotes = new List<Remote>();
            Remote lastRemote = null;
            Uri uri = null;
            Action<string> remoteParser = str =>
            {
                string[] parts = str.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 3)
                {
                    if (lastRemote == null || lastRemote.Name != parts[0])
                    {
                        lastRemote = new Remote
                        {
                            Name = parts[0]
                        };

                        remotes.Add(lastRemote);
                    }

                    switch (parts[2])
                    {
                        case "(fetch)":
                            if (Uri.TryCreate(parts[1], UriKind.RelativeOrAbsolute, out uri))
                            {
                                lastRemote.FetchUri = uri;
                            }
                            break;
                        case "(push)":
                            if (Uri.TryCreate(parts[1], UriKind.RelativeOrAbsolute, out uri))
                            {
                                lastRemote.PushUri = uri;
                            }
                            break;
                    }
                }
            };

            ProcessHelper.StartAndWaitForExit(GetGitPath(), "remote -v", remoteParser);

            return remotes.ToArray();
        }

        public static bool SetRemote(Remote remote)
        {
            return ExecuteCommand(string.Format("remote set-url {0} {1}", remote.Name, remote.FetchUri), "Setting Remote", string.Format("Adding Set '{0}' with URL '{1}'", remote.Name, remote.FetchUri));
        }

        public static bool AddRemote(Remote remote)
        {
            return ExecuteCommand(string.Format("remote add {0} {1}", remote.Name, remote.FetchUri), "Adding Remote", string.Format("Adding Remote '{0}' with URL '{1}'", remote.Name, remote.FetchUri));
        }

        private static float? ParseProgress(string msg)
        {
            const string CHECKING_OUT_PREFIX = "Checking out files: ";
            const string RECEIVING_OBJECTS_PREFIX = "Receiving objects: ";
            const string RESOLVING_DELTAS_PREFIX = "Resolving deltas: ";

            string[] prefixes = new string[] { CHECKING_OUT_PREFIX, RECEIVING_OBJECTS_PREFIX, RESOLVING_DELTAS_PREFIX };

            float clonePercent;

            foreach (var prefix in prefixes)
            {
                if (msg.StartsWith(prefix))
                {
                    if (float.TryParse(msg.Substring(prefix.Length).Trim().Split(' ')[0].TrimEnd('%'), out clonePercent))
                    {
                        return clonePercent / 100f;
                    }
                }
            }

            return null;
        }

        public static void Commit(string message, bool commitAll)
        {
            string tempFilePath = System.IO.Path.GetTempFileName();

            try
            {
                File.WriteAllText(tempFilePath, message);

                if (commitAll)
                {
                    AddAll();

                }

                string gitCommand = string.Format("commit -F {0} ", tempFilePath).Trim();

                if (!ExecuteCommand(gitCommand, "Git Commit", "Committing changes."))
                {
                    EditorUtility.DisplayDialog("Error", "There was an error while performing the Commit.  See error log.", "OK");
                }
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }

        public static bool AddAll()
        {
            try
            {
                int i = 0;

                EditorUtility.DisplayProgressBar("Staging files", "Starting to stage files...", 0f);

                Action<string> callback = msg =>
                {
                    i++;

                    if (i > 100)
                        i = 1;

                    EditorUtility.DisplayProgressBar("Staging files", msg, i / 100f);
                };

                return ProcessHelper.StartAndWaitForExit(GetGitPath(), "-c core.autocrlf=false add .", null, callback);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public static bool Stash()
        {
            try
            {
                int i = 0;

                EditorUtility.DisplayProgressBar("Stashing Staged Changes", "Starting stash...", 0f);

                Action<string> callback = msg =>
                {
                    i++;

                    if (i > 100)
                        i = 1;

                    EditorUtility.DisplayProgressBar("Stashing Staged Changes", msg, i / 100f);
                };

                return ProcessHelper.StartAndWaitForExit(GetGitPath(), "stash", null, callback);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public static bool StashDrop()
        {
            return ExecuteCommand("stash drop", "Dropping Last Stash", "Removing the last stash.");
        }


        public static void Push()
        {
            Push("origin", "master");
        }

        public static void Pull()
        {
            Pull("origin", "master");
        }

        private static void Pull(string remote, string branch)
        {
            if (!ExecuteCommand(string.Format("pull {0} {1}", remote, branch), "Git Pull", string.Format("Pulling from {0}/{1}", remote, branch)))
            {
                EditorUtility.DisplayDialog("Error", "There was an error while performing the Pull.  See error log.", "OK");
            }
        }

        static void Push(string remote, string branch)
        {
            if (!ExecuteCommand(string.Format("push {0} {1}", remote, branch), "Git Push", string.Format("Pushing Changes to {0}/{1}", remote, branch)))
            {
                EditorUtility.DisplayDialog("Error", "There was an error while performing the Push.  See error log.", "OK");
            }
        }

        public static Module[] GetModules()
        {
            var gitModulesFile = new FileInfo(Path.Combine(Directory.GetParent(Application.dataPath).FullName, ".gitmodules"));

            List<Module> modules = new List<Module>();

            if (gitModulesFile.Exists)
            {
                Module module = null;

                using (var reader = gitModulesFile.OpenText())
                {
                    string line;
                    string[] lineParts;

                    const string subModulePrefix = "[submodule ";

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith(subModulePrefix))
                        {
                            module = new Module
                            {
                                Name = line.Substring(subModulePrefix.Length + 1).TrimEnd(']', '"')
                            };

                            modules.Add(module);
                        }
                        else if (module != null)
                        {
                            lineParts = line.Split(new char[] { '=' }, 2).Select(p => p.Trim()).ToArray();

                            if (lineParts.Length == 2)
                            {
                                switch (lineParts[0])
                                {
                                    case "path":
                                        module.Path = lineParts[1];
                                        break;
                                    case "url":
                                        module.Url = new Uri(lineParts[1]);
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            return modules.ToArray();
        }

        public static void Initalize()
        {
            ProcessHelper.StartAndWaitForExit(GetGitPath(), "init");
        }

        public static bool PromptUserToDownloadGitIfNotInstalled()
        {
            if (!IsGitInstalled)
            {
                if (EditorUtility.DisplayDialog("Install Git?", "It looks like you may not have Git installed.\r\nDo you want to download it now?", "Download", "Close"))
                {
                    Process.Start("https://git-scm.com/downloads");
                }

                return false;
            }

            return true;
        }

        public static string GetGitPath()
        {
            return EnvironmentHelpers.GetExecutablePath("git.exe");
        }

        public class Module
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public Uri Url { get; set; }
        }

        public class Remote
        {
            public string Name { get; set; }
            public Uri FetchUri { get; set; }
            public Uri PushUri { get; set; }
        }

        public static bool UpdateSubmodule(string url)
        {
            var module = GetModules().Where(x => url.Equals(x.Url.AbsoluteUri, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if (module == null)
            {
                UnityEngine.Debug.LogErrorFormat("Module with Url {0} Not Found", url);
                return false;
            }
            else
            {
                string command = string.Format("submodule update --remote {0}", module.Path);

                return ExecuteCommand(command, "Updating Submodule", "Updating Module: " + module.Name);
            }
        }

        static bool ExecuteCommand(string command, string title, string message)
        {
            try
            {
                EditorUtility.DisplayProgressBar(title, message, 0f);

                return ProcessHelper.StartAndWaitForExit(GetGitPath(), command);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

    }
}
