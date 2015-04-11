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

    [Register(LifeTime.Transient)]
    public class ExistingBackupsManager : IExistingBackupsManager
    {
        public ExistingBackupsManager(BackupRootDirectory backupRoot)
        {

        }
    }
}
