﻿using GalaSoft.MvvmLight;
using Registrar;
using Services.Disk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private bool _deviceWithBackupsExists;
        private IExistingBackupsPoller _existingBackupsPoller;

        public ManageBackupsViewModel(
            IExistingBackupsPoller existingBackupsPoller,
            IExistingBackupsManager)
        {
            _existingBackupsPoller = existingBackupsPoller;

            _existingBackupsPoller.Subscribe(
                x => Console.WriteLine("Added"), 
                x => Console.WriteLine("Removed"));
        }
    }
}
