using System;
using System.Collections.Generic;
using WpfApp1.Models;

using WpfApp1.Services.Interfaces;

namespace WpfApp1.Services
{
    public class FakeNewsService : INewsService
    {
        public List<NewsItem> GetLatestNews()
        {
            return new List<NewsItem>
            {
                new NewsItem
                {
                    Title = "Patch Notes 2.4: Volcanic Peaks",
                    Description = "New map, new weapons, and extensive balancing changes for the competitive season.",
                    Date = DateTime.Now.AddDays(-1),
                    Category = "Patch Notes"
                },
                new NewsItem
                {
                    Title = "Double XP Weekend!",
                    Description = "Join us this weekend for double experience points on all match types.",
                    Date = DateTime.Now.AddDays(-3),
                    Category = "Event"
                },
                new NewsItem
                {
                    Title = "Dev Blog: The Future of Combat",
                    Description = "Our leads discuss the upcoming overhaul to the melee combat system.",
                    Date = DateTime.Now.AddDays(-5),
                    Category = "Dev Blog"
                }
            };
        }
    }
}
