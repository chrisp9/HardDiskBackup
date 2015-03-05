using Domain;
using Domain.BackupSchedule;
using System;

namespace Services.Factories
{
    public interface IBackupScheduleFactory 
    {
        BackupSchedule Create(BackupScheduleType backupScheduleType, BackupTime backupTime, int param);
    }
        
    public class BackupScheduleFactory : IBackupScheduleFactory
    {
        private INextBackupDateTimeFactory _nextBackupDateTimeFactory;
        private IDateTimeProvider _dateTimeProvider;

        public BackupScheduleFactory(
            INextBackupDateTimeFactory nextBackupDateTimeFactory,
            IDateTimeProvider dateTimeProvider)
        {
            _nextBackupDateTimeFactory = nextBackupDateTimeFactory;
            _dateTimeProvider = dateTimeProvider;
        }

        public BackupSchedule Create(
            BackupScheduleType backupScheduleType, 
            BackupTime backupTime, 
            int param)
        {
            switch (backupScheduleType)
            {
                case BackupScheduleType.Weekly:
                    return new WeeklyBackupSchedule(_nextBackupDateTimeFactory, _dateTimeProvider, (DayOfWeek) param, backupTime);
                case BackupScheduleType.Monthly:
                    return new MonthlyBackupSchedule(_nextBackupDateTimeFactory, _dateTimeProvider, param, backupTime);
                case BackupScheduleType.Daily:
                    return new DailyBackupSchedule(_nextBackupDateTimeFactory, _dateTimeProvider, backupTime);
                default:
                    throw new InvalidOperationException("Invalid backupScheduleType was specified. Did you add a new one?");
            }
        }
    }
}
