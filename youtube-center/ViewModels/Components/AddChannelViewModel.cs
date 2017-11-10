﻿using System;
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

        private static readonly Regex OutlineRegex = new Regex(@"(?:<outline text=""([\w ]+)"" title=""(?:[\w ]+)"" type=""rss"" xmlUrl=""https:\/\/www\.youtube\.com\/feeds\/videos\.xml\?channel_id=([\w\d-]+)"" \/>)");

        //

        public AddChannelViewModel(ISettingsRepository settingsRepository, IYoutubeService youtubeService)
        {
            _settingsRepository = settingsRepository;
            _youtubeService = youtubeService;

            GoBackCommand = new RelayCommand(() => MessengerInstance.Send(ComponentView.Home));
            OpenSubscriptionsCommand = new RelayCommand(() => Process.Start("https://www.youtube.com/subscription_manager"));
            FilebrowseCommand = new RelayCommand(Filebrowse);
            ContextCommand = new RelayCommand<string>(Context);

            AddCommand = new RelayCommand(
                Add, 
                () => Url.Length > 0
            );

            ImportCommand = new RelayCommand(
                Import,
                () => File.Exists(Path)
            );

            Channels = new ObservableCollection<Channel>(_settingsRepository.Channels.OrderBy(c => c.Name));
        }

        //

        public RelayCommand GoBackCommand { get; set; }

        public RelayCommand OpenSubscriptionsCommand { get; set; }

        public RelayCommand FilebrowseCommand { get; set; }
        
        public RelayCommand AddCommand { get; set; }

        public RelayCommand ImportCommand { get; set; }

        public RelayCommand<string> ContextCommand { get; set; }

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
        
        private async void Add()
        {
            // Retrieve details
            var (successful, username, id) = await _youtubeService.FindDetails(Url);

            // Convey an error at some point
            if (!successful)
            {
                return;
            }

            // Only add if it doesn't already exist
            if (_settingsRepository.Channels.Any(c => c.Name == username))
            {
                return;
            }

            // Update model
            var channel = new Channel
            {
                Name = username,
                Id = id
            };
            channel.Videos = new List<Video>(await _youtubeService.RetrieveVideos(channel));
            await _youtubeService.ThumbnailCheck(channel);

            // Add to settings
            _settingsRepository.Channels.Add(channel);
            _settingsRepository.Save();

            // Update listing
            Channels = new ObservableCollection<Channel>(_settingsRepository.Channels.OrderBy(c => c.Name));
            MessengerInstance.Send(Request.Refresh);
            Url = "";
        }

        private async void Import()
        {
            // Load file and parse
            var text = File.ReadAllText(Path);

            foreach (Match match in OutlineRegex.Matches(text))
            {
                // Extract data to a quantified object
                var (name, id) = (match.Groups[1].Value, match.Groups[2].Value);
                var channel = new Channel
                {
                    Name = name,
                    Id = id,
                    Url = $"https://www.youtube.com/channel/{id}"
                };

                // Only add if it doesn't already exist
                if (_settingsRepository.Channels.Any(c => c.Name == name))
                    continue;

                // Get video, thumbnails
                try
                {
                    channel.Videos = new List<Video>(await _youtubeService.RetrieveVideos(channel));
                    await _youtubeService.ThumbnailCheck(channel);
                }

                catch
                {
                    
                }

                // Add to settings and save
                _settingsRepository.Channels.Add(channel);
                _settingsRepository.Save();
            }

            // Save that there was an attempt now to find videos
            _settingsRepository.LastChecked = DateTime.Now;
            _settingsRepository.Save();

            // Update listing
            Channels = new ObservableCollection<Channel>(_settingsRepository.Channels.OrderBy(c => c.Name));
            MessengerInstance.Send(Request.Refresh);
        }

        private void Context(string token)
        {
            switch (token)
            {
                case "remove":
                    var channels = _settingsRepository.Channels;
                    _settingsRepository.Channels.Remove(channels.First(c => c.Id == SelectedChannel.Id));
                    _settingsRepository.Save();
                    Channels.Remove(SelectedChannel);
                    MessengerInstance.Send(Request.Refresh);
                    break;
            }
        }
    }
}