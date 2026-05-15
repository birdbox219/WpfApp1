using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using WpfApp1.Helpers;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    public class LauncherService
    {
        public async Task LaunchGameAsync(GameInfo game)
        {
            if (game == null || string.IsNullOrEmpty(game.ExecutablePath))
                return;

            try
            {
                game.CurrentState = GameState.Launching;

                // Handle Steam protocol specifically if needed
                if (game.Platform == GamePlatform.Steam && game.ExecutablePath.StartsWith("steam://"))
                {
                    Process.Start(new ProcessStartInfo(game.ExecutablePath) { UseShellExecute = true });
                    // For Steam, we rely on the monitor to find the process later
                    return;
                }

                if (!File.Exists(game.ExecutablePath))
                {
                    game.CurrentState = GameState.Error;
                    return;
                }

                await Task.Run(() =>
                {
                    var process = ProcessHelper.StartProcess(game.ExecutablePath, game.InstallPath, game.LaunchArguments);
                    if (process != null)
                    {
                        game.ProcessId = process.Id;
                        game.IsRunning = true;
                        game.CurrentState = GameState.Running;
                        game.LastPlayed = DateTime.Now;
                    }
                    else
                    {
                        game.CurrentState = GameState.Error;
                    }
                });
            }
            catch (Exception)
            {
                game.CurrentState = GameState.Error;
            }
        }

        public async Task StopGameAsync(GameInfo game)
        {
            if (game == null || game.ProcessId == null) return;

            try
            {
                await Task.Run(() =>
                {
                    var process = ProcessHelper.GetProcessById(game.ProcessId.Value);
                    if (process != null && !process.HasExited)
                    {
                        process.Kill();
                    }
                });
            }
            catch { }
        }

        public bool IsGameRunning(GameInfo game)
        {
            if (game == null) return false;

            if (game.ProcessId.HasValue)
            {
                return ProcessHelper.IsProcessRunning(game.ProcessId.Value);
            }

            // Fallback: search by executable name
            var process = ProcessHelper.FindProcessByExecutable(game.ExecutablePath);
            if (process != null)
            {
                game.ProcessId = process.Id;
                return true;
            }

            return false;
        }
    }
}
