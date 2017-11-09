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

        public MainWindowViewModel()
        {
            Display = SimpleIoc.Default.GetInstance<HomeViewModel>();

            MessengerInstance.Register<ComponentView>(this, _ =>
            {
                switch (_)
                {
                    case ComponentView.Home:
                        Display = SimpleIoc.Default.GetInstance<HomeViewModel>();
                        break;
                    case ComponentView.Add:
                        Display = SimpleIoc.Default.GetInstance<AddChannelViewModel>();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(_), _, null);
                }
            });
        }

        public ViewModelBase Display
        {
            get => _display;
            set => Set(() => Display, ref _display, value);
        }
    }
}