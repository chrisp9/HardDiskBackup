using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper;

namespace Domain.BackupSchedule
{
    public class WeeklyBackupSchedule : BackupSchedule
    {
        private DayOfWeek _dayOfWeek;

        public WeeklyBackupSchedule(
            INextBackupDateTimeFactory nextBackupDateTimeFactory,
            IDateTimeProvider provider, 
            DayOfWeek dayOfWeek, 
            BackupTime time) : base(provider, nextBackupDateTimeFactory, time)
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
            int start = (int) from.DayOfWeek;
            int target = (int) _dayOfWeek;

            if (start == target && new BackupTime(from.TimeOfDay) < BackupTime)
            {
                // We're on the same day as the target, but before the scheduled time
                // so we schedule for the current day
                return new BackupDate(from.Year, from.Month, start+1);
            }

            if (target <= start) 
                target += 7;

            var nextWeek = from.AddDays(target - start);
            return new BackupDate(nextWeek.Year, nextWeek.Month, nextWeek.Day);
        }
    }
}
