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
        void SerializeToFile<T>(T toSerialize);
    }

    public class JsonLayer : IJsonLayer
    {
        private const string _fileName = "settings.json";
        private readonly IFileWrap _fileWrapper;
        private readonly IDirectoryWrap _directoryWrapper;
        private readonly IEnvironmentWrap _environmentWrapper;

        public JsonLayer(
            IFileWrap fileWrapper, 
            IDirectoryWrap directoryWrapper,
            IEnvironmentWrap environmentWrapper)
        {
            _fileWrapper = fileWrapper;
            _directoryWrapper = directoryWrapper;
            _environmentWrapper = environmentWrapper;
        }

        public void SerializeToFile<T>(T toSerialize)
        {
            var serialized = JsonConvert.SerializeObject(toSerialize);

            var appData = _environmentWrapper.AppDataPath;

            var backupToolDir = Path.Combine(appData, "HdBackupTool");

            if (!_directoryWrapper.Exists(backupToolDir))
                _directoryWrapper.CreateDirectory(backupToolDir);

            var persistenceFile = Path.Combine(backupToolDir, _fileName);

            if (_fileWrapper.Exists(persistenceFile))
                _fileWrapper.Delete(persistenceFile);

            _fileWrapper.Create(persistenceFile);
            _fileWrapper.WriteAllText(persistenceFile, serialized);
        }

    }
}
