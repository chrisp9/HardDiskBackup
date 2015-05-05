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
using Registrar;
using Services.Scheduling;

namespace Services.Persistence
{
    public interface IJsonSerializer
    {
        bool FileExists { get; }
        void SerializeToFile(SetScheduleModel setScheduleModel, IEnumerable<IDirectoryInfoWrap> directories);
        IBackupSettings DeserializeFromFile();
    }

    [Register(LifeTime.Transient)]
    public class JsonSerializer : IJsonSerializer
    {
        public bool FileExists
        {
            get
            {
                var appData = _environmentWrapper.AppDataPath;
                var directory = Path.Combine(appData, _backupToolDir);

                return (_directoryWrapper.Exists(directory)
                    && _fileWrapper.Exists(Path.Combine(directory, _setScheduleModelFilename)));
            }
         }

        private const string _setScheduleModelFilename = "schedule.json";
        private const string _directoriesFileName = "directories.json";
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

        public JsonSerializer(
            IFileWrap fileWrapper, 
            IDirectoryWrap directoryWrapper,
            IEnvironmentWrap environmentWrapper)
        {
            _fileWrapper = fileWrapper;
            _directoryWrapper = directoryWrapper;
            _environmentWrapper = environmentWrapper;
        }

        public void SerializeToFile(
            ISetScheduleModel setScheduleModel,
            IEnumerable<IDirectoryInfoWrap> directories)
        {
            var serializedSchedule = JsonConvert.SerializeObject(setScheduleModel);
            var serializedDirectories = JsonConvert.SerializeObject(directories.Select(x => x.FullName).ToArray());
            var backupToolDir = _path;

            if (!_directoryWrapper.Exists(backupToolDir))
                _directoryWrapper.CreateDirectory(backupToolDir);

            var scheduleFile = Path.Combine(backupToolDir, _setScheduleModelFilename);
            var directoriesFile = Path.Combine(backupToolDir, _directoriesFileName);

            if (_fileWrapper.Exists(scheduleFile))
                _fileWrapper.Delete(scheduleFile);

            if (_fileWrapper.Exists(directoriesFile))
                _fileWrapper.Delete(directoriesFile);

            _fileWrapper.Create(scheduleFile);
            _fileWrapper.WriteAllText(scheduleFile, serializedSchedule);

            _fileWrapper.Create(directoriesFile);
            _fileWrapper.WriteAllText(scheduleFile, serializedDirectories);
        }

        public IEnumerable<IDirectoryInfoWrap> DeserializeDirectories()
        {
            var serialized = _fileWrapper.ReadAllText(_directoriesFileName);
            var dirs = JsonConvert.DeserializeObject<string>(serialized);

            foreach (var dir in dirs)
            {
                try
                {
                    yield return new DirectoryInfoWrap(new DirectoryInfo(dir));
                }
                catch { }
            }
            
        }

        public IBackupSettings DeserializeFromFile()
        {
            var serialized = _fileWrapper.ReadAllText(_path);
            return JsonConvert.DeserializeObject<BackupSettings>(serialized);
        }
    }
}
