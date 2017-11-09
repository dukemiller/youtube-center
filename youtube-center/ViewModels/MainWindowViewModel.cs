using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using youtube_center.Models;
using youtube_center.Services.Interface;

namespace youtube_center.ViewModels
{
    public class MainWindowViewModel: ViewModelBase
    {
        private readonly IYoutubeService _youtubeService;

        public MainWindowViewModel(IYoutubeService youtubeService)
        {
            _youtubeService = youtubeService;
            TestCommand = new RelayCommand(Test);
        }

        public RelayCommand TestCommand { get; set; }

        private async void Test()
        {
            // https://www.youtube.com/subscription_manager
            // https://www.youtube.com/feeds/videos.xml?channel_id=UCtUbO6rBht0daVIOGML3c8w

            var testChannel = new Channel {Id = "UCtUbO6rBht0daVIOGML3c8w"};
            var videos = new List<Video>(await _youtubeService.RetrieveVideos(testChannel));

            Console.WriteLine(videos);
        }

    }
}