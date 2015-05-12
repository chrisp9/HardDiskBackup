using Domain;
using Registrar;
using Services.Disk.FileSystem;
using System;
using System.Windows.Input;

namespace HardDiskBackup.Commands
{
    public interface IDeleteBackupCommand : ICommand { }

    [Register(LifeTime.SingleInstance)]
    public class DeleteBackupCommand : IDeleteBackupCommand
    {
        private IBackupFileSystem _backupFileSystem;
        private IExistingBackupsModel _existingBackupsmodel;

        public DeleteBackupCommand(
            IBackupFileSystem backupFileSystem,
            IExistingBackupsModel existingBackupsModel)
        {
            _backupFileSystem = backupFileSystem;
            _existingBackupsmodel = existingBackupsModel;
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
            formattedBackup.DeleteIsInProgress = true;

            _backupFileSystem.Delete(formattedBackup.ExistingBackup, () => _existingBackupsmodel.Remove(formattedBackup));
        }
    }
}