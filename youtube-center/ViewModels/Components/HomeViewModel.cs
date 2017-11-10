using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using youtube_center.Enums;
using youtube_center.Models;
using youtube_center.Repositories.Interface;
using youtube_center.Services.Interface;

namespace youtube_center.ViewModels.Components
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IYoutubeService _youtubeService;
        private ObservableCollection<Video> _videos = new ObservableCollection<Video>();
        private Video _selectedVideo;

        // 

        public HomeViewModel(ISettingsRepository settingsRepository, IYoutubeService youtubeService)
        {
            _settingsRepository = settingsRepository;
            _youtubeService = youtubeService;

            // https://www.youtube.com/subscription_manager
            // https://www.youtube.com/feeds/videos.xml?channel_id=UCtUbO6rBht0daVIOGML3c8w

            AddCommand = new RelayCommand(() => MessengerInstance.Send(ComponentView.Add));
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

        // 

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
                    Videos = new ObservableCollection<Video>(
                        _settingsRepository
                            .Channels.SelectMany(channel => channel.Videos)
                            .OrderByDescending(video => video.Uploaded).ThenBy(video => video.Title)
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_), _, null);
            }
        }

    }
}