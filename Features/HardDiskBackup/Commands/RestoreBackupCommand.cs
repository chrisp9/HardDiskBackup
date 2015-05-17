using Domain;
using Registrar;
using Services.Disk.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HardDiskBackup.Commands
{
    public interface IRestoreBackupCommand : ICommand { }

    [Register(LifeTime.SingleInstance)]
    public class RestoreBackupCommand : IRestoreBackupCommand
    {
        private IBackupFileSystem _backupFileSystem;
        private IExistingBackupsModel _existingBackupsModel;

        public RestoreBackupCommand(
                IBackupFileSystem backupFileSystem,
                IExistingBackupsModel existingBackupsModel)
        {
            _backupFileSystem = backupFileSystem;
            _existingBackupsModel = existingBackupsModel;
        }

        public bool CanExecute(object parameter) 
        {
            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            var formattedBackup = parameter as FormattedExistingBackup;

        }
    }
}
