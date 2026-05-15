using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WpfApp1.Helpers;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    public class GameMonitoringService : IDisposable
    {
        private readonly ObservableCollection<GameInfo> _monitoredGames;
        private CancellationTokenSource _cts;
        private bool _isMonitoring;

        public event Action<GameInfo> GameStarted;
        public event Action<GameInfo> GameStopped;
        public event Action<GameInfo, int> GameCrashed;

        public GameMonitoringService(ObservableCollection<GameInfo> monitoredGames)
        {
            _monitoredGames = monitoredGames;
        }

        public void StartMonitoring()
        {
            if (_isMonitoring) return;

            _isMonitoring = true;
            _cts = new CancellationTokenSource();
            
            Task.Run(() => MonitorLoop(_cts.Token));
        }

        public void StopMonitoring()
        {
            _cts?.Cancel();
            _isMonitoring = false;
        }

        private async Task MonitorLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    foreach (var game in _monitoredGames.ToList())
                    {
                        bool wasRunning = game.IsRunning;
                        bool isRunningNow = false;

                        if (game.ProcessId.HasValue)
                        {
                            isRunningNow = ProcessHelper.IsProcessRunning(game.ProcessId.Value);
                        }

                        // If not found by PID, try finding by executable name (e.g. for Steam games launched via protocol)
                        if (!isRunningNow && !string.IsNullOrEmpty(game.ExecutablePath))
                        {
                            var process = ProcessHelper.FindProcessByExecutable(game.ExecutablePath);
                            if (process != null)
                            {
                                game.ProcessId = process.Id;
                                isRunningNow = true;
                            }
                        }

                        if (isRunningNow && !wasRunning)
                        {
                            game.IsRunning = true;
                            game.CurrentState = GameState.Running;
                            GameStarted?.Invoke(game);
                        }
                        else if (!isRunningNow && wasRunning)
                        {
                            game.IsRunning = false;
                            game.CurrentState = GameState.Installed;
                            game.ProcessId = null;
                            GameStopped?.Invoke(game);
                        }
                    }
                }
                catch { }

                await Task.Delay(3000, token); // Poll every 3 seconds
            }
        }

        public void Dispose()
        {
            StopMonitoring();
            _cts?.Dispose();
        }
    }
}
