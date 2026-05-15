using System;
using WpfApp1.ViewModels.Base;

namespace WpfApp1.Services
{
    public class NavigationService
    {
        public event Action<BaseViewModel> OnNavigationRequested;

        public void NavigateTo(BaseViewModel viewModel)
        {
            OnNavigationRequested?.Invoke(viewModel);
        }
    }
}
