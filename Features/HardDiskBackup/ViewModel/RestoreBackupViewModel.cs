using Domain;
using GalaSoft.MvvmLight.CommandWpf;
using Services.Disk.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardDiskBackup.ViewModel
{
    public class RestoreBackupViewModel
    {
        public FormattedExistingBackup FormattedExistingBackup { get; private set; }
        public RelayCommand RestoreBackupCommand;

        private IBackupFileSystem _backupFileSystem;
        private RestoreToOriginalLocationBackupStrategy _originalLocationStrat;

        public RestoreBackupViewModel(
            FormattedExistingBackup backup,
            IBackupFileSystem backupFileSystem,
            RestoreToOriginalLocationBackupStrategy originalLocationStrat)
        {
            FormattedExistingBackup = backup;
            _backupFileSystem = backupFileSystem;
            _originalLocationStrat = originalLocationStrat;

            RestoreBackupCommand = new RelayCommand(
                async () =>
                {
                    await _originalLocationStrat.Restore(backup.ExistingBackup, (_) => { });

                },
                () => true);

        }

    }
}
