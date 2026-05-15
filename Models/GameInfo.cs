using System;
using WpfApp1.ViewModels.Base;

namespace WpfApp1.Models
{
    public class GameInfo : BaseViewModel
    {
        private string _name;
        private string _installPath;
        private string _executablePath;
        private string _iconPath;
        private string _bannerPath;
        private GamePlatform _platform;
        private string _version;
        private bool _isInstalled;
        private bool _isRunning;
        private DateTime? _lastPlayed;
        private long _sizeOnDisk;
        private GameState _currentState;
        private int? _processId;
        private string _launchArguments;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string InstallPath
        {
            get => _installPath;
            set => SetProperty(ref _installPath, value);
        }

        public string ExecutablePath
        {
            get => _executablePath;
            set => SetProperty(ref _executablePath, value);
        }

        public string IconPath
        {
            get => _iconPath;
            set => SetProperty(ref _iconPath, value);
        }

        public string BannerPath
        {
            get => _bannerPath;
            set => SetProperty(ref _bannerPath, value);
        }

        public GamePlatform Platform
        {
            get => _platform;
            set => SetProperty(ref _platform, value);
        }

        public string Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
        }

        public bool IsInstalled
        {
            get => _isInstalled;
            set => SetProperty(ref _isInstalled, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public DateTime? LastPlayed
        {
            get => _lastPlayed;
            set => SetProperty(ref _lastPlayed, value);
        }

        public long SizeOnDisk
        {
            get => _sizeOnDisk;
            set => SetProperty(ref _sizeOnDisk, value);
        }

        public GameState CurrentState
        {
            get => _currentState;
            set => SetProperty(ref _currentState, value);
        }

        public int? ProcessId
        {
            get => _processId;
            set => SetProperty(ref _processId, value);
        }

        public string LaunchArguments
        {
            get => _launchArguments;
            set => SetProperty(ref _launchArguments, value);
        }
    }
}
