using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using youtube_center.Repositories.Interface;
using youtube_center.Services;
using youtube_center.Services.Interface;
using youtube_center.ViewModels;

namespace youtube_center
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Repositories
            // SimpleIoc.Default.Register<ISettingsRepository>(SettingsRepository.Load);

            // Services
            SimpleIoc.Default.Register<IYoutubeService, YoutubeService>();

            // Viewmodels
            SimpleIoc.Default.Register<MainWindowViewModel>();
        }

        public static MainWindowViewModel Main => ServiceLocator.Current.GetInstance<MainWindowViewModel>();

    }
}