using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper;

namespace Domain
{
    public class WeeklyBackupSchedule
    {
        private DayOfWeek _dayOfWeek;
        private TimeSpan _time;
        private IDateTimeProvider _dateTimeProvider;

        public WeeklyBackupSchedule(IDateTimeProvider provider, DayOfWeek dayOfWeek, IDateTimeWrap time)
        {
            _dayOfWeek = dayOfWeek;
            _time = time.TimeOfDay;
            _dateTimeProvider = provider;
        }

        public NextBackupDateTime CalculateNextBackupTime()
        {
            var next = Next(_dateTimeProvider.Now, _dayOfWeek);
            var nextBackup = new DateTimeWrap(next.Year, next.Month, next.Day, _time.Hours, _time.Minutes, _time.Seconds);
            return new NextBackupDateTime(nextBackup);
        }

        private static DateTime Next(DateTime from, DayOfWeek dayOfWeek)
        {
            int start = (int)from.DayOfWeek;
            int target = (int)dayOfWeek;
            if (target <= start)
                target += 7;
            return from.AddDays(target - start);
        }

    }
}
