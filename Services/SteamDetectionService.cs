using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WpfApp1.Helpers;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    public class SteamDetectionService
    {
        public string GetSteamInstallPath()
        {
            return RegistryHelper.GetStringValue(RegistryHive.CurrentUser, @"Software\Valve\Steam", "SteamPath") 
                   ?? RegistryHelper.GetStringValue(RegistryHive.LocalMachine, @"SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath");
        }

        public List<string> GetSteamLibraries()
        {
            var libraries = new List<string>();
            var steamPath = GetSteamInstallPath();

            if (string.IsNullOrEmpty(steamPath) || !Directory.Exists(steamPath))
                return libraries;

            // Add default library
            var defaultLibrary = Path.Combine(steamPath, "steamapps");
            if (Directory.Exists(defaultLibrary))
                libraries.Add(defaultLibrary);

            // Check for additional libraries in libraryfolders.vdf
            var vdfPath = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
            if (File.Exists(vdfPath))
            {
                try
                {
                    var content = File.ReadAllText(vdfPath);
                    // Match paths in "path" "C:\\Games\\Steam"
                    var matches = Regex.Matches(content, @"""path""\s+""([^""]+)""");
                    foreach (Match match in matches)
                    {
                        var path = match.Groups[1].Value.Replace(@"\\", @"\");
                        var libPath = Path.Combine(path, "steamapps");
                        if (Directory.Exists(libPath) && !libraries.Contains(libPath, StringComparer.OrdinalIgnoreCase))
                        {
                            libraries.Add(libPath);
                        }
                    }
                }
                catch { }
            }

            return libraries;
        }

        public async Task<List<GameInfo>> DetectSteamGamesAsync()
        {
            return await Task.Run(() =>
            {
                var games = new List<GameInfo>();
                var libraries = GetSteamLibraries();

                foreach (var lib in libraries)
                {
                    if (!Directory.Exists(lib)) continue;

                    var acfFiles = Directory.GetFiles(lib, "appmanifest_*.acf");
                    foreach (var acf in acfFiles)
                    {
                        try
                        {
                            var game = ParseAcfFile(acf, lib);
                            if (game != null) games.Add(game);
                        }
                        catch { }
                    }
                }

                return games;
            });
        }

        private GameInfo ParseAcfFile(string filePath, string libraryPath)
        {
            var content = File.ReadAllText(filePath);
            
            var nameMatch = Regex.Match(content, @"""name""\s+""([^""]+)""");
            var appIdMatch = Regex.Match(content, @"""appid""\s+""([^""]+)""");
            var folderMatch = Regex.Match(content, @"""installdir""\s+""([^""]+)""");

            if (!nameMatch.Success || !appIdMatch.Success || !folderMatch.Success)
                return null;

            var name = nameMatch.Groups[1].Value;
            var appId = appIdMatch.Groups[1].Value;
            var folderName = folderMatch.Groups[1].Value;
            var installPath = Path.Combine(libraryPath, "common", folderName);

            if (!Directory.Exists(installPath)) return null;

            // Try to find the main executable
            var exePath = FindExecutable(installPath);

            // Use Steam's public CDN for game header images — always available
            var bannerUrl = $"https://cdn.akamai.steamstatic.com/steam/apps/{appId}/header.jpg";

            // Fallback: try local cache if needed
            var steamPath = GetSteamInstallPath();
            if (!string.IsNullOrEmpty(steamPath))
            {
                var localBanner = Path.Combine(steamPath.Replace("/", "\\"), "appcache", "librarycache", $"{appId}_header.jpg");
                if (File.Exists(localBanner))
                {
                    bannerUrl = localBanner;
                }
            }

            return new GameInfo
            {
                Name = name,
                InstallPath = installPath,
                ExecutablePath = exePath ?? $"steam://rungameid/{appId}",
                BannerPath = bannerUrl,
                Platform = GamePlatform.Steam,
                IsInstalled = true,
                CurrentState = GameState.Installed,
                LaunchArguments = $"-applaunch {appId}"
            };
        }

        private string FindExecutable(string directory)
        {
            // Simple heuristic: find .exe in root, or most likely one
            var exes = Directory.GetFiles(directory, "*.exe", SearchOption.TopDirectoryOnly);
            if (exes.Length == 1) return exes[0];
            
            // If multiple, look for one matching directory name or just return first
            var dirName = Path.GetFileName(directory);
            var likely = exes.FirstOrDefault(e => Path.GetFileNameWithoutExtension(e).Equals(dirName, StringComparison.OrdinalIgnoreCase));
            
            return likely ?? exes.FirstOrDefault();
        }
    }
}
