using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WpfApp1.Models;
using WpfApp1.Services;
using WpfApp1.ViewModels.Base;

namespace WpfApp1.ViewModels
{
    public class GameCardViewModel : BaseViewModel
    {
        private readonly LauncherService _launcherService;
        private BitmapImage _bannerImage;

        public GameInfo Game { get; }

        /// <summary>
        /// The game banner as a ready-to-bind BitmapImage.
        /// Handles both file paths and HTTP URLs.
        /// </summary>
        public BitmapImage BannerImage
        {
            get => _bannerImage;
            private set => SetProperty(ref _bannerImage, value);
        }

        public ICommand LaunchCommand { get; }
        public ICommand StopCommand { get; }

        public GameCardViewModel(GameInfo game, LauncherService launcherService)
        {
            Game = game;
            _launcherService = launcherService;

            LaunchCommand = new RelayCommand(async _ => await LaunchAsync(), _ => !Game.IsRunning);
            StopCommand = new RelayCommand(async _ => await StopAsync(), _ => Game.IsRunning);

            LoadBannerImage();
        }

        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient();

        private async void LoadBannerImage()
        {
            var path = Game.BannerPath;
            if (string.IsNullOrWhiteSpace(path))
                return;

            try
            {
                if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    var response = await _httpClient.GetAsync(path);
                    if (response.IsSuccessStatusCode)
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        var memStream = new System.IO.MemoryStream();
                        await stream.CopyToAsync(memStream);
                        memStream.Position = 0;

                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                        bitmap.StreamSource = memStream;
                        bitmap.DecodePixelWidth = 460;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        BannerImage = bitmap;
                    }
                }
                else
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                    bitmap.UriSource = new Uri(path, UriKind.Absolute);
                    bitmap.DecodePixelWidth = 460;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    BannerImage = bitmap;
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("image_errors.txt", $"Error loading {path}: {ex.Message}\n");
                BannerImage = null;
            }
        }

        private async Task LaunchAsync()
        {
            await _launcherService.LaunchGameAsync(Game);
        }

        private async Task StopAsync()
        {
            await _launcherService.StopGameAsync(Game);
        }
    }
}
