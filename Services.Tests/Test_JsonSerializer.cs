using Domain;
using Moq;
using NUnit.Framework;
using Services.Persistence;
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
        public void File_is_deleted_if_it_already_exists()
        {
            // Arrange
            SetupSut();
            _mockFileWrap.Setup(x => x.Exists(It.IsAny<string>()))
                .Returns(true);

            // Act
            _sut.SerializeToFile(Mock.Of<IBackupSettings>());

            //Assert
            _mockFileWrap.Verify(x => x.Delete(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void File_is_not_deleted_if_it_doesnt_already_exist()
        {
            // Arrange
            SetupSut();
            _mockFileWrap.Setup(x => x.Exists(It.IsAny<string>()))
                .Returns(false);

            // Act
            _sut.SerializeToFile(Mock.Of<IBackupSettings>());

            //Assert
            _mockFileWrap.Verify(x => x.Delete(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void Backup_tool_directory_is_created_if_it_doesnt_exist()
        {
            // Arrange
            SetupSut();
            _mockDirectoryWrap.Setup(x => x.Exists(It.IsAny<string>()))
                .Returns(false);

            // Act
            _sut.SerializeToFile(Mock.Of<IBackupSettings>());

            //Assert
            _mockFileWrap.Verify(x => x.Create(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void Deserialize_returns_correct_data()
        {
            // Arrange
            SetupSut();

            var mockPersistedOptions = new Mock<IBackupSettings>();
            mockPersistedOptions.Setup(x => x.BackupDirectories).Returns(new[] { CreateBackupDirectory("c:\\stuff") });
            mockPersistedOptions.Setup(x => x.NextBackup).Returns(new BackupDateTime(DateTime.Now));

            // Act
            _sut.SerializeToFile(Mock.Of<IBackupSettings>());

            //Assert
            _mockFileWrap.Verify(x => x.Create(It.IsAny<string>()), Times.Once());
        }

        private BackupDirectory CreateBackupDirectory(string path)
        {
            var mockDirectoryInfoWrap = new Mock<IDirectoryInfoWrap>();
            mockDirectoryInfoWrap.Setup(x => x.FullName).Returns(path);

            return new BackupDirectory(mockDirectoryInfoWrap.Object);
        }

        private void SetupSut()
        {
            _mockFileWrap = new Mock<IFileWrap>();
            _mockDirectoryWrap = new Mock<IDirectoryWrap>();
            _mockEnvironmentWrap = new Mock<IEnvironmentWrap>();

            _mockFileWrap.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
            _mockEnvironmentWrap.Setup(x => x.AppDataPath).Returns(@"c:\");

            _sut = new JsonSerializer(
                _mockFileWrap.Object, 
                _mockDirectoryWrap.Object, 
                _mockEnvironmentWrap.Object);
        }
    }
}
