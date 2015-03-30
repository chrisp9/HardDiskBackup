using Domain;
using GalaSoft.MvvmLight;
using Registrar;
using Services.Disk;
using Services.Disk.FileSystem;
using Services.Factories;
using Services.Scheduling;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HardDiskBackup.ViewModel
{
    [Register(LifeTime.SingleInstance)]
    public class BackupViewModel : ViewModelBase
    {
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

            _driveNotifier.Subscribe(drive =>
            {
                var rootDirectory = _backupDirectoryFactory.GetBackupRootDirectoryForDrive(drive);
                _backupFileSystem.Target(rootDirectory);

                _backupFileSystem.Copy(
                    backupScheduleService.NextBackup.BackupDirectories);
            });
        }

        public async void Backup(IDriveInfoWrap drive, IEnumerable<BackupDirectory> backupDirectories)
        {
            await Task.Run(() => _backupFileSystem.CalculateTotalSize(backupDirectories));
        }
    }
}
