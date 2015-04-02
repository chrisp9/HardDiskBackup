using Domain;
using GalaSoft.MvvmLight;
using Registrar;
using Services.Disk;
using Services.Disk.FileSystem;
using Services.Factories;
using Services.Scheduling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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

        private string _status;

        private IDriveNotifier _driveNotifier;
        private IBackupScheduleService _backupScheduleService;
        private IBackupFileSystem _backupFileSystem;
        private IDirectoryFactory _backupDirectoryFactory;

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

            Status = "Waiting for backup device to be plugged in...";

            _driveNotifier.Subscribe(async drive =>
            {
                var rootDirectory = _backupDirectoryFactory.GetBackupRootDirectoryForDrive(drive);
                _backupFileSystem.Target(rootDirectory);
                await Backup(_backupScheduleService.NextBackup.BackupDirectories);
            });
        }

        public async Task Backup(IEnumerable<BackupDirectory> backupDirectories)
        {
            Status = "Calculating size of files to copy...";
            var x = await Task.Run(() => _backupFileSystem.CalculateTotalSize(backupDirectories));

            Status = "Copying files...";
            await Task.Run(() => _backupFileSystem.Copy(backupDirectories));
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChangedHandler;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
