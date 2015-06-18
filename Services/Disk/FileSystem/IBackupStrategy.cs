﻿using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Services.Disk.FileSystem
{
    public interface IBackupStrategy
    {
        Task Restore(ExistingBackup existingBackup, 
            Action<IFileInfoWrap> onFileopy);
    }
}
