﻿using Domain;
using GalaSoft.MvvmLight;
using HardDiskBackup.Commands;
using Registrar;
using Services.Disk;
using Services.Disk.FileSystem;
using Services.Factories;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace HardDiskBackup.ViewModel
{
    [Register(LifeTime.SingleInstance)]
    public class ManageBackupsViewModel : ViewModelBase
    {
        public bool DeviceWithBackupsExists
        {
            get { return _deviceWithBackupsExists; }
            set
            {
                _deviceWithBackupsExists = value;
                RaisePropertyChanged("DeviceWithBackupsExists");
            }
        }

        public ObservableCollection<FormattedExistingBackup> FormattedExistingBackups
        {
            get
            {
                return _existingBackupsModel.ExistingBackups;
            }
        }

        public ICommand DeleteBackupCommand
        {
            get
            {
                return _deleteBackupCommand;
            }
        }

        public ICommand RestoreBackupCommand
        {
            get
            {
                return _restoreBackupCommand;
            }
        }

        private bool _deviceWithBackupsExists = false;
        private IExistingBackupsPoller _existingBackupsPoller;
        private IExistingBackupsFactory _existingBackupsFactory;
        private IDeleteBackupCommand _deleteBackupCommand;
        private IRestoreBackupCommand _restoreBackupCommand;
        private IExistingBackupsModel _existingBackupsModel;

        public ManageBackupsViewModel(
            IExistingBackupsPoller existingBackupsPoller,
            IExistingBackupsFactory existingBackupsFactory,
            IDeleteBackupCommand deleteBackupCommand,
            IRestoreBackupCommand restoreBackupCommand,
            IExistingBackupsModel existingBackupsModel)
        {
            _deleteBackupCommand = deleteBackupCommand;
            _existingBackupsPoller = existingBackupsPoller;
            _existingBackupsFactory = existingBackupsFactory;
            _existingBackupsModel = existingBackupsModel;
            _restoreBackupCommand = restoreBackupCommand;

            _existingBackupsPoller.Subscribe(
                onAddedCallback: async dir =>
                {
                    var existingBackups = await _existingBackupsFactory.Create(dir);

                    existingBackups.ToList()
                        .ForEach(x => _existingBackupsModel.Add(new FormattedExistingBackup(x)));

                    DeviceWithBackupsExists = true;
                },

                onRemovedCallback: dir =>
                {
                    DeviceWithBackupsExists = false; _existingBackupsModel.Clear();
                }
            );
        }
    }
}