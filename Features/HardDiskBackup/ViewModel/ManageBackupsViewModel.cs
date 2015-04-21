using Domain;
using GalaSoft.MvvmLight;
using Registrar;
using Services.Disk;
using Services.Factories;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

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

        public ExistingBackup[] ExistingBackups
        {
            get { return _existingBackups; }
            set
            {
                _existingBackups = value;
                RaisePropertyChanged("ExistingBackups");
            }
        }

        public FormattedExistingBackup[] FormattedExistingBackups 
        {
            get { return _formattedExistingBackups; }
            set 
            {
                _formattedExistingBackups = value;
                RaisePropertyChanged("FormattedExistingBackups");
            }
        }

        private FormattedExistingBackup[] _formattedExistingBackups;

        private bool _deviceWithBackupsExists = false;
        private IExistingBackupsPoller _existingBackupsPoller;
        private IExistingBackupsFactory _existingBackupsFactory;
        private ExistingBackup[] _existingBackups;

        public ManageBackupsViewModel(
            IExistingBackupsPoller existingBackupsPoller,
            IExistingBackupsFactory existingBackupsFactory)
        {
            _existingBackupsPoller = existingBackupsPoller;
            _existingBackupsFactory = existingBackupsFactory;

            _existingBackupsPoller.Subscribe(
                onAddedCallback:   async dir => 
                { 
                    ExistingBackups = await _existingBackupsFactory.Create(dir);

                    FormattedExistingBackups = ExistingBackups
                        .Select(x => new FormattedExistingBackup(x))
                        .ToArray();

                    DeviceWithBackupsExists = true;
                },

                onRemovedCallback: dir => 
                {
                    ExistingBackups = null; DeviceWithBackupsExists = false; FormattedExistingBackups = null;
                }
            );
        }
    }
}