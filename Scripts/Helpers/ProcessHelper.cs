using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FVTC.LearningInnovations.Unity.Helpers
{
    public static class ProcessHelper
    {
        public static Process Start(string path)
        {
            return Start(path, null);
        }

        public static Process Start(string path, string arguments)
        {
            var process = Process.Start(new ProcessStartInfo
            {
                UseShellExecute = false,
                Arguments = arguments,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            });

            process.OutputDataReceived += (sender, e) =>
            {
                UnityEngine.Debug.LogWarning(e.Data);
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                UnityEngine.Debug.LogError(e.Data);
            };

            return process;
        }

        public static void StartAndWaitForExit(string path)
        {
            StartAndWaitForExit(path, null);
        }

        public static void StartAndWaitForExit(string path, string arguments)
        {
            using (var process = Start(path, arguments))
            {
                process.WaitForExit();
            }
        }


    }
}
