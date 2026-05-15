using System;
using System.Globalization;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;
using WpfApp1.Models;

namespace WpfApp1.Helpers
{
    public class PlatformToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GamePlatform platform)
            {
                switch (platform)
                {
                    case GamePlatform.Steam:
                        return PackIconKind.Steam;
                    case GamePlatform.EpicGames:
                        return PackIconKind.GamepadVariant;
                    case GamePlatform.Riot:
                        return PackIconKind.ShieldOutline;
                    case GamePlatform.BattleNet:
                        return PackIconKind.Flame;
                    case GamePlatform.Standalone:
                        return PackIconKind.Application;
                    default:
                        return PackIconKind.HelpCircle;
                }
            }
            return PackIconKind.HelpCircle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
