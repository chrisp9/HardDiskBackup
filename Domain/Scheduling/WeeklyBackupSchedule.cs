using System;

namespace Domain.Scheduling
{
    public class WeeklyBackupSchedule : BackupSchedule
    {
        private DayOfWeek _dayOfWeek;

        public WeeklyBackupSchedule(
            INextBackupDateTimeFactory nextBackupDateTimeFactory,
            IDateTimeProvider provider,
            DayOfWeek dayOfWeek,
            BackupTime time)
            : base(provider, nextBackupDateTimeFactory, time)
        {
            _dayOfWeek = dayOfWeek;
        }

        public override NextBackupDateTime CalculateNextBackupDateTime()
        {
            var next = Next(DateTimeProvider.Now);
            return NextBackupDateTimeFactory.Create(next, BackupTime);
        }

        private BackupDate Next(DateTime from)
        {
            // Yes the casts suck, but DayOfWeek is unlikely to change
            int start = (int)from.DayOfWeek;
            int target = (int)_dayOfWeek;

            if (start == target && new BackupTime(from.TimeOfDay) < BackupTime)
            {
                // We're on the same day as the target, but before the scheduled time
                // so we schedule for the current day
                return new BackupDate(from.Year, from.Month, start + 1);
            }

            if (target <= start)
                target += 7;

            var nextWeek = from.AddDays(target - start);
            return new BackupDate(nextWeek.Year, nextWeek.Month, nextWeek.Day);
        }
    }
}