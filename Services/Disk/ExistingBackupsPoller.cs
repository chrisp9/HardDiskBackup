using Domain;
using Registrar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Services.Disk
{
    public interface IExistingBackupsPoller
    {
        void Subscribe(Action<BackupRootDirectory> onAddedCallback, Action<BackupRootDirectory> onRemovedCallback);
        void Unsubscribe();
    }

    [Register(LifeTime.SingleInstance)]
    public class ExistingBackupsPoller : IExistingBackupsPoller
    {
        private IDisposable _subscription;
        private IScheduler _scheduler;
        private IDriveInfoService _diskService;
        private IDirectoryInfoWrap _lastObservedDirectory;

        public ExistingBackupsPoller(IScheduler scheduler, IDriveInfoService driveInfoService)
        {
            _scheduler = scheduler;
            _diskService = driveInfoService;
        }

        public void Subscribe(Action<BackupRootDirectory> onAddedCallback, Action<BackupRootDirectory> onRemovedCallback)
        {
            if (_subscription != null)
                throw new InvalidOperationException("Cannot subscribe when subscription already exists. Call Unsubscribe first");

            var directoryObserver = Observer.Create<long>(_ => 
                { 
                    var directory = _diskService.GetDrives()
                        .SelectMany(x => x.RootDirectory.GetDirectories())
                        .FirstOrDefault(x => x.FullName.ToLower().Contains("diskbackupapp"));

                    // The directory does not exist this time, but did exist last time -> Removed drive
                    if(directory == null && _lastObservedDirectory != null)
                        onRemovedCallback(new BackupRootDirectory(_lastObservedDirectory));

                    // The directory exists now but did not exist last time --> Added drive
                    if(directory != null && _lastObservedDirectory == null)
                        onAddedCallback(new BackupRootDirectory(directory));

                    _lastObservedDirectory = directory;
                });

            // Poll every sec.
            _subscription = Observable.Interval(TimeSpan.FromSeconds(1), _scheduler).Subscribe(directoryObserver);
        }

        public void Unsubscribe()
        {
            _subscription.Dispose();
        }
    }
}
