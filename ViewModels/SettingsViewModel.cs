using System.Collections.Generic;
using WpfApp1.ViewModels.Base;

namespace WpfApp1.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private bool _isDarkTheme = true;
        private string _installDirectory = "C:\\Games\\VolcanicPeaks";
        private string _selectedResolution = "1920x1080";

        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set => SetProperty(ref _isDarkTheme, value);
        }

        public string InstallDirectory
        {
            get => _installDirectory;
            set => SetProperty(ref _installDirectory, value);
        }

        public List<string> Resolutions { get; } = new List<string>
        {
            "1280x720",
            "1600x900",
            "1920x1080",
            "2560x1440",
            "3840x2160"
        };

        public string SelectedResolution
        {
            get => _selectedResolution;
            set => SetProperty(ref _selectedResolution, value);
        }
    }
}
