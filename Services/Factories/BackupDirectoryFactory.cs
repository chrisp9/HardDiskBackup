using Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Services.Factories
{
    public interface IBackupDirectoryFactory
    {
        BackupDirectory Create(string path);
    }

    public class BackupDirectoryFactory : IBackupDirectoryFactory
    {
        public BackupDirectory Create(string path)
        {
            var directoryInfo = new DirectoryInfoWrap(path);
            return new BackupDirectory(directoryInfo);
        }
    }
}
