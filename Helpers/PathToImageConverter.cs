using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WpfApp1.Helpers
{
    /// <summary>
    /// Converts a string path (file path or URL) into a BitmapImage for WPF Image controls.
    /// Handles both local file paths and HTTP/HTTPS URLs gracefully.
    /// </summary>
    public class PathToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = value as string;
            if (string.IsNullOrWhiteSpace(path))
                return null;

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;

                if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    bitmap.UriSource = new Uri(path, UriKind.Absolute);
                }
                else
                {
                    bitmap.UriSource = new Uri(path, UriKind.Absolute);
                }

                bitmap.DecodePixelWidth = 460; // Limit decode size for performance (2x card width)
                bitmap.EndInit();
                bitmap.Freeze(); // Make cross-thread safe
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
