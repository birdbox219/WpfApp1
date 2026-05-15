using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfApp1.Models;
using WpfApp1.Services;
using WpfApp1.ViewModels.Base;

namespace WpfApp1.ViewModels
{
    public class InstalledGamesViewModel : BaseViewModel
    {
        private readonly GameDetectionService _detectionService;
        private readonly LauncherService _launcherService;
        private GameMonitoringService _monitoringService;
        private bool _isScanning;
        private string _scanStatus;

        public ObservableCollection<GameCardViewModel> InstalledGames { get; } = new ObservableCollection<GameCardViewModel>();

        public bool IsScanning
        {
            get => _isScanning;
            set => SetProperty(ref _isScanning, value);
        }

        public string ScanStatus
        {
            get => _scanStatus;
            set => SetProperty(ref _scanStatus, value);
        }

        public ICommand ScanGamesCommand { get; }
        public ICommand RefreshCommand { get; }

        public InstalledGamesViewModel(
            GameDetectionService detectionService,
            LauncherService launcherService)
        {
            _detectionService = detectionService;
            _launcherService = launcherService;

            ScanGamesCommand = new RelayCommand(async _ => await ScanGamesAsync());
            RefreshCommand = new RelayCommand(async _ => await RefreshAsync());

            // Initial scan
            _ = ScanGamesAsync();
        }

        private async Task ScanGamesAsync()
        {
            if (IsScanning) return;

            IsScanning = true;
            ScanStatus = "Scanning for games...";

            var games = await _detectionService.ScanForInstalledGamesAsync();
            
            InstalledGames.Clear();
            foreach (var game in games)
            {
                InstalledGames.Add(new GameCardViewModel(game, _launcherService));
            }

            // Start monitoring the detected games
            _monitoringService?.StopMonitoring();
            _monitoringService = new GameMonitoringService(new ObservableCollection<GameInfo>(games));
            _monitoringService.StartMonitoring();

            IsScanning = false;
            ScanStatus = $"Found {InstalledGames.Count} games.";
        }

        private async Task RefreshAsync()
        {
            var gameModels = new ObservableCollection<GameInfo>(InstalledGames.Select(vm => vm.Game));
            await _detectionService.RefreshGameStatesAsync(gameModels);
        }
    }
}
