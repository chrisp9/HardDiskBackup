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
        public void WriteAllText_is_called()
        {
            // Arrange
            SetupSut();
            _mockDirectoryWrap.Setup(x => x.Exists(It.IsAny<string>()))
                .Returns(false);

            // Act
            _sut.SerializeToFile(Mock.Of<ISetScheduleModel>(), new[] {new BackupDirectory(Mock.Of<IDirectoryInfoWrap>())});

            //Assert
            _mockFileWrap.Verify(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
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

            _mockFileWrap.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
            _mockEnvironmentWrap.Setup(x => x.AppDataPath).Returns(@"c:\");

            _sut = new JsonSerializer(
                _mockFileWrap.Object, 
                _mockDirectoryWrap.Object, 
                _mockEnvironmentWrap.Object);
        }
    }
}
