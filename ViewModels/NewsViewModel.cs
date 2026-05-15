using System.Collections.Generic;
using WpfApp1.Models;
using WpfApp1.Services.Interfaces;
using WpfApp1.ViewModels.Base;

namespace WpfApp1.ViewModels
{
    public class NewsViewModel : BaseViewModel
    {
        public List<NewsItem> AllNews { get; }

        public NewsViewModel(INewsService newsService)
        {
            AllNews = newsService.GetLatestNews();
        }
    }
}
