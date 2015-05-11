using Domain;
using Newtonsoft.Json;
using Registrar;
using Services.Scheduling;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SystemWrapper.IO;

namespace Services.Persistence
{
    public interface IJsonSerializer
    {
        bool FileExists { get; }
        void SerializeToFile(ISetScheduleModel setScheduleModel, IEnumerable<BackupDirectory> directories);
        ISetScheduleModel DeserializeSetScheduleModelFromFile();
        IEnumerable<BackupDirectory> DeserializeBackupDirectoriesFromFile();
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
                    && _fileWrapper.Exists(Path.Combine(directory, _setScheduleModelFilename)))
                    && _fileWrapper.Exists(Path.Combine(directory, _directoriesFileName));
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
            IEnumerable<BackupDirectory> directories)
        {
            var serializedSchedule = JsonConvert.SerializeObject(setScheduleModel);
            var serializedDirectories = JsonConvert.SerializeObject(directories.Select(x => x.Directory.FullName).ToArray());
            var backupToolDir = _path;

            if (!_directoryWrapper.Exists(backupToolDir))
                _directoryWrapper.CreateDirectory(backupToolDir);

            var scheduleFile = Path.Combine(backupToolDir, _setScheduleModelFilename);
            var directoriesFile = Path.Combine(backupToolDir, _directoriesFileName);

            _fileWrapper.WriteAllText(scheduleFile, serializedSchedule);
            _fileWrapper.WriteAllText(directoriesFile, serializedDirectories);
        }

        public IEnumerable<BackupDirectory> DeserializeBackupDirectoriesFromFile()
        {
            var directoriesFile =  Path.Combine(_path, _directoriesFileName);
            var serialized = _fileWrapper.ReadAllText(directoriesFile);
            var dirs = JsonConvert.DeserializeObject<IEnumerable<string>>(serialized);

            var backupDirectories = new List<BackupDirectory>();
            foreach (var dir in dirs)
            {
                try
                {
                    backupDirectories.Add(new BackupDirectory(new DirectoryInfoWrap(new DirectoryInfo(dir))));
                }
                catch
                { 
                    // This means the user has deleted the directory
                }
            }

            return backupDirectories;
        }

        public ISetScheduleModel DeserializeSetScheduleModelFromFile()
        {
            var scheduleFile = Path.Combine(_path, _setScheduleModelFilename);
            var serialized = _fileWrapper.ReadAllText(scheduleFile);
            var model = JsonConvert.DeserializeObject<SetScheduleModel>(serialized);

            return model;
        }
    }
}
