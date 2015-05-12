using Domain;
using System.Collections.Generic;

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