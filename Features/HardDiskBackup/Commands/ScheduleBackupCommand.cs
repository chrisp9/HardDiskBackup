using Domain;
using GalaSoft.MvvmLight.Messaging;
using HardDiskBackup.View;
using HardDiskBackup.ViewModel;
using Registrar;
using Services;
using Services.Persistence;
using Services.Scheduling;
using System;
using System.Windows.Input;

namespace HardDiskBackup.Commands
{
    public interface IScheduleBackupCommand : ICommand { }

    [Register(LifeTime.SingleInstance)]
    public class ScheduleBackupCommand : IScheduleBackupCommand
    {
        private ISetScheduleModel _setScheduleModel;
        private IBackupScheduleService _backupScheduleService;
        private IBackupDirectoryModel _backupDirectoryModel;
        private IDispatcher _dispatcher;
        private IJsonSerializer _jsonSerializer;
        private IMessenger _messenger;

        public ScheduleBackupCommand(
            ISetScheduleModel setScheduleModel,
            IBackupScheduleService backupScheduleService,
            IBackupDirectoryModel backupDirectoryModel,
            IDispatcher dispatcher,
            IMessenger messenger,
            IJsonSerializer jsonSerializer)
        {
            _setScheduleModel = setScheduleModel;
            _backupScheduleService = backupScheduleService;
            _backupDirectoryModel = backupDirectoryModel;
            _dispatcher = dispatcher;
            _jsonSerializer = jsonSerializer;
            _messenger = messenger;
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

            _jsonSerializer.SerializeToFile(_setScheduleModel, directories);
            var backup = BackupDirectoriesAndSchedule.Create(directories, schedule);

            _backupScheduleService.ScheduleNextBackup(backup, 
                () => _messenger.Send(Messages.PerformBackup));
        }
    }
}