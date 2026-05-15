using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WpfApp1.Helpers
{
    public static class FileSystemHelper
    {
        public static bool FileExists(string path)
        {
            return !string.IsNullOrEmpty(path) && File.Exists(path);
        }

        public static bool DirectoryExists(string path)
        {
            return !string.IsNullOrEmpty(path) && Directory.Exists(path);
        }

        public static long GetDirectorySize(string path)
        {
            if (!DirectoryExists(path)) return 0;

            try
            {
                var dirInfo = new DirectoryInfo(path);
                return dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
            }
            catch
            {
                return 0;
            }
        }

        public static IEnumerable<string> FindFiles(string rootPath, string searchPattern, int maxDepth = 3)
        {
            if (!DirectoryExists(rootPath)) yield break;

            var queue = new Queue<(string Path, int Depth)>();
            queue.Enqueue((rootPath, 0));

            while (queue.Count > 0)
            {
                var (currentPath, depth) = queue.Dequeue();

                string[] files = null;
                try
                {
                    files = Directory.GetFiles(currentPath, searchPattern);
                }
                catch { }

                if (files != null)
                {
                    foreach (var file in files) yield return file;
                }

                if (depth < maxDepth)
                {
                    string[] subDirs = null;
                    try
                    {
                        subDirs = Directory.GetDirectories(currentPath);
                    }
                    catch { }

                    if (subDirs != null)
                    {
                        foreach (var subDir in subDirs)
                        {
                            queue.Enqueue((subDir, depth + 1));
                        }
                    }
                }
            }
        }
    }
}
