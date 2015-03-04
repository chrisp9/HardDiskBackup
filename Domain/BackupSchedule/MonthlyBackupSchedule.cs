using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper;

namespace Domain.BackupSchedule
{
    public class MonthlyBackupSchedule : BackupSchedule
    {
        private int _dayOfMonth;
        
        public MonthlyBackupSchedule(
            INextBackupDateTimeFactory nextBackupDateTimeFactory,
            IDateTimeProvider provider, 
            int dayOfMonth, 
            BackupTime time) : base(provider, nextBackupDateTimeFactory, time)
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

            return (start <= target)
                ? new BackupDate(new DateTime(from.Year, from.Month, _dayOfMonth))
                : new BackupDate(from.AddMonths(1).AddDays(target - start));
        }
    }
}
