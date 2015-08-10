using System.ComponentModel;
using System.Runtime.CompilerServices;
using Domain;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Registrar;
using Services.Persistence;

namespace HardDiskBackup.ViewModel
{
    [Register(LifeTime.SingleInstance)]
    public class MainWindowViewModel : ViewModelBase
    {
        public bool IsFirstStartup => !_persistedOptions.FileExists;

        public bool PerformBackupSelected
        {
            get
            {
                return _performBackupSelected;
            }
            set
            {
                _performBackupSelected = value;
                OnPropertyChanged();
            }
        }

        private bool _performBackupSelected;
        public FirstRunViewModel FirstRunViewModel { get; private set; }

        public SetScheduleViewModel SetScheduleViewModel { get; private set; }

        public ManageBackupsViewModel ManageBackupsViewModel { get; private set; }

        public BackupViewModel BackupViewModel { get; private set; }

        private IDateTimeProvider _dateTimeProvider;
        private readonly IJsonSerializer _persistedOptions;

        public MainWindowViewModel(
            IDateTimeProvider dateTimeProvider,
            IJsonSerializer jsonSerializer,
            FirstRunViewModel firstRunViewModel,
            BackupViewModel backupViewModel,
            ManageBackupsViewModel manageBackupsViewModel,
            IMessenger messenger)
        {
            _dateTimeProvider = dateTimeProvider;
            _persistedOptions = jsonSerializer;
            FirstRunViewModel = firstRunViewModel;
            BackupViewModel = backupViewModel;
            ManageBackupsViewModel = manageBackupsViewModel;

            messenger.Register<Messages>(this, message =>
            {
                if (message == Messages.PerformBackup)
                    PerformBackupSelected = true;
            });

        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}