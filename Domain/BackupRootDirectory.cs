﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Domain
{
    public class BackupRootDirectory : IDirectory
    {
        public IDirectoryInfoWrap Directory { get; private set; }

        public BackupRootDirectory(IDirectoryInfoWrap directory)
        {
            if (directory == null)
                throw new ArgumentNullException("You passed a null directory when instantiating a BackupDirectory");

            Directory = directory;
        }

        public override string ToString()
        {
            return Directory.FullName.ToLower().Replace('/', '\\');
        }
    }
}
