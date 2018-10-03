using FVTC.LearningInnovations.Unity.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.Editor
{
    public class InstallSubmodules
    {

        [MenuItem("FVTC/Learning Innovations/Install Submodule/Mixed Reality")]
        private static void InstallMixedRealitySubmodule()
        {

        }


        [MenuItem("FVTC/Learning Innovations/Git/Initialize New Repository")]
        private static void InitializeGitRepository()
        {       
            if (GitHelper.PromptUserToDownloadGitIfNotInstalled() && !GitHelper.IsProjectGitRepository)
            {
                GitHelper.Initalize();
            }
        }

        [MenuItem("FVTC/Learning Innovations/Git/Initialize New Repository", true)]
        private static bool InitializeGitRepositoryValidation()
        {
            return !GitHelper.IsProjectGitRepository;
        }

    }
}