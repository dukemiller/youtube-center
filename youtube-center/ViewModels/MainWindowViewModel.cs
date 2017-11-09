using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using youtube_center.Models;
using youtube_center.Services.Interface;

namespace youtube_center.ViewModels
{
    public class MainWindowViewModel: ViewModelBase
    {
        private readonly IYoutubeService _youtubeService;
        private ObservableCollection<Video> _videos = new ObservableCollection<Video>();
        private Video _selectedVideo;

        public MainWindowViewModel(IYoutubeService youtubeService)
        {
            _youtubeService = youtubeService;

            // Load videos from _settings later

            TestCommand = new RelayCommand(Test);
        }

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

        public RelayCommand TestCommand { get; set; }

        private async void Test()
        {
            // https://www.youtube.com/subscription_manager
            // https://www.youtube.com/feeds/videos.xml?channel_id=UCtUbO6rBht0daVIOGML3c8w

            var testChannel = new Channel {Id = "UCtUbO6rBht0daVIOGML3c8w"};
            var videos = new List<Video>(await _youtubeService.RetrieveVideos(testChannel));
            Videos = new ObservableCollection<Video>(videos.OrderByDescending(video => video.Uploaded).ThenBy(video => video.Title));
        }

    }
}