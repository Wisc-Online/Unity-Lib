using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FVTC.LearningInnovations.Unity.Helpers
{
    public class EnvironmentHelpers
    {
        public static string GetExecutablePath(string executableName)
        {
            var exePath = Environment.GetEnvironmentVariable("PATH").Split(';').Select(p => Path.Combine(p, executableName)).Where(p => File.Exists(p)).FirstOrDefault();

            return exePath;
        }
    }
}
