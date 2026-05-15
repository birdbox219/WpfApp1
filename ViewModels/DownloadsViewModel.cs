using System.Collections.ObjectModel;
using WpfApp1.Models;
using WpfApp1.Services.Interfaces;
using WpfApp1.ViewModels.Base;

namespace WpfApp1.ViewModels
{
    public class DownloadsViewModel : BaseViewModel
    {
        public ObservableCollection<DownloadItem> Downloads { get; }

        public DownloadsViewModel(IDownloadService downloadService)
        {
            Downloads = downloadService.Downloads;
        }
    }
}
