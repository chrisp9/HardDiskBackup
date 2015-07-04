using Domain;
using Registrar;
using Services.Disk.FileSystem;
using System;
using System.Linq;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using HardDiskBackup.View;
using Services;

namespace HardDiskBackup.Commands
{
    public interface IDeleteBackupCommand : ICommand { }

    [Register(LifeTime.SingleInstance)]
    public class DeleteBackupCommand : IDeleteBackupCommand
    {
        private IBackupFileSystem _backupFileSystem;
        private IExistingBackupsModel _existingBackupsmodel;
        private IDialogService _dialogService;

        public DeleteBackupCommand(
            IBackupFileSystem backupFileSystem,
            IExistingBackupsModel existingBackupsModel,
            IDialogService dialogService)
        {
            _backupFileSystem = backupFileSystem;
            _existingBackupsmodel = existingBackupsModel;
            _dialogService = dialogService;
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

        public async void Execute(object parameter)
        {
            var dialogResult = await _dialogService.PresentDialog<MainWindow>("Are you sure?", "Once deleted, backups cannot be restored");
            if (dialogResult == MessageDialogResult.Negative)
                return;

            var formattedBackup = parameter as FormattedExistingBackup;
            formattedBackup.DeleteIsInProgress = true;

            await _backupFileSystem.Delete(
                formattedBackup.ExistingBackup.BackupDirectory.Directory, 
                () => _existingBackupsmodel.Remove(formattedBackup));
        }
    }
}