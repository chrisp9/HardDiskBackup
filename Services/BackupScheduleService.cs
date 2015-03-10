using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IBackupScheduleService
    {
        void ScheduleNextBackup(Backup backup);
        void TriggerImmediateBackup(IEnumerable<BackupDirectory> backupDirectories);
    }

    /// <summary>
    /// Maintains the Date/Time of the next backup
    /// Notifies subscribers when a Backup is due
    /// </summary>
    public class BackupScheduleService
    {
        private IScheduler _scheduler;
        //private Backup _next;

        public BackupScheduleService(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public void ScheduleNextBackup(Backup backup)
        {
            throw new NotImplementedException();
            //_scheduler.Schedule(
             //   backup.BackupSchedule.CalculateNextBackupDateTime().DateTime, 
             //   (_, __) => { back });
        }
    }
}
