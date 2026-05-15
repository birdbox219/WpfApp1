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
            "adobe", "apple", "mozilla", "oracle", "logitech", "corsair", "razer", "hp ", "dell", "lenovo", "asus", "acer"
        };

        // Keywords in app names to ignore
        private readonly string[] _ignoredKeywords = new[]
        {
            "redistributable", "update", "runtime", "service", "driver", "sdk", "tools", "visual c++", "framework", "antivirus", "player"
        };

        public async Task<List<GameInfo>> DetectStandaloneGamesAsync()
        {
            var games = new List<GameInfo>();
            var installedApps = GetInstalledApplications();

            // Filter out obvious non-games
            var potentialGames = installedApps.Where(app => 
                !string.IsNullOrEmpty(app.Name) && 
                !string.IsNullOrEmpty(app.InstallPath) &&
                !IsIgnored(app.Name, app.Publisher)
            ).ToList();

            // To avoid spamming Steam API, limit concurrency
            var semaphore = new System.Threading.SemaphoreSlim(5);
            var tasks = potentialGames.Select(async app =>
            {
                await semaphore.WaitAsync();
                try
                {
                    // Check if it's a game via Steam API
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

        private bool IsIgnored(string name, string publisher)
        {
            var lowerName = name.ToLowerInvariant();
            var lowerPub = (publisher ?? "").ToLowerInvariant();

            if (_ignoredPublishers.Any(p => lowerPub.Contains(p))) return true;
            if (_ignoredKeywords.Any(k => lowerName.Contains(k))) return true;

            return false;
        }

        private async Task<bool> VerifyGameWithSteamAsync(string name)
        {
            try
            {
                // Clean up name (e.g., remove "v1.0", "GOG", etc.)
                var cleanName = name.Replace("GOG.com", "").Replace("GOG", "").Trim();
                if (string.IsNullOrWhiteSpace(cleanName)) return false;

                var url = $"https://store.steampowered.com/api/storesearch/?term={Uri.EscapeDataString(cleanName)}&l=english&cc=US";
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    var total = doc.RootElement.GetProperty("total").GetInt32();
                    return total > 0; // If Steam returns at least 1 match, we consider it a game!
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
