using System.Windows;
using WpfApp1.Services.Interfaces;

namespace WpfApp1.Services
{
    public class DialogService : IDialogService
    {
        public void ShowMessage(string message, string title = "Launcher", MessageBoxImage icon = MessageBoxImage.Information)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, icon);
        }

        public bool ShowConfirmation(string message, string title = "Launcher")
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }
    }
}
