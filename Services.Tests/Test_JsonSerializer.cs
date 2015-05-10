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