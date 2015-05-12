using Domain;
using Registrar;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;

namespace Services.Scheduling
{
    public interface IBackupScheduleService
    {
        BackupDirectoriesAndSchedule NextBackup { get; }

        void ScheduleNextBackup(BackupDirectoriesAndSchedule backup, Action action);

        void TriggerImmediateBackup(IEnumerable<BackupDirectory> backupDirectories);
    }

    /// <summary>
    /// Maintains the Date/Time of the next backup
    /// Schedules an action to occur when the backup is due.
    /// </summary>
    [Register(LifeTime.SingleInstance)]
    public class BackupScheduleService : IBackupScheduleService
    {
        private IScheduler _scheduler;

        public BackupDirectoriesAndSchedule NextBackup { get; private set; }

        public BackupScheduleService(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public void ScheduleNextBackup(BackupDirectoriesAndSchedule backup, Action action)
        {
            var scheduledTime = backup.BackupSchedule.CalculateNextBackupDateTime().DateTime;
            _scheduler.Schedule(scheduledTime.DateTimeInstance, action);
            NextBackup = backup;
        }

        public void TriggerImmediateBackup(IEnumerable<BackupDirectory> backupDirectories)
        {
            throw new NotImplementedException();
        }
    }
}