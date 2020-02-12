using System;
using System.IO;
using System.Linq;

namespace FVTC.LearningInnovations.Unity.Helpers
{
    public static class EnvironmentHelpers
    {
        public static string GetExecutablePath(string executableName)
        {
            var exePath = Environment.GetEnvironmentVariable("PATH").Split(';').Select(p => Path.Combine(p, executableName)).Where(p => File.Exists(p)).FirstOrDefault();

            return exePath;
        }
    }
}
