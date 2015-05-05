using Domain;
using GalaSoft.MvvmLight;
using HardDiskBackup.View;
using HardDiskBackup.ViewModel;
using Newtonsoft.Json;
using Registrar;
using Services;
using Services.Factories;
using Services.Persistence;
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
        private IJsonSerializer _jsonSerializer;

        public ScheduleBackupCommand(
            ISetScheduleModel setScheduleModel,
            IBackupScheduleService backupScheduleService,
            IBackupDirectoryModel backupDirectoryModel,
            IWindowPresenter<BackupViewModel, IBackupView> backupViewPresenter,
            IDispatcher dispatcher,
            IJsonSerializer jsonSerializer)
        {
            _setScheduleModel = setScheduleModel;
            _backupScheduleService = backupScheduleService;
            _backupDirectoryModel = backupDirectoryModel;
            _backupViewPresenter = backupViewPresenter;
            _dispatcher = dispatcher;
            _jsonSerializer = jsonSerializer;
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

            _jsonSerializer.SerializeToFile(schedule, directories);
            
            _backupScheduleService.ScheduleNextBackup(backup, () => 
            {
                _dispatcher.InvokeAsync(() => 
                {
                    var window = _backupViewPresenter.Present();

                    // Null check needed for tests. Not ideal but does the job.
                    if(window != null) 
                        window.Show();
                });
            });
        }
    }
}