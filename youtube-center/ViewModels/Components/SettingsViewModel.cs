using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using youtube_center.Enums;
using youtube_center.Repositories.Interface;

namespace youtube_center.ViewModels.Components
{
    public class SettingsViewModel: ViewModelBase
    {
        private readonly ISettingsRepository _settingsRepository;

        private DoubleClickAction _clickAction;

        private bool _sound;

        // 

        public SettingsViewModel(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
            _clickAction = _settingsRepository.DoubleClickAction;
            _sound = _settingsRepository.SoundOnNew;

            GoBackCommand = new RelayCommand(() => MessengerInstance.Send(ComponentView.Home));
        }

        // 

        public RelayCommand GoBackCommand { get; set; }

        public ObservableCollection<DoubleClickAction> ClickActions { get; set; } = new ObservableCollection<DoubleClickAction>(Classes.Extensions.GetValues<DoubleClickAction>());

        public DoubleClickAction ClickAction
        {
            get => _clickAction;
            set
            {
                Set(() => ClickAction, ref _clickAction, value);
                _settingsRepository.DoubleClickAction = value;
                _settingsRepository.Save();
            }
        }

        public bool Sound
        {
            get => _sound;
            set
            {
                Set(() => Sound, ref _sound, value);
                _settingsRepository.SoundOnNew = value;
                _settingsRepository.Save();
            }
        }
    }
}