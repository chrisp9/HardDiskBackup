using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Services.BackupSchedule;

namespace Services
{
    public interface IPersistedOptions 
    {
        BackupDateTime NextBackup { get; set; }
        IEnumerable<string> BackupDirectories { get; set; }
        bool HasPersistedOptions { get; }

        void Persist();
        void ReadExisting();
    }

    public class PersistedOptions : IPersistedOptions
    {
        public BackupDateTime NextBackup { get; set; }
        public IEnumerable<string> BackupDirectories { get; set; }
        public bool HasPersistedOptions { get { return _jsonLayer.FileExists; } }

        private readonly IJsonLayer _jsonLayer;

        public PersistedOptions(IJsonLayer jsonLayer)
        {
            _jsonLayer = jsonLayer;
        }

        public void Persist()
        {
            // meh...
            _jsonLayer.SerializeToFile(this);
        }

        public void ReadExisting()
        {
            var persisted = _jsonLayer.DeserializeFromFile();
        }
    }
}
