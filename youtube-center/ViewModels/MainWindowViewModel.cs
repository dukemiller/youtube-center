using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using youtube_center.ViewModels.Components;

namespace youtube_center.ViewModels
{
    public class MainWindowViewModel: ViewModelBase
    {
        private ViewModelBase _display;

        public MainWindowViewModel()
        {
            Display = SimpleIoc.Default.GetInstance<HomeViewModel>();
        }

        public ViewModelBase Display
        {
            get => _display;
            set => Set(() => Display, ref _display, value);
        }
    }
}