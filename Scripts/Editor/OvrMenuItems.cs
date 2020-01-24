using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.Editor
{
    public class OvrMenuItems
    {

        const string OculusDesktopPackageId = "com.unity.xr.oculus.standalone";
        const string OculusIntegrationAssetStoreUrl = "https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022";


        static readonly Uri UNITY_LIB_OVR_HELPERS_URL = new Uri("https://github.com/Wisc-Online/Unity-OvrHelpers.git");
        const string UNITY_LIB_OVR_HELPERS_PATH = "Assets/FVTC/LearningInnovations-OvrHelpers";

        [MenuItem("Learning Innovations/Oculus/Install or Update Submodule")]
        static void InstallOvrHelpersSubmodule()
        {
            if (GitHelper.PromptUserToDownloadGitIfNotInstalled())
            {
                var modules = GitHelper.GetModules();

                if (modules.Any(m => m.Url == UNITY_LIB_OVR_HELPERS_URL))
                {
                    // Update
                    GitHelper.UpdateSubmodule(UNITY_LIB_OVR_HELPERS_URL.AbsoluteUri);

                    AssetDatabase.Refresh();
                }
                else if (GetIsOculusIntegrationAssetsInstalled())
                {
                    // Install
                    GitHelper.AddModule(UNITY_LIB_OVR_HELPERS_PATH, UNITY_LIB_OVR_HELPERS_URL);

                    AssetDatabase.Refresh();
                }
                else
                {
                    // prompt user to first install the Oculus

                    string message = @"The Oculus Integration Asset package must first be installed.

Do you want to visit the Oculus Asset Store to install the required assets?";

                    if (EditorUtility.DisplayDialog("Oculus Integration Asset Required", message, "Yes", "No"))
                    {
                        InstallOculusIntegrationAssets(false);
                    }

                }

            }
        }

        private static AddRequest _addOculusToolsAssetRequest;

        [MenuItem("Learning Innovations/Oculus/Install Oculus Desktop Package")]
        public static void InstallOculusDesktopPackage()
        {
            _addOculusToolsAssetRequest = Client.Add(OculusDesktopPackageId);

            EditorApplication.update += OculusDesktopPackageInstallProgress;
        }

        private static void OculusDesktopPackageInstallProgress()
        {
            if (_addOculusToolsAssetRequest != null)
            {
                if (_addOculusToolsAssetRequest.IsCompleted)
                {
                    EditorApplication.update -= OculusDesktopPackageInstallProgress;

                    switch (_addOculusToolsAssetRequest.Status)
                    {
                        case StatusCode.Success:
                            EditorUtility.DisplayDialog("Package Installed", "Oculus Desktop Package Installed Successfully", "Yay!");
                            Debug.Log($"Oculus Tools Asset installed successfully");
                            break;

                        case StatusCode.Failure:
                        default:
                            EditorUtility.DisplayDialog("Package Installation Failed", "Oculus Desktop package failed to install.", "Close");
                            Debug.LogError($"Error installing Oculus Tools\n{_addOculusToolsAssetRequest.Error.message}");
                            break;
                    }
                }
            }
        }

        [MenuItem("Learning Innovations/Oculus/Install Oculus Integration Assets")]
        public static void InstallOculusIntegrationAssets()
        {
            InstallOculusIntegrationAssets(true);
        }

        private static void InstallOculusIntegrationAssets(bool cancellable)
        {
            // first ensure the Oculus Utilities Asset is installed

            string message = @"Press OK to open the Unity Asset Store in a browser.

From there, select either 'Add to My Assets' or 'Open in Unity'";

            bool doIt = EditorUtility.DisplayDialog("Install Oculus Integration", message, "OK", cancellable ? "Cancel" : null);

            if (doIt)
            {
                Application.OpenURL(OculusIntegrationAssetStoreUrl);
            }
        }

        [MenuItem("Learning Innovations/Oculus/Install Oculus Integration Assets", true)]
        public static bool InstallOculusIntegrationAssetsValidate()
        {
            return !GetIsOculusIntegrationAssetsInstalled();
        }

        private static bool GetIsOculusIntegrationAssetsInstalled()
        {
            bool folderExists = AssetDatabase.IsValidFolder(System.IO.Path.Combine("Assets", "Oculus"));

            return folderExists;
        }
    }
}