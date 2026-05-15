using System.Collections.Generic;
using System.Windows.Input;
using WpfApp1.Models;
using WpfApp1.Services.Interfaces;
using WpfApp1.ViewModels.Base;

namespace WpfApp1.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private GameStatus _status = GameStatus.Ready;
        private string _statusText = "Version 1.0.4 - Up to date";

        public GameStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public List<NewsItem> LatestNews { get; }

        public ICommand PlayCommand { get; }

        public HomeViewModel(INewsService newsService, IDialogService dialogService)
        {
            _dialogService = dialogService;
            LatestNews = newsService.GetLatestNews();
            PlayCommand = new RelayCommand(_ => ExecutePlay());
        }

        private void ExecutePlay()
        {
            Status = GameStatus.Playing;
            StatusText = "Game is running...";
            _dialogService.ShowMessage("Simulating Game Launch!\n\nLaunching 'Volcanic Peaks' engine...");
            
            // Revert status after "launch"
            Status = GameStatus.Ready;
            StatusText = "Version 1.0.4 - Up to date";
        }
    }
}
