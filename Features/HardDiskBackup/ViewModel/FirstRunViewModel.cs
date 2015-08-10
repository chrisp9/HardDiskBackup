using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Domain;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using HardDiskBackup.Commands;
using Registrar;
using Services.Disk;
using Services.Factories;
using Services.Persistence;
using Services.Scheduling;

namespace HardDiskBackup.ViewModel
{
    [Register(LifeTime.SingleInstance)]
    public class FirstRunViewModel : ViewModelBase, IDataErrorInfo, IFirstRunViewModel
    {
        public ICommand AddPathCommand { get; private set; }

        public ICommand RemovePathCommand { get; private set; }

        public ICommand ScheduleBackupCommand { get; private set; }

        public BackupDirectoriesAndSchedule Backup { get; private set; }

        public string DirectoryPath { get; set; }

        public SetScheduleViewModel SetScheduleViewModel { get; set; }

        public IBackupDirectoryModel BackupDirectoryModel { get; set; }

        private IDateTimeProvider _dateTimeProvider;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IBackupDirectoryValidator _backupDirectoryValidator;
        private readonly IDirectoryFactory _directoryFactory;
        private readonly ISetScheduleModel _setScheduleModel;

        public FirstRunViewModel( // TODO: Factor out commands.
            IDateTimeProvider dateTimeProvider,
            IJsonSerializer jsonSerializer,
            IBackupDirectoryValidator backupDirectoryValidator,
            IDirectoryFactory backupDirectoryFactory,
            IBackupDirectoryModel backupDirectoryModel,
            ISetScheduleModel setScheduleModel,
            IScheduleBackupCommand scheduleBackupCommand,
            IJsonSerializer jsonSerialiser,
            Func<SetScheduleViewModel> setScheduleViewModel)
        {
            _dateTimeProvider = dateTimeProvider;
            _jsonSerializer = jsonSerializer;

            _backupDirectoryValidator = backupDirectoryValidator;
            _directoryFactory = backupDirectoryFactory;
            _setScheduleModel = setScheduleModel;
            SetScheduleViewModel = setScheduleViewModel();

            ScheduleBackupCommand = scheduleBackupCommand;
            BackupDirectoryModel = backupDirectoryModel;

            AddPathCommand = new RelayCommand(
                () =>
                {
                    var backupDirectory = _directoryFactory.GetBackupDirectoryFor(DirectoryPath);
                    BackupDirectoryModel.Add(backupDirectory);
                },
                () => _backupDirectoryValidator.CanAdd(DirectoryPath) == ValidationResult.Success);

            RemovePathCommand = new RelayCommand<BackupDirectory>(
                (item) => { BackupDirectoryModel.Remove(item); },
                _ => true);

            if (!_jsonSerializer.FileExists) return;

            _setScheduleModel.Load(_jsonSerializer.DeserializeSetScheduleModelFromFile());
            var directories = _jsonSerializer.DeserializeBackupDirectoriesFromFile();
            directories.ToList().ForEach(x => BackupDirectoryModel.Add(x));
            ScheduleBackupCommand.Execute(this);
        }

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                CommandManager.InvalidateRequerySuggested();
                if (columnName != "DirectoryPath")
                    throw new InvalidOperationException("FirstRunViewModel only supports validation for DirectoryPath, but you tried to validate: " + columnName);

                // TODO: Move to validator?
                switch (_backupDirectoryValidator.CanAdd(DirectoryPath))
                {
                    case ValidationResult.InvalidPath:
                        return "This path is not valid";

                    case ValidationResult.PathAlreadyExists:
                        return "You cannot add this path because it is already included in the backup";

                    case ValidationResult.Success:
                        return null;

                    default:
                        throw new ArgumentException("Invalid ValidationResult");
                }
            }
        }
    }
}