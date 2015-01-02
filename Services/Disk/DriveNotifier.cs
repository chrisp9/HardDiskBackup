using Domain;
using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Services.Disk
{
    public interface IDiskNotifier
    {
        void Subscribe(Action<IDriveInfoWrap> onNewDisk);
        void Unsubscribe();
    }

    public class DriveNotifier : IDiskNotifier
    {
        private IDisposable _subscription;
        private IScheduler _scheduler;
        private IDiskService _diskService;
        private Action<IDriveInfoWrap> _onNewDisk;

        public DriveNotifier(IScheduler scheduler, IDiskService diskService)
        {
            _scheduler = scheduler;
            _diskService = diskService;
        }

        /// <summary>
        /// Polls for a new removable disk added to the system.
        /// Only devices added after calling this method are detected
        /// </summary>
        /// <param name="onNewDisk">
        /// The delegate to execute when a new disk is observed
        /// </param>
        public void Subscribe(Action<IDriveInfoWrap> onNewDisk)
        {
            if (_subscription != null)
                throw new InvalidOperationException("Cannot subscribe when subscription already exists. Call Unsubscribe first");

            _onNewDisk = onNewDisk;

            var observer = new DiskObserver(x => _onNewDisk(x), _diskService.GetDrives());

            _subscription = Observable.Interval(TimeSpan.FromSeconds(1), _scheduler)
                .SelectMany(x => _diskService.GetDrives().ToObservable())
                .ObserveOn(_scheduler)
                .Subscribe(observer);
        }

        public void Unsubscribe()
        {
            _subscription.Dispose();
        }
    }
}
