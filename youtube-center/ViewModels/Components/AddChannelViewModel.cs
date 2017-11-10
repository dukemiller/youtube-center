using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using MahApps.Metro.Controls.Dialogs;
using youtube_center.Enums;
using youtube_center.Models;
using youtube_center.Repositories.Interface;
using youtube_center.Services.Interface;

namespace youtube_center.ViewModels.Components
{
    public class AddChannelViewModel : ViewModelBase
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IYoutubeService _youtubeService;
        private string _path;
        private ObservableCollection<Channel> _channels;
        private string _url = string.Empty;
        private Channel _selectedChannel;

        //

        public AddChannelViewModel(ISettingsRepository settingsRepository, IYoutubeService youtubeService)
        {
            _settingsRepository = settingsRepository;
            _youtubeService = youtubeService;

            GoBackCommand = new RelayCommand(() => MessengerInstance.Send(ComponentView.Home));
            OpenSubscriptionsCommand = new RelayCommand(() => Process.Start("https://www.youtube.com/subscription_manager"));
            FilebrowseCommand = new RelayCommand(Filebrowse);

            Channels = new ObservableCollection<Channel>(_settingsRepository.Channels.OrderBy(c => c.Name));
        }

        //

        public RelayCommand GoBackCommand { get; set; }

        public RelayCommand OpenSubscriptionsCommand { get; set; }

        public RelayCommand FilebrowseCommand { get; set; }
        

        public string Url
        {
            get => _url;
            set
            {
                Set(() => Url, ref _url, value);
                AddCommand.RaiseCanExecuteChanged();
            }
        }

        public string Path
        {
            get => _path;
            set
            {
                Set(() => Path, ref _path, value);
                ImportCommand.RaiseCanExecuteChanged();
            }
        }
        
        public ObservableCollection<Channel> Channels
        {
            get => _channels;
            set => Set(() => Channels, ref _channels, value);
        }

        public Channel SelectedChannel
        {
            get => _selectedChannel;
            set => Set(() => SelectedChannel, ref _selectedChannel, value);
        }

        // 

        // https://stackoverflow.com/questions/10315188/open-file-dialog-and-select-a-file-using-wpf-controls-and-c-sharp
        private void Filebrowse()
        {
            // Create OpenFileDialog 
            var dlg = new Microsoft.Win32.OpenFileDialog {FileName = "subscription_manager"};
            var result = dlg.ShowDialog();
            if (result == true)
                Path = dlg.FileName;
        }
        
    }
}