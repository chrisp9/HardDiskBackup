using Domain;
using Domain.Scheduling;
using Registrar;
using System;

namespace Services.Factories
{
    public interface IBackupScheduleFactory 
    {
        DayOfWeek? DayOfWeek { get; set; }
        int? DayOfMonth { get; set; }

        BackupSchedule Create(BackupScheduleType backupScheduleType, BackupTime backupTime);
    }
     
    [Register(LifeTime.Transient)]
    public class BackupScheduleFactory : IBackupScheduleFactory
    {
        public DayOfWeek? DayOfWeek { get; set; }
        public int? DayOfMonth { get; set; }

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
            BackupTime backupTime)
        {
            switch (backupScheduleType)
            {
                case BackupScheduleType.Weekly:
                    if (!DayOfWeek.HasValue) throw new InvalidOperationException("Cannot instantiate WeeklyBackupSchedule without specifying DayOfWeek");
                    return new WeeklyBackupSchedule(_nextBackupDateTimeFactory, _dateTimeProvider, DayOfWeek.Value, backupTime);
                case BackupScheduleType.Monthly:
                    if (!DayOfMonth.HasValue) throw new InvalidOperationException("Cannot instantiate MonthlyBackupSchedule without specifying DayOfMonth");
                    return new MonthlyBackupSchedule(_nextBackupDateTimeFactory, _dateTimeProvider, DayOfMonth.Value, backupTime);
                case BackupScheduleType.Daily:
                    return new DailyBackupSchedule(_nextBackupDateTimeFactory, _dateTimeProvider, backupTime);
                default:
                    throw new InvalidOperationException("Invalid backupScheduleType was specified. Has a new one been added but BackupScheduleFactory is not aware?");
            }
        }
    }
}