using Domain;
using Registrar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

namespace Services.Scheduling
{
    public interface IBackupScheduleService
    {
        void ScheduleNextBackup(Backup backup, Action action);
        void TriggerImmediateBackup(IEnumerable<BackupDirectory> backupDirectories);
    }

    /// <summary>
    /// Maintains the Date/Time of the next backup
    /// Schedules an action to occur when the backup is due.
    /// </summary>
    [Register(Scope.InstancePerDependancy)]
    public class BackupScheduleService : IBackupScheduleService
    {
        private IScheduler _scheduler;

        public BackupScheduleService(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public void ScheduleNextBackup(Backup backup, Action action)
        {
            var scheduledTime = backup.BackupSchedule.CalculateNextBackupDateTime().DateTime;
            _scheduler.Schedule(scheduledTime.DateTimeInstance, action);
        }

        public void TriggerImmediateBackup(IEnumerable<BackupDirectory> backupDirectories)
        {
            throw new NotImplementedException();
        }
    }
}
