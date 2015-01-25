using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;
using Domain;

namespace Services.BackupSchedule
{
    public interface IJsonLayer
    {
        bool FileExists { get; }
        void SerializeToFile(IPersistedOptions toSerialize);
        IPersistedOptions DeserializeFromFile();
    }

    public class JsonLayer : IJsonLayer
    {
        public bool FileExists
        {
            get
            {
                var appData = _environmentWrapper.AppDataPath;
                var directory = Path.Combine(appData, _backupToolDir);

                return (_directoryWrapper.Exists(directory)
                    && _fileWrapper.Exists(Path.Combine(directory, _fileName)));
            }
         }

        private const string _fileName = "settings.json";
        private const string _backupToolDir = "HdBackupTool";
        private readonly IFileWrap _fileWrapper;
        private readonly IDirectoryWrap _directoryWrapper;
        private readonly IEnvironmentWrap _environmentWrapper;

        private string _path
        {
            get
            {
                var appData = _environmentWrapper.AppDataPath;
                return Path.Combine(appData, _backupToolDir);
            }
        }

        public JsonLayer(
            IFileWrap fileWrapper, 
            IDirectoryWrap directoryWrapper,
            IEnvironmentWrap environmentWrapper)
        {
            _fileWrapper = fileWrapper;
            _directoryWrapper = directoryWrapper;
            _environmentWrapper = environmentWrapper;
        }

        public void SerializeToFile(IPersistedOptions toSerialize)
        {
            var serialized = JsonConvert.SerializeObject(toSerialize);

            var backupToolDir = _path;

            if (!_directoryWrapper.Exists(backupToolDir))
                _directoryWrapper.CreateDirectory(backupToolDir);

            var persistenceFile = Path.Combine(backupToolDir, _fileName);

            if (_fileWrapper.Exists(persistenceFile))
                _fileWrapper.Delete(persistenceFile);

            _fileWrapper.Create(persistenceFile);
            _fileWrapper.WriteAllText(persistenceFile, serialized);
        }

        public IPersistedOptions DeserializeFromFile()
        {
            var serialized = _fileWrapper.ReadAllText(_path);
            return JsonConvert.DeserializeObject<PersistedOptions>(serialized);
        }
    }
}
