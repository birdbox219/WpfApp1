using System.Collections.ObjectModel;
using WpfApp1.Models;

namespace WpfApp1.Services.Interfaces
{
    public interface IDownloadService
    {
        ObservableCollection<DownloadItem> Downloads { get; }
    }
}
