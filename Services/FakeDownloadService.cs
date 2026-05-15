using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using WpfApp1.Models;

using WpfApp1.Services.Interfaces;

namespace WpfApp1.Services
{
    public class FakeDownloadService : IDownloadService
    {
        private readonly DispatcherTimer _timer;
        private readonly Random _random = new Random();

        public ObservableCollection<DownloadItem> Downloads { get; } = new ObservableCollection<DownloadItem>();

        public FakeDownloadService()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;

            // Seed some data
            Downloads.Add(new DownloadItem { FileName = "Game Core Assets", Progress = 45, Status = "Downloading", Speed = "12.5 MB/s" });
            Downloads.Add(new DownloadItem { FileName = "Map Pack: Volcanic Peaks", Progress = 0, Status = "Queued", Speed = "0 MB/s" });

            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach (var download in Downloads)
            {
                if (download.Status == "Downloading")
                {
                    download.Progress += _random.NextDouble() * 2;
                    download.Speed = $"{_random.Next(10, 25)}.{_random.Next(0, 9)} MB/s";

                    if (download.Progress >= 100)
                    {
                        download.Progress = 100;
                        download.Status = "Completed";
                        download.Speed = "0 MB/s";
                    }
                }
            }
        }
    }
}
