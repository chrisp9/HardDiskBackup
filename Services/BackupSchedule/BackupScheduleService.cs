using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DiskSchedule
{
    public interface IBackupScheduleService
    {
        void ScheduleNextBackup(BackupDateTime backupTime);
    }

    public class BackupScheduleService
    {
        private IDateTimeProvider _dateTimeProvider;
        private IBackupSettings _persistedOptions;

        public BackupScheduleService(
            IDateTimeProvider dateTimeProvider,
            IBackupSettings persistedOptions)
        {
            _dateTimeProvider = dateTimeProvider;
            _persistedOptions = persistedOptions;
        }

        public void ScheduleNextBackup(BackupDateTime backupTime)
        {
            _persistedOptions.NextBackup = backupTime;
        }
    }
}
