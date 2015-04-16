using Domain;
using Registrar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Disk
{
    public interface IExistingBackupsManager
    {
        
    }

    public class ExistingBackupsManager : IExistingBackupsManager
    {
        private IEnumerable<ExistingBackup> _backupRoot;

        public ExistingBackupsManager(IEnumerable<ExistingBackup> backupRoot)
        {
            _backupRoot = backupRoot;
        }
    }
}