using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using WpfApp1.Models;
using WpfApp1.ViewModels.Base;

namespace WpfApp1.ViewModels
{
    public class NavigationItem : BaseViewModel
    {
        private bool _isSelected;
        private string _title;
        
        public string Title 
        { 
            get => _title; 
            set => SetProperty(ref _title, value); 
        }
        public string ResourceKey { get; set; }
        public string IconKind { get; set; }
        public bool IsBottom { get; set; }
        public BaseViewModel ViewModel { get; set; }
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }

    public class MainViewModel : BaseViewModel
    {
        private NavigationItem _selectedItem;

        public ObservableCollection<NavigationItem> MenuItems { get; }

        public NavigationItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    if (_selectedItem != null)
                    {
                        foreach (var item in MenuItems)
                        {
                            item.IsSelected = (item == _selectedItem);
                        }
                    }
                    OnPropertyChanged(nameof(CurrentViewModel));
                }
            }
        }

        public BaseViewModel CurrentViewModel => SelectedItem?.ViewModel;

        public MainViewModel(
            HomeViewModel homeVM, 
            NewsViewModel newsVM, 
            InstalledGamesViewModel libraryVM,
            DownloadsViewModel downloadsVM, 
            SettingsViewModel settingsVM)
        {
            MenuItems = new ObservableCollection<NavigationItem>
            {
                new NavigationItem { ResourceKey = "Nav_Home", IconKind = "Home", ViewModel = homeVM, IsBottom = false },
                new NavigationItem { ResourceKey = "Nav_Library", IconKind = "GamepadVariant", ViewModel = libraryVM, IsBottom = false },
                new NavigationItem { ResourceKey = "Nav_News", IconKind = "Newspaper", ViewModel = newsVM, IsBottom = false },
                new NavigationItem { ResourceKey = "Nav_Downloads", IconKind = "Download", ViewModel = downloadsVM, IsBottom = false },
                new NavigationItem { ResourceKey = "Nav_Settings", IconKind = "Settings", ViewModel = settingsVM, IsBottom = true }
            };

            WpfApp1.Services.LanguageService.LanguageChanged += UpdateTitles;
            UpdateTitles();

            SelectedItem = MenuItems.First();
        }

        private void UpdateTitles()
        {
            foreach (var item in MenuItems)
            {
                item.Title = WpfApp1.Services.LanguageService.GetString(item.ResourceKey);
            }
        }
    }
}
