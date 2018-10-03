using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.Helpers
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

                        return dotGitDir == null || dotGitDir.Length == 0;
                    }
                }

                return false;
            }
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
    }
}
