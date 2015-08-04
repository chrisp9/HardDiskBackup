using Domain;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HardDiskBackup.ViewModel;
using Registrar;
using Services.Persistence;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HardDiskBackup
{
    [Register(LifeTime.SingleInstance)]
    public class MainWindowViewModel : ViewModelBase
    {
        public bool IsFirstStartup
        {
            get
            {
                return !_persistedOptions.FileExists;
            }
        }

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
        private IJsonSerializer _persistedOptions;
        private IMessenger _messenger;

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
            _messenger = messenger;

            _messenger.Register<Messages>(this, message =>
            {
                if (message == Messages.PerformBackup)
                    PerformBackupSelected = true;
            });

        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChangedHandler;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}