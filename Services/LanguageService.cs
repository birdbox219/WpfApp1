using System;
using System.Linq;
using System.Windows;

namespace WpfApp1.Services
{
    public static class LanguageService
    {
        public static event Action LanguageChanged;
        public static bool IsArabic { get; private set; }

        public static void SetLanguage(bool arabic)
        {
            if (IsArabic == arabic && Application.Current.Resources.MergedDictionaries.Any(d => d.Source?.OriginalString.Contains("Lang.") == true)) 
                return;

            IsArabic = arabic;
            var dict = new ResourceDictionary();
            
            if (arabic)
            {
                dict.Source = new Uri("/Resources/Lang.ar.xaml", UriKind.Relative);
            }
            else
            {
                dict.Source = new Uri("/Resources/Lang.en.xaml", UriKind.Relative);
            }

            var appResources = Application.Current.Resources.MergedDictionaries;
            
            // Remove old language dicts
            var oldDicts = appResources.Where(d => d.Source != null && d.Source.OriginalString.Contains("/Resources/Lang.")).ToList();
            foreach (var d in oldDicts)
            {
                appResources.Remove(d);
            }

            appResources.Add(dict);

            LanguageChanged?.Invoke();
        }

        public static string GetString(string key)
        {
            return Application.Current.TryFindResource(key) as string ?? key;
        }
    }
}
