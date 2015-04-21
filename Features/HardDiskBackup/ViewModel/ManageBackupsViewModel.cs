using Domain;
using GalaSoft.MvvmLight;
using Registrar;
using Services.Disk;
using Services.Factories;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HardDiskBackup.Commands;
using Services.Disk.FileSystem;
using System.Windows.Input;

namespace HardDiskBackup.ViewModel
{
    [Register(LifeTime.SingleInstance)]
    public class ManageBackupsViewModel : ViewModelBase
    {
        public bool DeviceWithBackupsExists {
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

        private bool _deviceWithBackupsExists = false;
        private IExistingBackupsPoller _existingBackupsPoller;
        private IExistingBackupsFactory _existingBackupsFactory;
        private IDeleteBackupCommand _deleteBackupCommand;
        private IExistingBackupsModel _existingBackupsModel;

        public ManageBackupsViewModel(
            IExistingBackupsPoller existingBackupsPoller,
            IExistingBackupsFactory existingBackupsFactory,
            IDeleteBackupCommand deleteBackupCommand,
            IExistingBackupsModel existingBackupsModel)
        {
            _deleteBackupCommand = deleteBackupCommand;
            _existingBackupsPoller = existingBackupsPoller;
            _existingBackupsFactory = existingBackupsFactory;
            _existingBackupsModel = existingBackupsModel;

            _existingBackupsPoller.Subscribe(
                onAddedCallback:   async dir => 
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