using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WpfApp1.Services;
using WpfApp1.Services.Interfaces;
using WpfApp1.ViewModels;
using WpfApp1.Views;

namespace WpfApp1
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Services
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<INewsService, FakeNewsService>();
            services.AddSingleton<IDownloadService, FakeDownloadService>();
            
            // Phase Two Detection Services
            services.AddSingleton<SteamDetectionService>();
            services.AddSingleton<RiotDetectionService>();
            services.AddSingleton<EpicDetectionService>();
            services.AddSingleton<BattleNetDetectionService>();
            services.AddSingleton<GameDetectionService>();
            services.AddSingleton<LauncherService>();

            // ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<NewsViewModel>();
            services.AddSingleton<DownloadsViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<InstalledGamesViewModel>();

            // Views
            services.AddSingleton<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = _serviceProvider.GetRequiredService<MainViewModel>();
            mainWindow.Show();
        }
    }
}
