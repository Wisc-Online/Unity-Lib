using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.Editor
{
    public static class ProcessHelper
    {
        public static Process Start(string path)
        {
            return Start(path, null);
        }

        public static Process Start(string path, string arguments) {
            return Start(path, arguments, _debugLogMessage);
        }

        public static Process Start(string path, string arguments, Action<string> stdOutCallback)
        {
            return Start(path, arguments, stdOutCallback, _debugLogError);
        }

        static Action<string> _debugLogError = str => UnityEngine.Debug.LogError(str);
        static Action<string> _debugLogMessage = str => UnityEngine.Debug.Log(str);

        public static Process Start(string path, string arguments, Action<string> stdOutCallback, Action<string> stdErrCallback)
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = path,
                WorkingDirectory = System.IO.Directory.GetParent(Application.dataPath).FullName,
                UseShellExecute = false,
                Arguments = arguments,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            });

            if (stdOutCallback != null)
            {
                string stdOut;

                while((stdOut = process.StandardOutput.ReadLine()) != null)
                {
                    stdOutCallback(stdOut);
                }
            }

            if (stdErrCallback != null)
            {
                string stdOut;

                while ((stdOut = process.StandardError.ReadLine()) != null)
                {
                    stdErrCallback(stdOut);
                }
            }

            return process;
        }

        public static bool StartAndWaitForExit(string path)
        {
            return StartAndWaitForExit(path, null);
        }

        const int EXIT_CODE_SUCCESS = 0;

        public static bool StartAndWaitForExit(string path, string arguments)
        {
            using (var process = Start(path, arguments))
            {
                process.WaitForExit();

                return process.ExitCode == EXIT_CODE_SUCCESS;
            }
        }


    }
}
