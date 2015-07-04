using Domain;
using GalaSoft.MvvmLight;
using Registrar;
using Services.Disk;
using Services.Disk.FileSystem;
using Services.Factories;
using Services.Scheduling;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace HardDiskBackup.ViewModel
{
    [Register(LifeTime.SingleInstance)]
    public class BackupViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public string Status
        {
            get { return _status; }
            private set { _status = value; OnPropertyChanged(); }
        }

        public bool ProgressBarIsIndeterminate
        {
            get { return _progressBarIsIndeterminate; }
            set { _progressBarIsIndeterminate = value; OnPropertyChanged(); }
        }

        public long TotalBytesToCopy
        {
            get { return _totalBytesToCopy; }
            set { _totalBytesToCopy = value; OnPropertyChanged(); }
        }

        public long BytesCopiedSoFar
        {
            get { return _bytesCopiedSoFar; }
            set { _bytesCopiedSoFar = value; OnPropertyChanged(); }
        }

        private object _lock = new object();
        private long _totalBytesToCopy;
        private long _bytesCopiedSoFar;
        private bool _progressBarIsIndeterminate;
        private string _status;

        private IDriveNotifier _driveNotifier;
        private IBackupScheduleService _backupScheduleService;
        private IBackupFileSystem _backupFileSystem;
        private IDirectoryFactory _backupDirectoryFactory;

        private BackupRootDirectory _backupRootDirectory;

        public BackupViewModel(
            IDriveNotifier driveNotifier,
            IBackupScheduleService backupScheduleService,
            IDirectoryFactory backupDirectoryFactory,
            IBackupFileSystem backupFileSystem)
        {
            _driveNotifier = driveNotifier;
            _backupScheduleService = backupScheduleService;
            _backupDirectoryFactory = backupDirectoryFactory;
            _backupFileSystem = backupFileSystem;
            ProgressBarIsIndeterminate = true;

            Status = "Waiting for backup device to be plugged in...";

            _driveNotifier.Subscribe(async drive =>
            {
                var rootDirectory = _backupDirectoryFactory.GetBackupRootDirectoryForDrive(drive);
                _backupRootDirectory = rootDirectory;

                await Backup(_backupScheduleService.NextBackup.BackupDirectories);
            });
        }

        public async Task<Result> Backup(IEnumerable<BackupDirectory> backupDirectories)
        {
            Status = "Calculating size of files to copy...";
            await Task.Run(async () =>
            {
                long size = 0L;
                foreach (var b in backupDirectories)
                {
                    var currentsize = await _backupFileSystem.CalculateTotalSize(b.Directory);
                    size += currentsize.Value;
                }

                TotalBytesToCopy = size;
            });

            ProgressBarIsIndeterminate = false;
            Status = "Copying files...";

            var mirroredDirectory = _backupDirectoryFactory.GetMirroredDirectoryFor(
                _backupRootDirectory.Directory.FullName);

            var result = Result.Success();

            await Task.Run(async () => 
            {
                foreach(var b in backupDirectories) 
                {
                    var currentResult = await 
                        _backupFileSystem.Copy(b.Directory, mirroredDirectory.Directory.FullName, AddToTotal);

                    result = Result.Combine(result, currentResult);
                }
            });

            return result;
        }

        private void AddToTotal(IFileInfoWrap file)
        {
            BytesCopiedSoFar += file.Length;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChangedHandler;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}