using System;

namespace Domain.Scheduling
{
    public class MonthlyBackupSchedule : BackupSchedule
    {
        private int _dayOfMonth;

        public MonthlyBackupSchedule(
            INextBackupDateTimeFactory nextBackupDateTimeFactory,
            IDateTimeProvider provider,
            int dayOfMonth,
            BackupTime time)
            : base(provider, nextBackupDateTimeFactory, time)
        {
            _dayOfMonth = dayOfMonth;
        }

        public override NextBackupDateTime CalculateNextBackupDateTime()
        {
            var next = Next(DateTimeProvider.Now);
            return NextBackupDateTimeFactory.Create(next, BackupTime);
        }

        private BackupDate Next(DateTime from)
        {
            int start = from.Day;
            int target = _dayOfMonth;

            if (start == target && (new BackupTime(from.TimeOfDay) < BackupTime))
            {
                // We're on the same day as the target, but before the scheduled time
                // so we schedule for the current day
                return new BackupDate(from.Year, from.Month, start);
            }

            if (start < target)
            {
                // We're on an earlier day to the target, so specify the requried
                // date, which falls in the current month
                return new BackupDate(from.Year, from.Month, _dayOfMonth);
            }

            // Otherwise we're past the backup day so we need to schedule for the next
            // month.
            var nextMonth = from.AddMonths(1).AddDays(target - start);
            return new BackupDate(nextMonth.Year, nextMonth.Month, nextMonth.Day);
        }
    }
}