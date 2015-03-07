using Domain;
using GalaSoft.MvvmLight;
using Services.Factories;
using System;

namespace HardDiskBackup.ViewModel
{
    public class SetScheduleViewModel : ViewModelBase
    {
        public int DayOfMonth { get; set; }
        public int DayOfWeek { get; set; } // TODO Binding
        
        public bool IsDaily { get; set; }
        public bool IsWeekly { get; set; }
        public bool IsMonthly { get; set; }

        private IDateTimeProvider _dateTimeProvider;
        private IBackupScheduleFactory _backupScheduleFactory;

        public SetScheduleViewModel(
            IDateTimeProvider dateTimeProvider, 
            IBackupScheduleFactory backupScheduleFactory)
        {
            _dateTimeProvider = dateTimeProvider;
            _backupScheduleFactory = backupScheduleFactory;
        }

        /// <summary>
        /// Returns a BackupSchedule if the requires parameters have
        /// been supplied. Throws an InvalidOperationException if not
        /// </summary>
        public void CreateSchedule()
        {
            
        }
        
    }
}