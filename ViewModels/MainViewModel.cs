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
        public string Title { get; set; }
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
                new NavigationItem { Title = "Home", IconKind = "Home", ViewModel = homeVM, IsBottom = false },
                new NavigationItem { Title = "Library", IconKind = "GamepadVariant", ViewModel = libraryVM, IsBottom = false },
                new NavigationItem { Title = "News", IconKind = "Newspaper", ViewModel = newsVM, IsBottom = false },
                new NavigationItem { Title = "Downloads", IconKind = "Download", ViewModel = downloadsVM, IsBottom = false },
                new NavigationItem { Title = "Settings", IconKind = "Settings", ViewModel = settingsVM, IsBottom = true }
            };

            SelectedItem = MenuItems.First();
        }
    }
}
