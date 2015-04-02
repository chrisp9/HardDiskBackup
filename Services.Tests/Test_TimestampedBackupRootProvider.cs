using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Services.Factories;
using Moq;
using Domain;
using SystemWrapper.IO;

namespace Services.Tests
{
    public class Test_TimestampedBackupRootProvider
    {
        [Test]
        public void new_directory_is_created()
        {
            _sut.CreateTimestampedBackup(_backupRootDirectory);

            _mockDirectoryInfoWrap.Verify(x => x.CreateSubdirectory(
                DateTime.MinValue.ToString("yyyy-MM-dd_HH.mm.ss")),
                Times.Once());
        }

        [Test]
        public void Timestamped_BackupRoot_is_returned_with_correct_path()
        {
            var result = _sut.CreateTimestampedBackup(_backupRootDirectory);

            Assert.AreEqual(
                @"c:\test\"
                + DateTime.MinValue.ToString("yyyy-MM-dd_HH.mm.ss"),
                result.Directory.FullName);
        }

        [SetUp]
        public void SetupSut()
        {
            _mockDateTimeProvider = new Mock<IDateTimeProvider>();
            _mockDirectoryInfoWrap = new Mock<IDirectoryInfoWrap>();
            _backupRootDirectory = new BackupRootDirectory(_mockDirectoryInfoWrap.Object);

            _mockDateTimeProvider.Setup(x => x.Now).Returns(DateTime.MinValue);
            _mockDirectoryInfoWrap.Setup(x => x.FullName).Returns(@"c:\test");


            _sut = new TimestampedBackupRootProvider(_mockDateTimeProvider.Object);
        }

        private Mock<IDirectoryInfoWrap> _mockDirectoryInfoWrap;
        private Mock<IDateTimeProvider> _mockDateTimeProvider;
        private BackupRootDirectory _backupRootDirectory;
        private TimestampedBackupRootProvider _sut;
    }
}
