using Domain;
using Services.BackupSchedule;
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
        private IPersistedOptions _persistedOptions;

        public BackupScheduleService(
            IDateTimeProvider dateTimeProvider,
            IPersistedOptions persistedOptions)
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
