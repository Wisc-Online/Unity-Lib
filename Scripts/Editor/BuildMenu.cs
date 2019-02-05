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
            Build(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Learning Innovations/Build/Standalone/Windows (32-bit)")]
        static void BuildStandaloneWindows32()
        {
            Build(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
        }
        
        [MenuItem("Learning Innovations/Build/Standalone/Mac")]
        static void BuildStandaloneMac()
        {
            Build(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
        }

        [MenuItem("Learning Innovations/Build/Standalone/Linux")]
        static void BuildStandaloneLinux()
        {
            Build(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux);
        }

        [MenuItem("Learning Innovations/Build/Android/Android")]
        static void BuildAndroid()
        {
            Build(BuildTargetGroup.Android, BuildTarget.Android);
        }


        [MenuItem("Learning Innovations/Build/Android/Oculus Go")]
        static void BuildAndroidOculusGo()
        {
            Build(BuildTargetGroup.Android, BuildTarget.Android);
        }

        private static void Build(BuildTargetGroup buildTargetGroup, BuildTarget buildTarget)
        {
            BuildTargetGroup oldBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            BuildTarget oldBuildTarget = EditorUserBuildSettings.activeBuildTarget;


            DirectoryInfo buildOutputDirectory;
            EditorBuildSettingsScene[] scenes;
            BuildOptions buildOptions = BuildOptions.None;

            EditorUtility.DisplayProgressBar("Build", "Starting the build process.", 0f);

            if (oldBuildTarget != buildTarget || oldBuildTargetGroup != buildTargetGroup)
            {
                EditorUtility.DisplayProgressBar("Build", "Switching current build target to " + buildTargetGroup.ToString() + " - " + buildTarget.ToString(), 0f);
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, buildTarget);

                // SwitchActiveBuildTarget clears the progress bar, so show it again
                EditorUtility.DisplayProgressBar("Build", "Starting the build process.", 0f);
            }

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

            buildOutputDirectory = new DirectoryInfo(Path.Combine(buildsDir.FullName, buildTarget.ToString()));

            if (!buildsDir.Exists)
                buildsDir.Create();

            if (!buildOutputDirectory.Exists)
                buildOutputDirectory.Create();

            FileInfo buildOutputExe = new FileInfo(Path.Combine(buildOutputDirectory.FullName, string.Format("{0}.exe", PlayerSettings.productName)));

            BuildReport buildReport = null;

            try
            {
                buildReport = BuildPipeline.BuildPlayer(
                        scenes,
                        buildOutputExe.FullName,
                        buildTarget,
                        buildOptions);
            }
            finally
            {
                if (oldBuildTarget != buildTarget || oldBuildTargetGroup != buildTargetGroup)
                {
                    EditorUtility.DisplayProgressBar("Build", "Switching current build target back to " + oldBuildTargetGroup.ToString() + " - " + oldBuildTarget.ToString(), 0f);
                    EditorUserBuildSettings.SwitchActiveBuildTarget(oldBuildTargetGroup, oldBuildTarget);
                }

                EditorUtility.ClearProgressBar();

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