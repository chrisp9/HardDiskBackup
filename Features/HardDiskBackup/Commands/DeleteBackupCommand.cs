using HardDiskBackup.ViewModel;
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
    public interface IDeleteBackupCommand : ICommand { }

    [Register(LifeTime.SingleInstance)]
    public class DeleteBackupCommand : IDeleteBackupCommand
    {
        private IBackupFileSystem _backupFileSystem;

        public DeleteBackupCommand(IBackupFileSystem backupFileSystem)
        {
            _backupFileSystem = backupFileSystem;
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
            _backupFileSystem.Delete(formattedBackup.ExistingBackup);
        }
}
}
