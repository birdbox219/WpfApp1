using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WpfApp1.Helpers;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    public class BattleNetDetectionService
    {
        public async Task<List<GameInfo>> DetectBattleNetGamesAsync()
        {
            return await Task.Run(() =>
            {
                var games = new List<GameInfo>();
                
                // Common Battle.net install directories
                var searchPaths = new[]
                {
                    @"C:\Program Files (x86)\Overwatch",
                    @"C:\Program Files (x86)\Diablo IV",
                    @"C:\Program Files (x86)\Call of Duty",
                    @"C:\Program Files (x86)\Hearthstone",
                    @"C:\Program Files (x86)\World of Warcraft"
                };

                foreach (var path in searchPaths)
                {
                    if (Directory.Exists(path))
                    {
                        var name = Path.GetFileName(path);
                        var exe = FindBattleNetExe(path);
                        
                        var bannerUrl = GetBattleNetBanner(name);
                        
                        games.Add(new GameInfo
                        {
                            Name = name,
                            InstallPath = path,
                            ExecutablePath = exe,
                            BannerPath = bannerUrl,
                            Platform = GamePlatform.BattleNet,
                            IsInstalled = true,
                            CurrentState = GameState.Installed
                        });
                    }
                }

                // Also check Registry for more games
                CheckRegistry(games);

                return games;
            });
        }

        private void CheckRegistry(List<GameInfo> games)
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"))
                {
                    if (key == null) return;

                    foreach (var subkeyName in key.GetSubKeyNames())
                    {
                        using (var subkey = key.OpenSubKey(subkeyName))
                        {
                            var publisher = subkey?.GetValue("Publisher") as string;
                            if (publisher != null && (publisher.Contains("Blizzard") || publisher.Contains("Activision")))
                            {
                                var displayName = subkey.GetValue("DisplayName") as string;
                                var installLocation = subkey.GetValue("InstallLocation") as string;

                                if (!string.IsNullOrEmpty(displayName) && !string.IsNullOrEmpty(installLocation) && Directory.Exists(installLocation))
                                {
                                    if (!games.Any(g => g.InstallPath.Equals(installLocation, StringComparison.OrdinalIgnoreCase)))
                                    {
                                        games.Add(new GameInfo
                                        {
                                            Name = displayName,
                                            InstallPath = installLocation,
                                            ExecutablePath = FindBattleNetExe(installLocation),
                                            Platform = GamePlatform.BattleNet,
                                            IsInstalled = true,
                                            CurrentState = GameState.Installed
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private string GetBattleNetBanner(string gameName)
        {
            if (gameName.Contains("Overwatch")) return "https://blz-contentstack-images.akamaized.net/v3/assets/blt9c12f2495c23fbf3/blt72750e6878b1758c/629910d6a2a095493019859f/Overwatch2_Keyart_Vertical.jpg";
            if (gameName.Contains("Diablo IV")) return "https://blz-contentstack-images.akamaized.net/v3/assets/blt9c12f2495c23fbf3/blt81682701f5c66d1f/638972e3a1f10c10972b226e/D4_Gold_Standard_KV_Vertical.jpg";
            if (gameName.Contains("Hearthstone")) return "https://blz-contentstack-images.akamaized.net/v3/assets/blt9c12f2495c23fbf3/blt8355609462f6b31e/5db8c94625d8041c2c31c448/hearthstone-logo.png";
            if (gameName.Contains("World of Warcraft")) return "https://blz-contentstack-images.akamaized.net/v3/assets/blt9c12f2495c23fbf3/blt36500f40d5718a3d/629910d6a2a095493019859f/wow-logo.png";
            return string.Empty;
        }

        private string FindBattleNetExe(string directory)
        {
            var exes = Directory.GetFiles(directory, "*.exe", SearchOption.TopDirectoryOnly);
            // Battle.net games often have a " Launcher.exe" and a main exe.
            // Prefer the main exe (usually larger or without 'Launcher' in name)
            var mainExe = exes.FirstOrDefault(e => !e.Contains("Launcher") && !e.Contains("Setup"));
            return mainExe ?? exes.FirstOrDefault();
        }
    }
}
