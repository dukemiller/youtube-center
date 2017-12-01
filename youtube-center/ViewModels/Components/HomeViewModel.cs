using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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

        private readonly IChannelRepository _channelRepository;

        private readonly IVideoRepository _videoRepository;

        private readonly IYoutubeService _youtubeService;

        private ObservableCollection<Video> _currentVideoPage = new ObservableCollection<Video>();

        private ObservableCollection<Video> _videos = new ObservableCollection<Video>();

        private Video _selectedVideo;

        private int _index;

        private bool _loading;

        private const int VideosPerPage = 6;

        // 

        public HomeViewModel(ISettingsRepository settingsRepository, 
            IChannelRepository channelRepository,
            IVideoRepository videoRepository,
            IYoutubeService youtubeService)
        {
            _settingsRepository = settingsRepository;
            _channelRepository = channelRepository;
            _videoRepository = videoRepository;
            _youtubeService = youtubeService;
            
            ManageCommand = new RelayCommand(() => MessengerInstance.Send(ComponentView.Manage));
            SettingsCommand = new RelayCommand(() => MessengerInstance.Send(ComponentView.Settings));
            ContextCommand = new RelayCommand<string>(Context);
            DoubleClickCommand = new RelayCommand(DoubleClick);
            ChangeVideoPageCommand = new RelayCommand<string>(ChangeVideoPage);
            MessengerInstance.Register<Request>(this, HandleRequest);

            if (!IsInDesignMode)
            {
                LoadVideos();
                Poll();
            }
        }

        // 

        public int VideosIndex { get; set; }

        public ObservableCollection<Video> Videos
        {
            get => _videos;
            set => Set(() => Videos, ref _videos, value);
        }

        public ObservableCollection<Video> CurrentVideoPage
        {
            get => _currentVideoPage;
            set => Set(() => CurrentVideoPage, ref _currentVideoPage, value);
        }

        public Video SelectedVideo
        {
            get => _selectedVideo;
            set => Set(() => SelectedVideo, ref _selectedVideo, value);
        }

        public RelayCommand ManageCommand { get; set; }

        public RelayCommand SettingsCommand { get; set; }

        public RelayCommand DoubleClickCommand { get; set; }

        public RelayCommand<string> ChangeVideoPageCommand { get; set; }

        public RelayCommand<string> ContextCommand { get; set; }

        public string PageProgress => $"{VideosIndex + 1}/{(Videos?.Count - 1) / VideosPerPage + 1 ?? 1}";

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

        private async void Context(string token)
        {
            if (SelectedVideo == null)
                return;

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
                    // cmd.StandardInput.Flush();
                    // cmd.StandardInput.Close();
                    break;

                case "mark":
                    SelectedVideo.Watched ^= true;
                    Videos.Remove(SelectedVideo);
                    CurrentVideoPage.Remove(SelectedVideo);
                    RaisePropertyChanged(nameof(PageProgress));

                    var replacement = Videos
                        .Skip(VideosIndex * 6).Take(7)
                        .FirstOrDefault(v => !CurrentVideoPage.Contains(v));

                    if (replacement != null)
                        CurrentVideoPage.Add(replacement);

                    _videoRepository.Save();
                    break;
            }
        }

        private void DoubleClick()
        {
            switch (_settingsRepository.DoubleClickAction)
            {
                case DoubleClickAction.Youtube:
                    Context("youtube");
                    break;
                case DoubleClickAction.Streamlink:
                    Console.Beep();
                    Context("streamlink");
                    break;
                case DoubleClickAction.CopyToClipboard:
                    Console.Beep();
                    Clipboard.Clear();
                    Clipboard.SetText(SelectedVideo.Url);
                    break;
            }
        }

        private void ChangeVideoPage(string token)
        {
            switch (token)
            {
                case "up":
                    VideosIndex = Math.Max(VideosIndex - 1, 0);
                    CurrentVideoPage = new ObservableCollection<Video>(Videos.Skip(VideosIndex * VideosPerPage).Take(VideosPerPage));
                    break;
                case "down":
                    VideosIndex = Math.Min(VideosIndex + 1, (Videos.Count - 1) / VideosPerPage);
                    CurrentVideoPage = new ObservableCollection<Video>(Videos.Skip(VideosIndex * VideosPerPage).Take(VideosPerPage));
                    break;
            }
            RaisePropertyChanged(nameof(PageProgress));
        }

        private async void LoadVideos()
        {
            await Task.Run(() =>
            {
                Videos = new ObservableCollection<Video>(_channelRepository
                    .Channels
                    .SelectMany(_videoRepository.VideosFor)
                    .Where(video => Index == 1 ? video.Watched : Index == 0 && !video.Watched)
                    .OrderByDescending(video => video.Uploaded)
                    .ThenBy(video => video.Title)
                );

                VideosIndex = 0;
                CurrentVideoPage = new ObservableCollection<Video>(Videos.Skip(VideosIndex * VideosPerPage).Take(VideosPerPage));
                RaisePropertyChanged(nameof(PageProgress));
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
            var wait = DateTime.Now - _channelRepository.LastChecked;

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

            foreach (var channel in _channelRepository.Channels)
            {
                try
                {
                    var videos = new List<Video>();

                    // I have to replace and update the videos, because the viewcount / whatever always changes
                    foreach (var video in await _youtubeService.RetrieveVideos(channel))
                    {
                        var stored = _videoRepository.Videos[channel.Id].FirstOrDefault(v => video.Id == v.Id);
                        bool watched;

                        // This is a new video, mark as unwatched
                        if (stored == null)
                        {
                            if (!anyChanges)
                                anyChanges = true;
                            watched = false;
                        }

                        // Preserve old status
                        else
                            watched = stored.Watched;

                        video.Watched = watched;
                        videos.Add(video);
                    }

                    _videoRepository.Videos[channel.Id] = videos.OrderByDescending(v => v.Uploaded);
                    _videoRepository.Save();
                }

                catch
                {
                }
            }

            if (anyChanges)
            {
                LoadVideos();
                if (_settingsRepository.SoundOnNew)
                    Console.Beep(); // TODO: Replace this with an async sound method later
            }

            _channelRepository.LastChecked = DateTime.Now;
            _channelRepository.Save();
            Loading = false;
        }
    }
}