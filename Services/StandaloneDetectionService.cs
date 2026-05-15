using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    public class StandaloneDetectionService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        
        // Publishers to strictly ignore
        private readonly string[] _ignoredPublishers = new[] 
        { 
            "microsoft", "intel", "nvidia", "amd", "realtek", "google", 
            "adobe", "apple", "mozilla", "oracle", "logitech", "corsair", "razer", 
            "hp ", "dell", "lenovo", "asus", "acer", "brave", "qt company", "blender", 
            "git", "github", "docker", "vmware", "canonical", "valve", 
            "epic games", "riot games", "blizzard", "electronic arts", "ubisoft"
        };

        // Keywords in app names to ignore
        private readonly string[] _ignoredKeywords = new[]
        {
            "redistributable", "update", "runtime", "service", "driver", "sdk", "tools", 
            "visual c++", "framework", "antivirus", "player", "browser", "studio", 
            "server", "client", "vpn", "launcher", "engine", "plugin", "extension",
            "support", "manager", "viewer", "setup", "installer"
        };

        public async Task<List<GameInfo>> DetectStandaloneGamesAsync()
        {
            var games = new List<GameInfo>();
            var installedApps = GetInstalledApplications();

            // Filter out obvious non-games and other launcher games
            var potentialGames = installedApps.Where(app => 
                !string.IsNullOrEmpty(app.Name) && 
                !string.IsNullOrEmpty(app.InstallPath) &&
                !IsIgnored(app.Name, app.Publisher, app.InstallPath)
            ).ToList();

            // Increase concurrency to 20 for much faster scanning
            var semaphore = new System.Threading.SemaphoreSlim(20);
            var tasks = potentialGames.Select(async app =>
            {
                await semaphore.WaitAsync();
                try
                {
                    // Strict Steam API check
                    var isGame = await VerifyGameWithSteamAsync(app.Name);
                    if (isGame)
                    {
                        var exePath = FindExecutable(app.InstallPath);
                        if (exePath != null)
                        {
                            lock (games)
                            {
                                games.Add(new GameInfo
                                {
                                    Name = app.Name,
                                    InstallPath = app.InstallPath,
                                    ExecutablePath = exePath,
                                    Platform = GamePlatform.Standalone,
                                    IsInstalled = true,
                                    CurrentState = GameState.Installed
                                });
                            }
                        }
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
            return games;
        }

        private bool IsIgnored(string name, string publisher, string installPath)
        {
            var lowerName = name.ToLowerInvariant();
            var lowerPub = (publisher ?? "").ToLowerInvariant();
            var lowerPath = (installPath ?? "").ToLowerInvariant();

            // Ignore games that are already handled by other launchers
            if (lowerPath.Contains("steamapps") || lowerPath.Contains("epic games") || lowerPath.Contains("riot games") || lowerPath.Contains("battle.net"))
                return true;

            // Ignore common system/dev paths
            if (lowerPath.Contains("windows") || lowerPath.Contains("system32") || lowerPath.Contains("appdata"))
                return true;

            if (_ignoredPublishers.Any(p => lowerPub.Contains(p))) return true;
            if (_ignoredKeywords.Any(k => lowerName.Contains(k))) return true;

            return false;
        }

        private async Task<bool> VerifyGameWithSteamAsync(string name)
        {
            try
            {
                // Clean up name
                var cleanName = name.Replace("GOG.com", "").Replace("GOG", "").Trim();
                if (cleanName.Length <= 2) return false;

                var url = $"https://store.steampowered.com/api/storesearch/?term={Uri.EscapeDataString(cleanName)}&l=english&cc=US";
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    
                    if (doc.RootElement.TryGetProperty("items", out var items) && items.GetArrayLength() > 0)
                    {
                        foreach (var item in items.EnumerateArray())
                        {
                            var steamName = item.GetProperty("name").GetString();
                            
                            // Strict match check
                            if (steamName.Equals(cleanName, StringComparison.OrdinalIgnoreCase))
                                return true;
                            
                            // Loose match check (if Steam appends "Edition" etc.)
                            if (steamName.StartsWith(cleanName, StringComparison.OrdinalIgnoreCase) && 
                                Math.Abs(steamName.Length - cleanName.Length) < 15)
                                return true;
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        private string FindExecutable(string directory)
        {
            try
            {
                if (!Directory.Exists(directory)) return null;

                var exes = Directory.GetFiles(directory, "*.exe", SearchOption.TopDirectoryOnly)
                    .Where(e => !e.ToLower().Contains("unins") && !e.ToLower().Contains("crash") && !e.ToLower().Contains("updater") && !e.ToLower().Contains("report"))
                    .ToList();

                if (exes.Count == 1) return exes[0];

                var dirName = Path.GetFileName(directory);
                var likely = exes.FirstOrDefault(e => Path.GetFileNameWithoutExtension(e).Equals(dirName, StringComparison.OrdinalIgnoreCase));
                
                return likely ?? exes.FirstOrDefault();
            }
            catch { return null; }
        }

        private class AppEntry
        {
            public string Name { get; set; }
            public string Publisher { get; set; }
            public string InstallPath { get; set; }
        }

        private List<AppEntry> GetInstalledApplications()
        {
            var apps = new List<AppEntry>();
            var keys = new[]
            {
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)?.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)?.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
                RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64)?.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")
            };

            foreach (var baseKey in keys)
            {
                if (baseKey == null) continue;

                foreach (var subKeyName in baseKey.GetSubKeyNames())
                {
                    try
                    {
                        using var subKey = baseKey.OpenSubKey(subKeyName);
                        if (subKey == null) continue;

                        var name = subKey.GetValue("DisplayName") as string;
                        var publisher = subKey.GetValue("Publisher") as string;
                        var installPath = subKey.GetValue("InstallLocation") as string;

                        if (!string.IsNullOrEmpty(name))
                        {
                            apps.Add(new AppEntry
                            {
                                Name = name,
                                Publisher = publisher,
                                InstallPath = installPath
                            });
                        }
                    }
                    catch { }
                }
            }

            return apps;
        }
    }
}
