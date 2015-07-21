using Domain;
using GalaSoft.MvvmLight;
using HardDiskBackup.View;
using Registrar;
using Services;
using Services.Disk;
using Services.Disk.FileSystem;
using Services.Factories;
using Services.Scheduling;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Media;
using SystemWrapper.IO;

namespace HardDiskBackup.ViewModel
{
    [Register(LifeTime.SingleInstance)]
    public class BackupViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public SolidColorBrush LabelColor
        {
            get
            {
                return HasErrors 
                    ? new SolidColorBrush(Colors.Red) 
                    : new SolidColorBrush(Colors.Green);
            }
        }

        public double TotalHeight
        {
            get
            {
                return _totalHeight;
            }
            set
            {
                _totalHeight = value;
                OnPropertyChanged();
            }
        }

        private double _totalHeight;

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

        public bool HasErrors
        {
            get
            {
                return _backupResult != null 
                    && _backupResult.IsFail;
            }
        }

        public string FormattedResult
        {
            get
            {
                return _formattedResult;
            }
            private set
            {
                _formattedResult = value;
                OnPropertyChanged();
            }
        }

        private string _formattedResult;
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
        private ITimestampedBackupRootProvider _timestampedBackupRootProvider;
        private IResultFormatter _resultFormatter;

        private Result _backupResult;

        public BackupViewModel(
            IDriveNotifier driveNotifier,
            IBackupScheduleService backupScheduleService,
            IDirectoryFactory backupDirectoryFactory,
            IBackupFileSystem backupFileSystem,
            ITimestampedBackupRootProvider timestampedBackupRootProvider,
            IResultFormatter resultFormatter)
        {
            _driveNotifier = driveNotifier;
            _backupScheduleService = backupScheduleService;
            _backupDirectoryFactory = backupDirectoryFactory;
            _timestampedBackupRootProvider = timestampedBackupRootProvider;
            _backupFileSystem = backupFileSystem;
            _resultFormatter = resultFormatter;
            TotalHeight = 150;

            ProgressBarIsIndeterminate = true;

            Status = "Waiting for backup device to be plugged in...";

            _driveNotifier.Subscribe(async drive =>
            {
                var rootDirectory = _backupDirectoryFactory.GetBackupRootDirectoryForDrive(drive);
                _backupRootDirectory = rootDirectory;

                var result = await Backup(_backupScheduleService.NextBackup.BackupDirectories);
                _backupResult = result;

                if (result.IsSuccess)
                {
                    Status = "Completed";
                }
                else
                {
                    FormattedResult = _resultFormatter.FormatResult(result);
                    TotalHeight = 300;
                    Status = "Completed with errors";
                    OnPropertyChanged("Height");
                    OnPropertyChanged("HasErrors");
                    OnPropertyChanged("LabelColor");
                }
            });
        }

        // TODO: I'd like to refactor this at some point.
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

            var mirroredDirectory = _timestampedBackupRootProvider
                .CreateTimestampedBackup(_backupRootDirectory);

            var result = Result.Success();

            await Task.Run(async () => 
            {
                foreach(var b in backupDirectories) 
                {
                    var currentResult = await 
                        _backupFileSystem.Copy(
                            b.Directory, 
                            mirroredDirectory.Directory.FullName, 
                            AddToTotal);

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