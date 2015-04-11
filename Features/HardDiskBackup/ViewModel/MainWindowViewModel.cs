using Domain;
using GalaSoft.MvvmLight;
using HardDiskBackup.ViewModel;
using Registrar;
using Services.Persistence;

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

        public FirstRunViewModel FirstRunViewModel { get; private set; }
        public SetScheduleViewModel SetScheduleViewModel { get; private set; }
        public ManageBackupsViewModel ManageBackupsViewModel { get; private set; }

        private IDateTimeProvider _dateTimeProvider;
        private IJsonSerializer _persistedOptions;

        public MainWindowViewModel(
            IDateTimeProvider dateTimeProvider,
            IJsonSerializer jsonSerializer,
            FirstRunViewModel firstRunViewModel,
            ManageBackupsViewModel manageBackupsViewModel)
        {
            _dateTimeProvider = dateTimeProvider;
            _persistedOptions = jsonSerializer;
            FirstRunViewModel = firstRunViewModel;
            ManageBackupsViewModel = manageBackupsViewModel;
        }
    }
}