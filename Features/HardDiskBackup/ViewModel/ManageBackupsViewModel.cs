﻿using Domain;
using GalaSoft.MvvmLight;
using Registrar;
using Services.Disk;
using Services.Factories;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

        public ExistingBackup[] ExistingBackups { get; set; }

        public ObservableCollection<FormattedExistingBackup> FormattedExistingBackups { get; set; }
      
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
            FormattedExistingBackups = new ObservableCollection<FormattedExistingBackup>();

            _existingBackupsPoller.Subscribe(
                onAddedCallback:   async dir => 
                { 
                    ExistingBackups = await _existingBackupsFactory.Create(dir);
                    
                    ExistingBackups
                        .ToList()
                        .ForEach(x => FormattedExistingBackups.Add(new FormattedExistingBackup(x)));

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