using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WpfApp1.Helpers
{
    public static class ProcessHelper
    {
        public static Process StartProcess(string executablePath, string workingDirectory = null, string arguments = null)
        {
            if (string.IsNullOrEmpty(executablePath)) return null;

            var startInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                WorkingDirectory = workingDirectory ?? Path.GetDirectoryName(executablePath),
                Arguments = arguments ?? string.Empty,
                UseShellExecute = true
            };

            try
            {
                return Process.Start(startInfo);
            }
            catch
            {
                return null;
            }
        }

        public static Process GetProcessById(int processId)
        {
            try
            {
                return Process.GetProcessById(processId);
            }
            catch
            {
                return null;
            }
        }

        public static Process FindProcessByExecutable(string executablePath)
        {
            if (string.IsNullOrEmpty(executablePath)) return null;

            var exeName = Path.GetFileNameWithoutExtension(executablePath);
            return Process.GetProcessesByName(exeName).FirstOrDefault();
        }

        public static bool IsProcessRunning(int processId)
        {
            try
            {
                var process = Process.GetProcessById(processId);
                return process != null && !process.HasExited;
            }
            catch
            {
                return false;
            }
        }
    }
}
