using Domain;
using HardDiskBackup.View;
using Services.Factories;
using Services.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HardDiskBackup.Commands
{
    public interface IScheduleBackupCommand : ICommand { }

    public class ScheduleBackupCommand : IScheduleBackupCommand
    {
        private bool _canExecute = false;

        private ISetScheduleModel _setScheduleModel;
        private IBackupScheduleService _backupScheduleService;
        private IBackupDirectoryModel _backupDirectoryModel;

        public ScheduleBackupCommand(
            ISetScheduleModel setScheduleModel,
            IBackupScheduleService backupScheduleService,
            IBackupDirectoryModel backupDirectoryModel)
        {
            _setScheduleModel = setScheduleModel;
            _backupScheduleService = backupScheduleService;
            _backupDirectoryModel = backupDirectoryModel;
        }

        public bool CanExecute(object parameter)
        {

            var canExecute = _backupDirectoryModel.BackupDirectories.Count > 0
                && _setScheduleModel.IsScheduleValid();

            return canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            var schedule = _setScheduleModel.CreateSchedule();
            var directories = _backupDirectoryModel.BackupDirectories;

            var backup = Backup.Create(directories, schedule);
            _backupScheduleService.ScheduleNextBackup(backup, () => { var x = new BackupView(); });
        }
    }
}
