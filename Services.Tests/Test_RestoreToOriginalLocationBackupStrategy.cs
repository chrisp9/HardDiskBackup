using Domain;
using Moq;
using NUnit.Framework;
using Services.Disk.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Services.Tests
{
    public class Test_RestoreToOriginalLocationBackupStrategy
    {
        private RestoreToOriginalLocationBackupStrategy _sut;
        private Mock<IErrorLogger> _mockErrorLogger;
        private Mock<IBackupFileSystem> _mockBackupFileSystem;

        [SetUp]
        public void SetUp()
        {
            _mockErrorLogger = new Mock<IErrorLogger>();
            _mockBackupFileSystem = new Mock<IBackupFileSystem>();

            _sut = new RestoreToOriginalLocationBackupStrategy(
                _mockErrorLogger.Object,
                _mockBackupFileSystem.Object);
        }

        [Test]
        public void Restore_restores_to_correct_location()
        {
            var existingBackup = CreateExistingBackup();

        }

        private ExistingBackup CreateExistingBackup()
        {
            var backupDate = new BackupDate(2015, 06,22);
            var backupTime = new BackupTime(08, 53, 00);

            var mockDirectoryInfoWrap = new Mock<IDirectoryInfoWrap>();

            mockDirectoryInfoWrap.Setup(x => x.FullName)
                .Returns(@"e:\DiskBackupApp\2015-06-22_08.53.00");

            var timestampedBackupRoot = new TimestampedBackupRoot(mockDirectoryInfoWrap.Object);

            return new ExistingBackup(backupDate, backupTime, timestampedBackupRoot, 100L);
        }
    }
}
