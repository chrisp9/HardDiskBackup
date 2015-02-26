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
            var sanitisedPath = path.Replace('/', '\\');
            var directoryInfo = new DirectoryInfoWrap(sanitisedPath);
            return new BackupDirectory(directoryInfo);
        }
    }
}
