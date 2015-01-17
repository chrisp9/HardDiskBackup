using Domain;
using Moq;
using NUnit.Framework;
using Services.BackupSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Services.Tests
{
    public class Test_JsonLayer
    {
        private const string _fileName = "settings.json";
        private Mock<IFileWrap> _mockFileWrap;
        private Mock<IDirectoryWrap> _mockDirectoryWrap;
        private Mock<IEnvironmentWrap> _mockEnvironmentWrap;
        private IJsonLayer _sut;
        
        [Test]
        public void File_is_deleted_if_it_already_exists()
        {
            // Arrange
            SetupSut();
            _mockFileWrap.Setup(x => x.Exists(It.IsAny<string>()))
                .Returns(true);

            // Act
            _sut.SerializeToFile("");

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
            _sut.SerializeToFile("");

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
            _sut.SerializeToFile("");

            //Assert
            _mockFileWrap.Verify(x => x.Create(It.IsAny<string>()), Times.Once());
        }

        private void SetupSut()
        {
            _mockFileWrap = new Mock<IFileWrap>();
            _mockDirectoryWrap = new Mock<IDirectoryWrap>();
            _mockEnvironmentWrap = new Mock<IEnvironmentWrap>();

            _mockFileWrap.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
            _mockEnvironmentWrap.Setup(x => x.AppDataPath).Returns(@"c:\");

            _sut = new JsonLayer(
                _mockFileWrap.Object, 
                _mockDirectoryWrap.Object, 
                _mockEnvironmentWrap.Object);
        }
    }
}
