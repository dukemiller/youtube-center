using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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

        private readonly IVideoRepository _videoRepository;

        private readonly IYoutubeService _youtubeService;

        private ObservableCollection<Video> _videos = new ObservableCollection<Video>();

        private Video _selectedVideo;

        private int _index;
        private bool _loading;

        // 

        public HomeViewModel(ISettingsRepository settingsRepository, IVideoRepository videoRepository, IYoutubeService youtubeService)
        {
            _settingsRepository = settingsRepository;
            _videoRepository = videoRepository;
            _youtubeService = youtubeService;

            // https://www.youtube.com/subscription_manager
            // https://www.youtube.com/feeds/videos.xml?channel_id=UCtUbO6rBht0daVIOGML3c8w

            ManageCommand = new RelayCommand(() => MessengerInstance.Send(ComponentView.Manage));
            ContextCommand = new RelayCommand<string>(Context);
            MessengerInstance.Register<Request>(this, HandleRequest);

            if (!IsInDesignMode)
            {
                LoadVideos();
                Poll();
            }
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
        
        public RelayCommand ManageCommand { get; set; }

        public RelayCommand<string> ContextCommand { get; set; }

        public bool Loading
        {
            get => _loading;
            set => Set(() => Loading, ref _loading, value);
        }

        public int Index
        {
            get => _index;
            set
            {
                Set(() => Index, ref _index, value);
                LoadVideos();
            }
        }

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
                    SelectedVideo.Watched ^= true;
                    _videoRepository.Save();
                    LoadVideos();
                    break;
            }
        }

        private async void LoadVideos()
        {
            await Task.Run(() =>
            {
                Videos = new ObservableCollection<Video>(
                    _settingsRepository
                        .Channels
                        .SelectMany(_videoRepository.VideosFor)
                        .Where(video => Index == 1 ? video.Watched : Index == 0 && !video.Watched)
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

        private async void Poll()
        {
            var wait = DateTime.Now - _settingsRepository.LastChecked;
            
            if (wait.TotalMinutes >= 5)
                await CheckForNewVideos();
            
            while (true)
            {
                // Update every 5 minutes
                await Task.Delay(60 * 5 * 1000);
                await CheckForNewVideos();
            }
        }

        private async Task CheckForNewVideos()
        {
            Loading = true;
            var anyChanges = false;

            foreach (var channel in _settingsRepository.Channels)
            {
                try
                {
                    // TODO: This is very expensive, fix this
                    var newVideos = new List<Video>(await _youtubeService.RetrieveVideos(channel));
                    newVideos = new List<Video>(newVideos.Where(video =>_videoRepository.Videos[channel.Id].All(v => video.Id != v.Id)));

                    if (!anyChanges && newVideos.Any())
                        anyChanges = true;

                    var combined = _videoRepository.Videos[channel.Id].ToList();
                    combined.AddRange(newVideos);

                    _videoRepository.Videos[channel.Id] = combined.OrderByDescending(v => v.Uploaded);
                    _videoRepository.Save();
                }

                catch
                {

                }
            }

            if (anyChanges)
                LoadVideos();

            _settingsRepository.LastChecked = DateTime.Now;
            _settingsRepository.Save();
            Loading = false;
        }
    }
}