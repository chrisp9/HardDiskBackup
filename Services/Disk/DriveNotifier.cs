using Domain;
using Registrar;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Services.Disk
{
    public interface IDriveNotifier
    {
        void Subscribe(Func<IDriveInfoWrap, Task> onNewDisk);

        void Unsubscribe();
    }

    [Register(LifeTime.SingleInstance)]
    public class DriveNotifier : IDriveNotifier
    {
        private IDisposable _subscription;
        private readonly IScheduler _scheduler;
        private readonly IDriveInfoService _diskService;
        private Func<IDriveInfoWrap, Task> _onNewDisk;

        public DriveNotifier(IScheduler scheduler, IDriveInfoService diskService)
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
        public void Subscribe(Func<IDriveInfoWrap, Task> onNewDisk)
        {
            if (_subscription != null)
                throw new InvalidOperationException("Cannot subscribe when subscription already exists. Call Unsubscribe first");

            _onNewDisk = onNewDisk;

            var observer = new DiskObserver(x => { _onNewDisk(x); _subscription.Dispose(); }, _diskService.GetDrives());

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