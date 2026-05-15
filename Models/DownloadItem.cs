using WpfApp1.ViewModels.Base;

namespace WpfApp1.Models
{
    public class DownloadItem : BaseViewModel
    {
        private double _progress;
        private string _status;
        private string _speed;

        public string FileName { get; set; }

        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public string Speed
        {
            get => _speed;
            set => SetProperty(ref _speed, value);
        }
    }
}
