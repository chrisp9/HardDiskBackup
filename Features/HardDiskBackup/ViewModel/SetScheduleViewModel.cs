using Domain;
using GalaSoft.MvvmLight;
using Services.Factories;

namespace HardDiskBackup.ViewModel
{
    public class SetScheduleViewModel : ViewModelBase
    {
        private IDateTimeProvider _dateTimeProvider;
        private IBackupScheduleFactory _backupScheduleFactory;

        public SetScheduleViewModel(
            IDateTimeProvider dateTimeProvider, 
            IBackupScheduleFactory backupScheduleFactory)
        {
            _dateTimeProvider = dateTimeProvider;
            _backupScheduleFactory = backupScheduleFactory;
        }
    }
}
