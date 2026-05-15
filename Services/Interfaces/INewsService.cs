using System.Collections.Generic;
using WpfApp1.Models;

namespace WpfApp1.Services.Interfaces
{
    public interface INewsService
    {
        List<NewsItem> GetLatestNews();
    }
}
