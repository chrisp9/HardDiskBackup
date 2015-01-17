using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Services
{
    public interface IPersistedOptions 
    {
        BackupDateTime NextBackup;
        IEnumerable<string> BackupDirectories;
    }

    public class PersistedOptions : IPersistedOptions
    {
        public BackupDateTime NextBackup;
        public IEnumerable<string> BackupDirectories;

    }
}
