using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    public class GameDetectionService
    {
        private readonly SteamDetectionService _steamService;
        private readonly RiotDetectionService _riotService;
        private readonly EpicDetectionService _epicService;
        private readonly BattleNetDetectionService _battleNetService;
        private readonly StandaloneDetectionService _standaloneService;

        public GameDetectionService(
            SteamDetectionService steamService,
            RiotDetectionService riotService,
            EpicDetectionService epicService,
            BattleNetDetectionService battleNetService,
            StandaloneDetectionService standaloneService)
        {
            _steamService = steamService;
            _riotService = riotService;
            _epicService = epicService;
            _battleNetService = battleNetService;
            _standaloneService = standaloneService;
        }

        public async Task<ObservableCollection<GameInfo>> ScanForInstalledGamesAsync()
        {
            var allGames = new List<GameInfo>();

            try
            {
                // Run scans in parallel
                var tasks = new List<Task<List<GameInfo>>>
                {
                    _steamService.DetectSteamGamesAsync(),
                    _riotService.DetectRiotGamesAsync(),
                    _epicService.DetectEpicGamesAsync(),
                    _battleNetService.DetectBattleNetGamesAsync(),
                    _standaloneService.DetectStandaloneGamesAsync()
                };

                var results = await Task.WhenAll(tasks);

                foreach (var result in results)
                {
                    allGames.AddRange(result);
                }
            }
            catch (Exception ex)
            {
                // In a real app, log this
                Console.WriteLine($"Error during game scan: {ex.Message}");
            }

            // Remove duplicates by install path
            var uniqueGames = allGames
                .GroupBy(g => g.Name?.ToLowerInvariant())
                .Select(g => g.First())
                .OrderBy(g => g.Name)
                .ToList();

            return new ObservableCollection<GameInfo>(uniqueGames);
        }

        public async Task RefreshGameStatesAsync(ObservableCollection<GameInfo> games)
        {
            await Task.Run(() =>
            {
                foreach (var game in games)
                {
                    // Update state based on file existence
                    if (!string.IsNullOrEmpty(game.ExecutablePath) && System.IO.File.Exists(game.ExecutablePath))
                    {
                        if (game.CurrentState == GameState.NotInstalled)
                            game.CurrentState = GameState.Installed;
                    }
                    else
                    {
                        game.CurrentState = GameState.NotInstalled;
                    }
                }
            });
        }
    }
}
