using Domain;
using GalaSoft.MvvmLight;
using HardDiskBackup.View;
using HardDiskBackup.ViewModel;
using Registrar;
using Services;
using Services.Factories;
using Services.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace HardDiskBackup.Commands
{
    public interface IScheduleBackupCommand : ICommand { }

    [Register(LifeTime.SingleInstance)]
    public class ScheduleBackupCommand : IScheduleBackupCommand
    {
        private ISetScheduleModel _setScheduleModel;
        private IBackupScheduleService _backupScheduleService;
        private IBackupDirectoryModel _backupDirectoryModel;
        private IWindowPresenter<BackupViewModel, IBackupView> _backupViewPresenter;
        private IDispatcher _dispatcher;

        public ScheduleBackupCommand(
            ISetScheduleModel setScheduleModel,
            IBackupScheduleService backupScheduleService,
            IBackupDirectoryModel backupDirectoryModel,
            IWindowPresenter<BackupViewModel, IBackupView> backupViewPresenter,
            IDispatcher dispatcher
            )
        {
            _setScheduleModel = setScheduleModel;
            _backupScheduleService = backupScheduleService;
            _backupDirectoryModel = backupDirectoryModel;
            _backupViewPresenter = backupViewPresenter;
            _dispatcher = dispatcher;
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
            _backupScheduleService.ScheduleNextBackup(backup, () => 
            {
                _dispatcher.InvokeAsync(() => 
                {
                    var window = _backupViewPresenter.Present();

                    if(window != null)
                        window.Show();
                });
            });
        }
    }
}