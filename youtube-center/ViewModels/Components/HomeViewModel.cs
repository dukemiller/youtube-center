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

            MessengerInstance.Register<Request>(this, _ =>
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
            });
            
            Videos = new ObservableCollection<Video>(
                _settingsRepository
                    .Channels.SelectMany(channel => channel.Videos)
                    .OrderByDescending(video => video.Uploaded).ThenBy(video => video.Title)
            );

            TestCommand = new RelayCommand(Test);

            AddCommand = new RelayCommand(() => MessengerInstance.Send(ComponentView.Add));
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

        public RelayCommand TestCommand { get; set; }

        public RelayCommand AddCommand { get; set; }

        // 

        private async void Test()
        {
            // https://www.youtube.com/subscription_manager
            // https://www.youtube.com/feeds/videos.xml?channel_id=UCtUbO6rBht0daVIOGML3c8w
            // TODO: check if theres a better way to get every channel later?

            foreach (var channel in _settingsRepository.Channels)
            {
                channel.Videos = new List<Video>(await _youtubeService.RetrieveVideos(channel));
                await _youtubeService.ThumbnailCheck(channel);
            }

            _settingsRepository.LastChecked = DateTime.Now;
            _settingsRepository.Save();

            Videos = new ObservableCollection<Video>(
                _settingsRepository
                    .Channels.SelectMany(channel => channel.Videos)
                    .OrderByDescending(video => video.Uploaded).ThenBy(video => video.Title)
            );
        }
    }
}