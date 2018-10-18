using FVTC.LearningInnovations.Unity.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
    }
}
