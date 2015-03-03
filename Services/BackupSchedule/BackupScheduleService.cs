using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper;

namespace Services.DiskSchedule
{
    public interface IBackupScheduleService
    {
        void ScheduleNextBackup(NextBackupDateTime backupTime);
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

        public void ScheduleNextBackup(NextBackupDateTime backupTime)
        {
            _persistedOptions.NextBackup = backupTime;
        }
    }
}
