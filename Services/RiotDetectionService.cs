using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WpfApp1.Helpers;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    public class RiotDetectionService
    {
        public async Task<List<GameInfo>> DetectRiotGamesAsync()
        {
            return await Task.Run(() =>
            {
                var games = new List<GameInfo>();
                var riotGamesRoot = @"C:\Riot Games";

                if (Directory.Exists(riotGamesRoot))
                {
                    var gameDirs = Directory.GetDirectories(riotGamesRoot);
                    foreach (var dir in gameDirs)
                    {
                        var name = Path.GetFileName(dir);
                        if (name.Equals("VALORANT", StringComparison.OrdinalIgnoreCase))
                        {
                            var exe = Path.Combine(dir, @"live\ShooterGame\Binaries\Win64\VALORANT-Win64-Shipping.exe");
                            if (File.Exists(exe))
                            {
                                games.Add(new GameInfo
                                {
                                    Name = "VALORANT",
                                    InstallPath = dir,
                                    ExecutablePath = exe,
                                    BannerPath = "https://images.contentstack.io/v3/assets/blt73edb393b61d40a2/blt809d437016258055/5f973c52a329432890637c38/VALORANT_Live_Keyart_5.jpg",
                                    Platform = GamePlatform.Riot,
                                    IsInstalled = true,
                                    CurrentState = GameState.Installed
                                });
                            }
                        }
                        else if (name.Equals("League of Legends", StringComparison.OrdinalIgnoreCase))
                        {
                            var exe = Path.Combine(dir, "LeagueClient.exe");
                            if (File.Exists(exe))
                            {
                                games.Add(new GameInfo
                                {
                                    Name = "League of Legends",
                                    InstallPath = dir,
                                    ExecutablePath = exe,
                                    BannerPath = "https://images.contentstack.io/v3/assets/blt73edb393b61d40a2/bltd0230219c6f272a2/5e1762c95333f2113337f71b/League_of_Legends_Logo_Transparent.png",
                                    Platform = GamePlatform.Riot,
                                    IsInstalled = true,
                                    CurrentState = GameState.Installed
                                });
                            }
                        }
                    }
                }

                return games;
            });
        }
    }
}
