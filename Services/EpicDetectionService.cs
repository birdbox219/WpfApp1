using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WpfApp1.Helpers;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    public class EpicDetectionService
    {
        public async Task<List<GameInfo>> DetectEpicGamesAsync()
        {
            return await Task.Run(() =>
            {
                var games = new List<GameInfo>();
                var manifestPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Epic\EpicGamesLauncher\Data\Manifests");

                if (!Directory.Exists(manifestPath)) return games;

                var manifestFiles = Directory.GetFiles(manifestPath, "*.item");
                foreach (var file in manifestFiles)
                {
                    try
                    {
                        var content = File.ReadAllText(file);
                        
                        // Simple manual JSON-like parsing to avoid external dependencies
                        var displayName = GetJsonValue(content, "DisplayName");
                        var installLocation = GetJsonValue(content, "InstallLocation").Replace(@"\\", @"\");
                        var launchExecutable = GetJsonValue(content, "LaunchExecutable");

                        if (!string.IsNullOrEmpty(displayName) && !string.IsNullOrEmpty(installLocation))
                        {
                            var exePath = Path.Combine(installLocation, launchExecutable);
                            if (Directory.Exists(installLocation))
                            {
                                games.Add(new GameInfo
                                {
                                    Name = displayName,
                                    InstallPath = installLocation,
                                    ExecutablePath = File.Exists(exePath) ? exePath : null,
                                    BannerPath = GetJsonValue(content, "Thumbnail"),
                                    Platform = GamePlatform.EpicGames,
                                    IsInstalled = true,
                                    CurrentState = GameState.Installed
                                });
                            }
                        }
                    }
                    catch { }
                }

                return games;
            });
        }

        private string GetJsonValue(string json, string key)
        {
            var pattern = $@"""{key}""\s*:\s*""([^""]+)""";
            var match = System.Text.RegularExpressions.Regex.Match(json, pattern);
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
    }
}
