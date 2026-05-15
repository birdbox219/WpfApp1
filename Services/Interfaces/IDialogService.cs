using System.Windows;

namespace WpfApp1.Services.Interfaces
{
    public interface IDialogService
    {
        void ShowMessage(string message, string title = "Launcher", MessageBoxImage icon = MessageBoxImage.Information);
        bool ShowConfirmation(string message, string title = "Launcher");
    }
}
