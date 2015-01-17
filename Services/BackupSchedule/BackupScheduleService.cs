using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DiskSchedule
{
    public class BackupScheduleService
    {
        private IDateTimeProvider _dateTimeProvider;

        public BackupScheduleService(
            IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;

        }

        public void ScheduleNextBackup(DateTime backupTime)
        {

        }
    }
}
