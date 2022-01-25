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

        static readonly Uri UNITY_LIB_OVR_HELPERS_URL = new Uri("https://github.com/Wisc-Online/Unity-OvrHelpers.git");

        const string UNITY_LIB_MR_PATH = "Assets/FVTC/LearningInnovations-MR";
        const string UNITY_LIB_OVR_HELPERS_PATH = "Assets/FVTC/LearningInnovations-OvrHelpers";



        [MenuItem("Learning Innovations/Mixed Reality/Install")]
        static void InstallMixedRealitySubmodule()
        {
            if (GitHelper.PromptUserToDownloadGitIfNotInstalled())
            {
                GitHelper.AddModule(UNITY_LIB_MR_PATH, UNITY_LIB_MR_URL);

                AssetDatabase.Refresh();
            }
        }

        [MenuItem("Learning Innovations/Mixed Reality/Install", true)]
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

        [MenuItem("Learning Innovations/Git/Set Origin")]
        static void GitSetOrigin()
        {
            Action<string> callback;


            callback = userInput =>
            {
                Uri uri = null;


                if (!string.IsNullOrEmpty(userInput) && Uri.TryCreate(userInput, UriKind.Absolute, out uri))
                {
                    bool success;

                    if (GitHelper.GetRemotes().Any(x => x.Name == "origin"))
                    {
                        success = GitHelper.SetRemote(new GitHelper.Remote
                        {
                            Name = "origin",
                            FetchUri = uri,
                            PushUri = uri
                        });
                    }
                    else
                    {
                        success = GitHelper.AddRemote(new GitHelper.Remote
                        {
                            Name = "origin",
                            FetchUri = uri,
                            PushUri = uri
                        });
                    }

                    if (success)
                    {
                        Dialog.Close("Git Remote Set", "Git remote set successfully. Have a nice day.");
                    }
                    else
                    {
                        Dialog.ErrorSeeOutput();
                    }
                }
                else
                {

                    if (Dialog.OkCancel("Not valid URL", "The value entered is not a valid URL.  Try again?"))
                    {
                        // try again, bozo!
                        GitSetOrigin();
                    }
                }
            };

            string originUrl;

            var origin = GitHelper.GetRemotes().Where(x => x.Name == "origin").FirstOrDefault();

            originUrl = (origin == null) ? null : origin.FetchUri.AbsoluteUri;

            Dialog.Prompt("Set Git Origin", "Specify the URL of your Git origin.", originUrl, callback);
        }

        [MenuItem("Learning Innovations/Git/Set Origin", true)]
        static bool ValidateGitSetOrigin()
        {
            return GitHelper.IsProjectGitRepository;
        }

        [MenuItem("Learning Innovations/Git/Commit All Changes")]
        static void GitCommitAll()
        {
            if (UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes())
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



        [MenuItem("Learning Innovations/Git/Undo All Changes")]
        static void GitUndo()
        {
            if (Dialog.YesNo("Undo All Changes?", "This process will undo any changes made since your last commit.\r\nAre you sure you want to continue?"))
            {
                if (UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes())
                {

                    // add all non-staged changes to stage
                    if (GitHelper.AddAll() && GitHelper.Stash() && GitHelper.StashDrop())
                    {
                        // success!!
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        Dialog.Close("Error", "There was a problem undoing your changes.\r\nSee console log for details.");
                    }
                }
            }
        }


        [MenuItem("Learning Innovations/Git/Undo All Changes", true)]
        static bool ValidateGitUndo()
        {
            return GitHelper.IsProjectGitRepository;
        }
    }
}