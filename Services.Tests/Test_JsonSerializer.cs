using Domain;
using Domain.Scheduling;
using Moq;
using NUnit.Framework;
using Services.Factories;
using Services.Persistence;
using Services.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Services.Tests
{
    public class Test_JsonSerializer
    {
        private const string _fileName = "settings.json";
        private Mock<IFileWrap> _mockFileWrap;
        private Mock<IDirectoryWrap> _mockDirectoryWrap;
        private Mock<IEnvironmentWrap> _mockEnvironmentWrap;
        private IJsonSerializer _sut;
        
        [Test]
        public void WriteAllText_is_called_twice()
        {
            // Arrange
            SetupSut();
            _mockDirectoryWrap.Setup(x => x.Exists(It.IsAny<string>()))
                .Returns(false);

            // Act
            _sut.SerializeToFile(Mock.Of<ISetScheduleModel>(), new[] {new BackupDirectory(Mock.Of<IDirectoryInfoWrap>())});

            //Assert
            _mockFileWrap.Verify(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [Test]
        public void Serialized_scheduleModel_is_correct()
        {
            SetupSut();

            _setScheduleModel.DayOfMonth = 10;
            _setScheduleModel.DayOfWeek = DayOfWeek.Wednesday;
            _setScheduleModel.Time = TimeSpan.FromHours(10);
            _setScheduleModel.SetScheduleType(BackupScheduleType.Weekly);

            var serialized = "{\"Time\":\"10:00:00\",\"DayOfMonth\":10,\"DayOfWeek\":3,\"ScheduleType\":1}";

            _sut.SerializeToFile(_setScheduleModel, new[] { new BackupDirectory(Mock.Of<IDirectoryInfoWrap>()) });

            _mockFileWrap.Verify(x => x.WriteAllText(It.IsAny<string>(), serialized));
        }

        [Test]
        public void Serialized_backupDirectories_are_correct()
        {
            SetupSut();
            var mockDiw = new Mock<IDirectoryInfoWrap>();
            mockDiw.Setup(x => x.FullName).Returns(@"c:\users\chris\desktop");

            var mockDiw2 = new Mock<IDirectoryInfoWrap>();
            mockDiw2.Setup(x => x.FullName).Returns(@"c:\users\chris\documents");

            _sut.SerializeToFile(_setScheduleModel, new[] { new BackupDirectory(mockDiw.Object), new BackupDirectory(mockDiw2.Object)});
            
            var serialized = "[\"c:\\\\users\\\\chris\\\\desktop\",\"c:\\\\users\\\\chris\\\\documents\"]";
            _mockFileWrap.Verify(x => x.WriteAllText(It.IsAny<string>(), serialized), Times.Once());
        }

        [Test]
        public void Serialized_BackupDirectories_are_written_to_correct_place()
        {
            SetupSut();

            _sut.SerializeToFile(_setScheduleModel, new[] { new BackupDirectory(Mock.Of<IDirectoryInfoWrap>()) });

            _mockFileWrap.Verify(x => x.WriteAllText(@"c:\users\chris\appdata\local\HdBackupTool\directories.json", 
                It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void Serialized_Schedule_is_written_to_correct_place()
        {
            SetupSut();

            _sut.SerializeToFile(_setScheduleModel, new[] { new BackupDirectory(Mock.Of<IDirectoryInfoWrap>()) });

            _mockFileWrap.Verify(x => x.WriteAllText(@"c:\users\chris\appdata\local\HdBackupTool\schedule.json", 
                It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void Settings_directory_is_created_if_it_does_not_already_exist()
        {
            SetupSut();

            _sut.SerializeToFile(_setScheduleModel, new[] { new BackupDirectory(Mock.Of<IDirectoryInfoWrap>()) });
            var settingsDirectory = @"c:\users\chris\appdata\local\HdBackupTool";

            _mockDirectoryWrap.Verify(x => x.CreateDirectory(settingsDirectory), Times.Once());
        }

        [Test]
        public void Settings_directory_is_not_created_if_it_already_exists()
        {
            SetupSut();

            var settingsDirectory = @"c:\users\chris\appdata\local\HdBackupTool";
            _mockDirectoryWrap.Setup(x => x.Exists(settingsDirectory)).Returns(true);
            _sut.SerializeToFile(_setScheduleModel, new[] { new BackupDirectory(Mock.Of<IDirectoryInfoWrap>()) });

            _mockDirectoryWrap.Verify(x => x.CreateDirectory(settingsDirectory), Times.Never());
        }

        [Test]
        public void Deserialize_works_as_expected_for_schedule()
        {
            SetupSut();

            var serialized = "{\"Time\":\"10:00:00\",\"DayOfMonth\":10,\"DayOfWeek\":3,\"ScheduleType\":1}";
            _mockFileWrap.Setup(x => x.ReadAllText(@"c:\users\chris\appdata\local\HdBackupTool\schedule.json"))
                .Returns(serialized);
                  
            var result = _sut.DeserializeSetScheduleModelFromFile();

            Assert.AreEqual(TimeSpan.FromHours(10), result.Time);
            Assert.AreEqual(10, result.DayOfMonth);
            Assert.AreEqual(DayOfWeek.Wednesday, result.DayOfWeek);
            Assert.AreEqual(BackupScheduleType.Weekly, result.ScheduleType);
        }

        [Test]
        public void Deserialize_works_as_expected_for_directories()
        {
            SetupSut();
            
            var serialized = "[\"c:\\\\users\\\\chris\\\\desktop\",\"c:\\\\users\\\\chris\\\\documents\"]";
            
            _mockFileWrap.Setup(x => x.ReadAllText(@"c:\users\chris\appdata\local\HdBackupTool\directories.json"))
                .Returns(serialized);

            var result = _sut.DeserializeBackupDirectoriesFromFile();

            Assert.AreEqual(@"c:\users\chris\desktop", result.First().Directory.FullName);
            Assert.AreEqual(@"c:\users\chris\documents", result.Last().Directory.FullName);
        }

        [Test]
        public void FileExists_returns_true_if_Model_file_and_Directories_file_both_exist()
        {
            SetupSut();

            _mockFileWrap.Setup(x => x.Exists(_directoriesFile)).Returns(true);
            _mockFileWrap.Setup(x => x.Exists(_scheduleFile)).Returns(true);
            _mockDirectoryWrap.Setup(x => x.Exists(_toolDirectory)).Returns(true);

            Assert.IsTrue(_sut.FileExists);
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void FileExists_returns_false_if_either_Model_file_or_Directories_file_do_not_exist(
            bool directoriesFileExists, bool scheduleFileExists)
        {
            SetupSut();

            _mockFileWrap.Setup(x => x.Exists(_directoriesFile)).Returns(directoriesFileExists);
            _mockFileWrap.Setup(x => x.Exists(_scheduleFile)).Returns(scheduleFileExists);
            _mockDirectoryWrap.Setup(x => x.Exists(_toolDirectory)).Returns(true);

            Assert.IsFalse(_sut.FileExists);
        }

        private string _directoriesFile = @"c:\users\chris\appdata\local\HdBackupTool\directories.json";
        private string _scheduleFile = @"c:\users\chris\appdata\local\HdBackupTool\schedule.json";
        private string _toolDirectory = @"c:\users\chris\appdata\local\HdBackupTool";

        private BackupDirectory CreateBackupDirectory(string path)
        {
            var mockDirectoryInfoWrap = new Mock<IDirectoryInfoWrap>();
            mockDirectoryInfoWrap.Setup(x => x.FullName).Returns(path);

            return new BackupDirectory(mockDirectoryInfoWrap.Object);
        }

        private ISetScheduleModel _setScheduleModel;

        private void SetupSut()
        {
            _mockFileWrap = new Mock<IFileWrap>();
            _mockDirectoryWrap = new Mock<IDirectoryWrap>();
            _mockEnvironmentWrap = new Mock<IEnvironmentWrap>();
            _setScheduleModel = new SetScheduleModel(Mock.Of<IBackupScheduleFactory>());

            _mockEnvironmentWrap.Setup(x => x.AppDataPath).Returns(@"c:\users\chris\appdata\local");

            _mockFileWrap.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);

            _sut = new JsonSerializer(
                _mockFileWrap.Object, 
                _mockDirectoryWrap.Object, 
                _mockEnvironmentWrap.Object);
        }
    }
}