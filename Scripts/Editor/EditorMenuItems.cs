using FVTC.LearningInnovations.Unity.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.Editor
{
    public class EditorMenuItems
    {

        static readonly Uri UNITY_LIB_MR_URL = new Uri("https://github.com/Wisc-Online/Unity-Lib-MR.git");
        const string UNITY_LIB_MR_PATH = "Assets/FVTC/LearningInnovations-MR";

        [MenuItem("Learning Innovations/Install Submodule/Mixed Reality")]
        static void InstallMixedRealitySubmodule()
        {
            if (GitHelper.PromptUserToDownloadGitIfNotInstalled())
            {
                GitHelper.AddModule(UNITY_LIB_MR_PATH, UNITY_LIB_MR_URL);

                AssetDatabase.Refresh();
            }
        }

        [MenuItem("Learning Innovations/Install Submodule/Mixed Reality", true)]
        static bool ValidateInstallMixedRealitySubmodule()
        {
            bool canRun = false;

            if (GitHelper.IsProjectGitRepository)
            {
                var modules = GitHelper.GetModules();

                canRun = !modules.Any(m => m.Url == UNITY_LIB_MR_URL);
            }

            return canRun;
        }


        [MenuItem("Learning Innovations/Git/Initialize New Repository")]
        static void InitializeGitRepository()
        {
            if (GitHelper.PromptUserToDownloadGitIfNotInstalled() && !GitHelper.IsProjectGitRepository)
            {
                GitHelper.Initalize();
            }
        }

        [MenuItem("Learning Innovations/Git/Initialize New Repository", true)]
        static bool ValidateInitializeGitRepository()
        {
            return !GitHelper.IsProjectGitRepository;
        }

        [MenuItem("Learning Innovations/Git/Commit All Changes")]
        static void GitCommitAll()
        {
            Action<string> acceptCallback = msg =>
            {
                if (string.IsNullOrEmpty(msg))
                {
                    GitCommitAll();
                }
                else
                {
                    GitHelper.Commit(msg, true);
                }
            };

            Dialog.PromptMultiLine("Git Commit", "Enter a commit message.", acceptCallback);
        }

        [MenuItem("Learning Innovations/Git/Commit All Changes", true)]
        static bool ValidateGitCommitAll()
        {
            return GitHelper.IsProjectGitRepository;
        }


        [MenuItem("Learning Innovations/Git/Pull")]
        static void GitPull()
        {
            GitHelper.Pull();
        }

        [MenuItem("Learning Innovations/Git/Push")]
        static void GitPush()
        {
            GitHelper.Push();
        }


        [MenuItem("Learning Innovations/Git/Pull", true)]
        static bool ValidateGitPull()
        {
            return GitHelper.IsProjectGitRepository;
        }

        [MenuItem("Learning Innovations/Git/Push", true)]
        static bool ValidateGitPush()
        {
            return GitHelper.IsProjectGitRepository;
        }


    }
}