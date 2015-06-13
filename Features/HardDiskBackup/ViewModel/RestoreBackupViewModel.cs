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

        public RestoreBackupViewModel(
            FormattedExistingBackup backup,
            IBackupFileSystem backupFileSystem)
        {
            FormattedExistingBackup = backup;
            _backupFileSystem = backupFileSystem;

            RestoreBackupCommand = new RelayCommand(
                () =>
                {
                 //   _backupFileSystem.Copy()

                },
                () => true);

        }

    }
}
