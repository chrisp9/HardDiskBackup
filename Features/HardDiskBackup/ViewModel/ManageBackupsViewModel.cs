using GalaSoft.MvvmLight;
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
        private IExistingBackupsPoller _existingBackupsPoller;

        public ManageBackupsViewModel(IExistingBackupsPoller existingBackupsPoller)
        {
            _existingBackupsPoller = existingBackupsPoller;
            _existingBackupsPoller.Subscribe(x => Console.WriteLine("Added"), x => Console.WriteLine("Removed"));
        }
    }
}
