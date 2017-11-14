using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using youtube_center.Enums;
using youtube_center.ViewModels.Components;

namespace youtube_center.ViewModels
{
    public class MainWindowViewModel: ViewModelBase
    {
        private ViewModelBase _display;

        // 

        public MainWindowViewModel()
        {
            Display = SimpleIoc.Default.GetInstance<HomeViewModel>();
            MessengerInstance.Register<ComponentView>(this, HandleComponentView);
        }

        // 

        public ViewModelBase Display
        {
            get => _display;
            set => Set(() => Display, ref _display, value);
        }

        // 

        private void HandleComponentView(ComponentView _)
        {
            switch (_)
            {
                case ComponentView.Home:
                    Display = SimpleIoc.Default.GetInstance<HomeViewModel>();
                    break;
                case ComponentView.Manage:
                    Display = SimpleIoc.Default.GetInstance<ManageViewModel>();
                    break;
                case ComponentView.Settings:
                    Display = SimpleIoc.Default.GetInstance<SettingsViewModel>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_), _, null);
            }
        }
    }
}