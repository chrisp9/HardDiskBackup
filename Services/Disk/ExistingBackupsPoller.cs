using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Disk
{
    public interface IExistingBackupsPoller
    {
        void Subscribe();
        void Unsubscribe();
    }

    public class ExistingBackupsPoller
    {
        private IDisposable _subscription;
        private IScheduler _scheduler;
        private IDriveInfoService _diskService;

        /// <summary>
        /// Polls for a removable disk containing backups produced with this program
        /// </summary>
        /// <param name="onNewDisk">
        /// The delegate to execute when a new disk is observed
        /// </param>
        public void Subscribe()
        {
            if (_subscription != null)
                throw new InvalidOperationException("Cannot subscribe when subscription already exists. Call Unsubscribe first");

            _subscription = Observable.Interval(TimeSpan.FromSeconds(1), _scheduler)
                .SelectMany(x => _diskService.GetDrives().ToObservable())
                .SelectMany(x => x.RootDirectory.GetDirectories())
                .Where(x => x.FullName.Contains("DiskBackupApp"))
                .ObserveOn(_scheduler)
                .Subscribe();
        }
    }
}
