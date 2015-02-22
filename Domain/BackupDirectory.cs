using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Domain
{
    public class BackupDirectory
    {
        public IDirectoryInfoWrap Directory { get; private set; }

        public BackupDirectory(IDirectoryInfoWrap directory)
        {
            if (directory == null)
                throw new ArgumentNullException("You passed a null directory when instantiating a BackupDirectory");

            if (!directory.Exists)
                throw new ArgumentException("You instantiated BackupDirectory with a directory which doesn't exist");

            Directory = directory;
        }

        public override string ToString()
        {
            return Directory.ToString();
        }
    }
}
