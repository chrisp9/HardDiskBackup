using Domain;
using System.IO;
using SystemWrapper.IO;

namespace Services.Disk
{
    public static class BackupDirectoryFactory
    {
        public static BackupDirectory Create(string path)
        {
            return new BackupDirectory(new DirectoryInfoWrap(new DirectoryInfo(path)));
        }
    }
}
