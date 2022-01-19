using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
                BuildTarget = BuildTarget.StandaloneLinux64
            });
        }

        [MenuItem("Learning Innovations/Build/Android/No VR")]
        static void BuildAndroid()
        {
            BuildAndroid(new string[] { });
        }

        [MenuItem("Learning Innovations/Build/Android/VR/Google Cardboard")]
        static void BuildAndroidGoogleCardboard()
        {
            BuildAndroid("cardboard");
        }

        [MenuItem("Learning Innovations/Build/Android/VR/Google Cardboard and Oculus")]
        static void BuildAndroidGoogleCardboardOculus()
        {
            BuildAndroid("cardboard", "Oculus");
        }

        [MenuItem("Learning Innovations/Build/Android/VR/Oculus")]
        static void BuildAndroidOculus()
        {
            BuildAndroid("Oculus");


        }


        [MenuItem("Learning Innovations/Build/Web GL")]
        static void BuildWebGL()
        {
            Build(new BuildSettings
            {
                BuildTargetGroup = BuildTargetGroup.WebGL,
                BuildTarget = BuildTarget.WebGL
            });
        }


        static BuildReport BuildAndroid(params string[] vrSdks)
        {
            return BuildAndroid(OnAndroidBuildCompleted, vrSdks);
        }

        static BuildReport BuildAndroid(Action<BuildReport> buildCompletionAction, params string[] vrSdks)
        {
            if (vrSdks == null)
                vrSdks = new string[] { };

            // swap AndroidManifest.xml
            FileInfo androidManifest, targetManifest = null, tempManifest = null;

            androidManifest = new FileInfo(System.IO.Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml"));

            DirectoryInfo manifestDir = new DirectoryInfo(System.IO.Path.Combine(Application.dataPath, "Plugins/Android"));
            if (manifestDir.Exists)
            {
                HashSet<string> desiredTargets = new HashSet<string>(vrSdks.Select(x => x.ToLower()));
                IEnumerable<string> manifestTargets;
                foreach (var file in manifestDir.GetFiles("AndroidManifest.*.xml"))
                {
                    manifestTargets = file.Name.Substring("AndroidManifest.".Length).Replace(".xml", "").ToLower().Split('.');

                    if (desiredTargets.SetEquals(manifestTargets))
                    {
                        targetManifest = file;
                        break;
                    }
                }

                if (targetManifest != null)
                {
                    if (androidManifest.Exists)
                    {
                        // swap the files (swap back after build)
                        tempManifest = new FileInfo(FileUtil.GetUniqueTempPathInProject());

                        FileUtil.MoveFileOrDirectory(androidManifest.FullName, tempManifest.FullName);
                    }

                    FileUtil.MoveFileOrDirectory(targetManifest.FullName, androidManifest.FullName);

                    AssetDatabase.Refresh();
                }
            }

            BuildReport buildReport;

            try
            {
                buildReport = Build(new BuildSettings
                {
                    BuildTargetGroup = BuildTargetGroup.Android,
                    BuildTarget = BuildTarget.Android,
                    IsVirtualRealitySupported = vrSdks.Any(),
                    VirtualRealitySDKs = vrSdks,
                    NameSuffix = string.Join(" + ", vrSdks.Select(x => System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x)).ToArray()),
                    PostBuildAction = buildCompletionAction
                });
            }
            finally
            {
                if (targetManifest != null)
                {
                    FileUtil.MoveFileOrDirectory(androidManifest.FullName, targetManifest.FullName);

                    if (tempManifest != null)
                    {
                        FileUtil.MoveFileOrDirectory(tempManifest.FullName, androidManifest.FullName);
                    }

                    AssetDatabase.Refresh();
                }
            }

            return buildReport;
        }

        private static void OnAndroidBuildCompleted(BuildReport report)
        {
            if (report.summary.result == BuildResult.Succeeded)
            {
                DirectoryInfo outputDir = new DirectoryInfo(System.IO.Path.GetDirectoryName(report.summary.outputPath));
                FileInfo apkFile = new FileInfo(report.summary.outputPath),
                        obbFile = new FileInfo(System.IO.Path.ChangeExtension(apkFile.FullName, "main.obb"));

                using (var scriptFileStream = new System.IO.FileStream(Path.Combine(outputDir.FullName, "adb-install.cmd"), FileMode.Create, FileAccess.Write))
                using (var scriptWriter = new System.IO.StreamWriter(scriptFileStream))
                {
                    string packageName = PlayerSettings.applicationIdentifier;

                    scriptWriter.WriteLine("@echo off");
                    scriptWriter.WriteLine("echo Uninstalling previous APK");
                    scriptWriter.WriteLine(string.Format("adb uninstall {0}", packageName));

                    scriptWriter.WriteLine("echo Removing previous OBB expansion file");
                    scriptWriter.WriteLine(string.Format("adb shell rm -r /sdcard/Android/obb/{0}", packageName));

                    scriptWriter.WriteLine("echo Installing new APK");
                    scriptWriter.WriteLine(string.Format("adb install {0}", apkFile.Name));


                    if (obbFile.Exists)
                    {
                        scriptWriter.WriteLine("echo Uploading new OBB expansion file");

                        //scriptWriter.WriteLine("FOR /F \"tokens = *USEBACKQ\" %%F IN (`adb shell echo $EXTERNAL_STORAGE`) DO (");
                        //scriptWriter.WriteLine("SET SDCARD=%%F");
                        //scriptWriter.WriteLine(")");

                        scriptWriter.WriteLine(string.Format("adb push -p {0} /sdcard/Android/obb/{2}/main.{1}.{2}.obb", obbFile.Name, PlayerSettings.Android.bundleVersionCode, packageName));
                    }

                    scriptWriter.WriteLine("pause");
                }
            }
        }

        private static BuildSettings GetCurrentBuildSettings()
        {
            return new BuildSettings
            {
                BuildTarget = EditorUserBuildSettings.activeBuildTarget,
                BuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup,
#if !UNITY_2019_1_OR_NEWER
                IsVirtualRealitySupported = PlayerSettings.virtualRealitySupported,
                VirtualRealitySDKs = PlayerSettings.GetVirtualRealitySDKs(EditorUserBuildSettings.selectedBuildTargetGroup),
#endif
            };
        }

        private static BuildReport Build(BuildSettings buildSettings)
        {
            var oldBuildSettings = GetCurrentBuildSettings();

            DirectoryInfo buildOutputDirectory;
            EditorBuildSettingsScene[] scenes;
            BuildOptions buildOptions = BuildOptions.None;

            EditorUtility.DisplayProgressBar("Build", "Starting the build process.", 0f);

            if (oldBuildSettings.BuildTarget != buildSettings.BuildTarget || oldBuildSettings.BuildTargetGroup != buildSettings.BuildTargetGroup)
            {
                EditorUtility.DisplayProgressBar("Build", "Switching current build target to " + buildSettings.BuildTargetGroup.ToString() + " - " + buildSettings.BuildTarget.ToString(), 0f);
                EditorUserBuildSettings.SwitchActiveBuildTarget(buildSettings.BuildTargetGroup, buildSettings.BuildTarget);

                // SwitchActiveBuildTarget clears the progress bar, so show it again
                EditorUtility.DisplayProgressBar("Build", "Starting the build process.", 0f);
            }

#if !UNITY_2019_1_OR_NEWER
            PlayerSettings.virtualRealitySupported = buildSettings.IsVirtualRealitySupported;
            PlayerSettings.SetVirtualRealitySDKs(buildSettings.BuildTargetGroup, buildSettings.VirtualRealitySDKs);
#endif

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

            if (!string.IsNullOrEmpty(buildSettings.NameSuffix))
            {
                buildOutputDirectory = new DirectoryInfo(Path.Combine(buildOutputDirectory.FullName, buildSettings.NameSuffix));

                if (!buildOutputDirectory.Exists)
                    buildOutputDirectory.Create();
            }

            if (buildSettings.BuildTargetGroup == BuildTargetGroup.Android)
            {
                buildOutputDirectory = new DirectoryInfo(Path.Combine(buildOutputDirectory.FullName, "v" + PlayerSettings.Android.bundleVersionCode));

                if (!buildOutputDirectory.Exists)
                    buildOutputDirectory.Create();
            }

            FileInfo buildOutputExe;

            if (buildSettings.BuildTargetGroup == BuildTargetGroup.Android)
            {
                buildOutputExe = new FileInfo(Path.Combine(buildOutputDirectory.FullName, string.Format("{0}.{1}",
                    PlayerSettings.applicationIdentifier + ".v" + PlayerSettings.Android.bundleVersionCode,
                    EditorUserBuildSettings.exportAsGoogleAndroidProject ? "aab" : buildSettings.GetBuildFileExtension()
                    )));
            }
            else
            {
                buildOutputExe = new FileInfo(Path.Combine(buildOutputDirectory.FullName, string.Format("{0}.{1}",
                    PlayerSettings.productName,
                    buildSettings.GetBuildFileExtension()
                    )));
            }

            BuildReport buildReport = null;

            try
            {
                buildReport = BuildPipeline.BuildPlayer(
                        scenes,
                        buildOutputExe.FullName,
                        buildSettings.BuildTarget,
                        buildOptions);

                if (buildSettings.PostBuildAction != null)
                {
                    buildSettings.PostBuildAction(buildReport);
                }
            }
            finally
            {
                if (oldBuildSettings.BuildTarget != buildSettings.BuildTarget || oldBuildSettings.BuildTargetGroup != buildSettings.BuildTargetGroup)
                {
                    EditorUtility.DisplayProgressBar("Build", "Switching current build target back to " + buildSettings.BuildTargetGroup.ToString() + " - " + buildSettings.BuildTarget.ToString(), 0f);
                    EditorUserBuildSettings.SwitchActiveBuildTarget(buildSettings.BuildTargetGroup, oldBuildSettings.BuildTarget);
                }

                EditorUtility.ClearProgressBar();

#if !UNITY_2019_1_OR_NEWER
                PlayerSettings.virtualRealitySupported = oldBuildSettings.IsVirtualRealitySupported;
                PlayerSettings.SetVirtualRealitySDKs(oldBuildSettings.BuildTargetGroup, oldBuildSettings.VirtualRealitySDKs);
#endif

                switch (buildReport.summary.result)
                {
                    case BuildResult.Succeeded:

                        if (EditorUtility.DisplayDialog("Build Succeeded", "Do you want to open the build folder?", "Yes", "No"))
                        {
                            FileInfo outputFile = new FileInfo(buildReport.summary.outputPath);

                            //System.Diagnostics.Process.Start("explorer.exe", buildOutputDirectory.FullName);
                            Process.Start("explorer.exe", string.Format("/select, \"{0}\"", outputFile.Exists ? outputFile.FullName : outputFile.Directory.FullName));
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

            return buildReport;
        }


        public class BuildSettings
        {
            public BuildTargetGroup BuildTargetGroup { get; set; }
            public BuildTarget BuildTarget { get; set; }
            public bool IsVirtualRealitySupported { get; set; }
            public string[] VirtualRealitySDKs { get; set; }
            public string NameSuffix { get; internal set; }
            public Action<BuildReport> PostBuildAction { get; internal set; }

            public string GetBuildFileExtension()
            {
                switch (BuildTargetGroup)
                {
                    case BuildTargetGroup.Android:
                        return "apk";
                    case BuildTargetGroup.WebGL:
                        return "";
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