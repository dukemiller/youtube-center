﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using youtube_center.Enums;
using youtube_center.Models;
using youtube_center.Repositories.Interface;

namespace youtube_center.ViewModels.Components
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly ISettingsRepository _settingsRepository;
        private ObservableCollection<Video> _videos = new ObservableCollection<Video>();
        private Video _selectedVideo;

        // 

        public HomeViewModel(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;

            // https://www.youtube.com/subscription_manager
            // https://www.youtube.com/feeds/videos.xml?channel_id=UCtUbO6rBht0daVIOGML3c8w

            AddCommand = new RelayCommand(() => MessengerInstance.Send(ComponentView.Add));
            ContextCommand = new RelayCommand<string>(Context);
            MessengerInstance.Register<Request>(this, HandleRequest);
            LoadVideos();
        }

        // 

        public ObservableCollection<Video> Videos
        {
            get => _videos;
            set => Set(() => Videos, ref _videos, value);
        }

        public Video SelectedVideo
        {
            get => _selectedVideo;
            set => Set(() => SelectedVideo, ref _selectedVideo, value);
        }
        
        public RelayCommand AddCommand { get; set; }

        public RelayCommand<string> ContextCommand { get; set; }

        // 

        private void Context(string token)
        {
            switch (token)
            {
                case "youtube":
                    Process.Start(SelectedVideo.Url);
                    break;

                case "streamlink":
                    var info = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };

                    var cmd = new Process {StartInfo = info};
                    cmd.Start();
                    cmd.StandardInput.WriteLine($"streamlink {SelectedVideo.Url} best");
                    cmd.StandardInput.Flush();
                    cmd.StandardInput.Close();
                    break;

                case "mark":
                    break;
            }
        }

        private async void LoadVideos()
        {
            await Task.Run(() =>
            {
                Videos = new ObservableCollection<Video>(
                    _settingsRepository
                        .Channels.SelectMany(channel => channel.Videos)
                        .OrderByDescending(video => video.Uploaded).ThenBy(video => video.Title)
                );
            });
        }

        private void HandleRequest(Request _)
        {
            switch (_)
            {
                case Request.Refresh:
                    LoadVideos();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_), _, null);
            }
        }

    }
}