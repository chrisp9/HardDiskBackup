using Domain;
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
        event Services.Disk.ExistingBackupsPoller.OnDirectoryAddedEventHandler OnDirectoryAdded;
        event Services.Disk.ExistingBackupsPoller.OnDirectoryRemovedEventHandler OnDirectoryRemoved;
        void Subscribe();
        void Unsubscribe();
    }

    public class ExistingBackupsPoller : IExistingBackupsPoller
    {
        public delegate void OnDirectoryAddedEventHandler(object e, BackupRootEventArgs args);
        public event OnDirectoryAddedEventHandler OnDirectoryAdded;

        public delegate void OnDirectoryRemovedEventHandler(object e, BackupRootEventArgs args);
        public event OnDirectoryRemovedEventHandler OnDirectoryRemoved;

        private IDisposable _subscription;
        private IScheduler _scheduler;
        private IDriveInfoService _diskService;
        private IDirectoryInfoWrap _lastObservedDirectory;


        public ExistingBackupsPoller(IScheduler scheduler, IDriveInfoService driveInfoService)
        {
            _scheduler = scheduler;
            _diskService = driveInfoService;
        }

        public void Subscribe()
        {
            if (_subscription != null)
                throw new InvalidOperationException("Cannot subscribe when subscription already exists. Call Unsubscribe first");

            var directoryObserver = Observer.Create<IDirectoryInfoWrap>(
                (o) => 
                { 
                    if(o == null && _lastObservedDirectory != null) 
                    {
                        var handler = OnDirectoryAdded;
                        if (handler != null)
                            handler(this, new BackupRootEventArgs(new BackupRootDirectory(o)));
                    }

                    if(o != null && _lastObservedDirectory == null) 
                    {
                        var handler = OnDirectoryRemoved;
                        if (handler != null)
                            handler(this, new BackupRootEventArgs(new BackupRootDirectory(_lastObservedDirectory)));
                    }

                    _lastObservedDirectory = o;
                });
                
            _subscription = Observable.Interval(TimeSpan.FromSeconds(1), _scheduler)
                .SelectMany(x => _diskService.GetDrives().ToObservable())
                .SelectMany(x => x.RootDirectory.GetDirectories())
                .Where(x => x.FullName.Contains("DiskBackupApp"))
                .ObserveOn(_scheduler)
                .Subscribe();
        }

        public void Unsubscribe()
        {
            _subscription.Dispose();
        }
    }
}
