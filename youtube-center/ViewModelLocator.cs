using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using youtube_center.Repositories;
using youtube_center.Repositories.Interface;
using youtube_center.Services;
using youtube_center.Services.Interface;
using youtube_center.ViewModels;
using youtube_center.ViewModels.Components;

namespace youtube_center
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Repositories
            SimpleIoc.Default.Register<ISettingsRepository>(SettingsRepository.Load);
            SimpleIoc.Default.Register<IChannelRepository>(ChannelRepository.Load);
            SimpleIoc.Default.Register<IVideoRepository>(VideoRepository.Load);

            // Services
            SimpleIoc.Default.Register<IYoutubeService, YoutubeService>();

            // Viewmodels
            SimpleIoc.Default.Register<MainWindowViewModel>();
            SimpleIoc.Default.Register<ManageViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<HomeViewModel>();
        }

        public static MainWindowViewModel Main => ServiceLocator.Current.GetInstance<MainWindowViewModel>();
        public static HomeViewModel Home => ServiceLocator.Current.GetInstance<HomeViewModel>();
        public static ManageViewModel Manage => ServiceLocator.Current.GetInstance<ManageViewModel>();
        public static SettingsViewModel Settings => ServiceLocator.Current.GetInstance<SettingsViewModel>();

    }
}