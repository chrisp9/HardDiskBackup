using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Scheduling
{
    public class DailyBackupSchedule : BackupSchedule
    {
        private BackupTime _timeOfDay;

        public DailyBackupSchedule(
            INextBackupDateTimeFactory nextBackupDateTimeFactory,
            IDateTimeProvider provider,
            BackupTime time)
            : base(provider, nextBackupDateTimeFactory, time) 
        {
            _timeOfDay = time;
        }

        public override NextBackupDateTime CalculateNextBackupDateTime()
        {
            var next = Next(DateTimeProvider.Now);
            return NextBackupDateTimeFactory.Create(next, BackupTime);
        }

        private BackupDate Next(DateTime from)
        {
            var today = from.Date;
            var start = new BackupTime(from.TimeOfDay);
            var target = _timeOfDay;

            return (start < target)
            ? new BackupDate(today)
            : new BackupDate(today.AddDays(1));
        }
    }
}
