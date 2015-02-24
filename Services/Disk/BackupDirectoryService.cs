using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using SystemWrapper.IO;
using SystemWrapper;
using Services.Factories;

namespace Services.Disk
{
    public interface IBackupDirectoryService
    {
        BackupDirectory GetDirectoryFor(string path);
    }

    public class BackupDirectoryService : IBackupDirectoryService
    {
        private IDirectoryWrap _directoryWrap;
        private IBackupDirectoryFactory _backupDirectoryFactory;

        public BackupDirectoryService(
            IDirectoryWrap directoryWrap,
            IBackupDirectoryFactory factory)
        {
            if (directoryWrap == null)
                throw new ArgumentNullException("The directoryWrap reference you passed in was null");

            _directoryWrap = directoryWrap;
            _backupDirectoryFactory = factory;
        }

        public BackupDirectory GetDirectoryFor(string path)
        {
            return _backupDirectoryFactory.Create(path);
        }
    }
}
