using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.Editor
{
    public class BuildMenu
    {
        const string BUILDS_DIRECTORY = "builds";

        [MenuItem("Learning Innovations/Build/Standalone/Windows (64-bit)")]
        static void BuildStandaloneWindows64()
        {
            Build(new BuildSettings
            {
                BuildTargetGroup = BuildTargetGroup.Standalone,
                BuildTarget = BuildTarget.StandaloneWindows64
            });
        }

        [MenuItem("Learning Innovations/Build/Standalone/Windows (32-bit)")]
        static void BuildStandaloneWindows32()
        {
            Build(new BuildSettings
            {
                BuildTargetGroup = BuildTargetGroup.Standalone,
                BuildTarget = BuildTarget.StandaloneWindows
            });
        }

        [MenuItem("Learning Innovations/Build/Standalone/Mac")]
        static void BuildStandaloneMac()
        {
            Build(new BuildSettings
            {
                BuildTargetGroup = BuildTargetGroup.Standalone,
                BuildTarget = BuildTarget.StandaloneOSX
            });
        }

        [MenuItem("Learning Innovations/Build/Standalone/Linux")]
        static void BuildStandaloneLinux()
        {
            Build(new BuildSettings
            {
                BuildTargetGroup = BuildTargetGroup.Standalone,
                BuildTarget = BuildTarget.StandaloneLinux
            });
        }

        [MenuItem("Learning Innovations/Build/Android/Android (No VR)")]
        static void BuildAndroid()
        {
            Build(new BuildSettings
            {
                BuildTargetGroup = BuildTargetGroup.Android,
                BuildTarget = BuildTarget.Android
            });
        }


        [MenuItem("Learning Innovations/Build/Android/Google Cardboard")]
        static void BuildAndroidGoogleCardboard()
        {
            Build(new BuildSettings
            {
                BuildTargetGroup = BuildTargetGroup.Android,
                BuildTarget = BuildTarget.Android,
                IsVirtualRealitySupported = true,
                VirtualRealitySDKs = new string[] { "cardboard" },
                NameSuffix = "Cardboard"
            });
        }


        [MenuItem("Learning Innovations/Build/Android/Oculus Go")]
        static void BuildAndroidOculusGo()
        {
            Build(new BuildSettings
            {
                BuildTargetGroup = BuildTargetGroup.Android,
                BuildTarget = BuildTarget.Android,
                IsVirtualRealitySupported = true,
                VirtualRealitySDKs = new string[] { "Oculus" },
                NameSuffix = "Oculus"
            });
        }

        private static BuildSettings GetCurrentBuildSettings()
        {
            return new BuildSettings
            {
                BuildTarget = EditorUserBuildSettings.activeBuildTarget,
                BuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup,
                IsVirtualRealitySupported = PlayerSettings.virtualRealitySupported,
                VirtualRealitySDKs = PlayerSettings.GetVirtualRealitySDKs(EditorUserBuildSettings.selectedBuildTargetGroup),
            };
        }

        private static void Build(BuildSettings buildSettings)
        {
            var oldBuildSettings = GetCurrentBuildSettings();

            DirectoryInfo buildOutputDirectory;
            EditorBuildSettingsScene[] scenes;
            BuildOptions buildOptions = BuildOptions.None;

            EditorUtility.DisplayProgressBar("Build", "Starting the build process.", 0f);

            if (oldBuildSettings.BuildTarget != buildSettings.BuildTarget || oldBuildSettings.BuildTargetGroup != buildSettings.BuildTargetGroup)
            {
                EditorUtility.DisplayProgressBar("Build", "Switching current build target to " + buildSettings.BuildTargetGroup.ToString() + " - " + buildSettings.BuildTarget.ToString(), 0f);
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, buildSettings.BuildTarget);

                // SwitchActiveBuildTarget clears the progress bar, so show it again
                EditorUtility.DisplayProgressBar("Build", "Starting the build process.", 0f);
            }

            PlayerSettings.virtualRealitySupported = buildSettings.IsVirtualRealitySupported;
            PlayerSettings.SetVirtualRealitySDKs(buildSettings.BuildTargetGroup, buildSettings.VirtualRealitySDKs);

            if (EditorUserBuildSettings.development)
                buildOptions |= BuildOptions.Development;

            if (EditorUserBuildSettings.allowDebugging)
                buildOptions |= BuildOptions.AllowDebugging;

            if (EditorUserBuildSettings.compressFilesInPackage)
                buildOptions |= BuildOptions.CompressWithLz4;

            scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).ToArray();

            var assetsDir = new DirectoryInfo(Application.dataPath);

            var projectDir = assetsDir.Parent;

            var buildsDir = new DirectoryInfo(Path.Combine(projectDir.FullName, BUILDS_DIRECTORY));

            buildOutputDirectory = new DirectoryInfo(Path.Combine(buildsDir.FullName, buildSettings.BuildTarget.ToString()));

            if (!buildsDir.Exists)
                buildsDir.Create();

            if (!buildOutputDirectory.Exists)
                buildOutputDirectory.Create();

            FileInfo buildOutputExe = new FileInfo(Path.Combine(buildOutputDirectory.FullName, string.Format("{0}{2}.{1}", 
                PlayerSettings.productName, 
                buildSettings.GetBuildFileExtension(),
                string.IsNullOrEmpty(buildSettings.NameSuffix) ? "" : " (" + buildSettings.NameSuffix + ")"
                )));

            BuildReport buildReport = null;

            try
            {
                buildReport = BuildPipeline.BuildPlayer(
                        scenes,
                        buildOutputExe.FullName,
                        buildSettings.BuildTarget,
                        buildOptions);
            }
            finally
            {
                if (oldBuildSettings.BuildTarget != buildSettings.BuildTarget || oldBuildSettings.BuildTargetGroup != buildSettings.BuildTargetGroup)
                {
                    EditorUtility.DisplayProgressBar("Build", "Switching current build target back to " + buildSettings.BuildTargetGroup.ToString() + " - " + buildSettings.BuildTarget.ToString(), 0f);
                    EditorUserBuildSettings.SwitchActiveBuildTarget(buildSettings.BuildTargetGroup, oldBuildSettings.BuildTarget);
                }

                EditorUtility.ClearProgressBar();

                PlayerSettings.virtualRealitySupported = oldBuildSettings.IsVirtualRealitySupported;
                PlayerSettings.SetVirtualRealitySDKs(oldBuildSettings.BuildTargetGroup, oldBuildSettings.VirtualRealitySDKs);

                switch (buildReport.summary.result)
                {
                    case BuildResult.Succeeded:
                        if (EditorUtility.DisplayDialog("Build Succeeded", "Do you want to open the build folder?", "Yes", "No"))
                        {
                            //System.Diagnostics.Process.Start("explorer.exe", buildOutputDirectory.FullName);
                            Process.Start("explorer.exe", string.Format("/select, \"{0}\"", buildOutputExe.FullName));
                        }
                        break;
                    case BuildResult.Failed:
                        EditorUtility.DisplayDialog("Build Failed", "The build failed :(\r\nSee the console window for details.", "OK");
                        break;
                    default:
                        // do nothing, assume the user cancelled
                        break;
                }
            }
        }


        public class BuildSettings
        {
            public BuildTargetGroup BuildTargetGroup { get; set; }
            public BuildTarget BuildTarget { get; set; }
            public bool IsVirtualRealitySupported { get; set; }
            public string[] VirtualRealitySDKs { get; set; }
            public string NameSuffix { get; internal set; }

            public string GetBuildFileExtension()
            {
                switch (BuildTargetGroup)
                {
                    case BuildTargetGroup.Android:
                        return "apk";
                }

                // if all else fails, assume .exe
                return "exe";
            }
        }

        static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
    }
}