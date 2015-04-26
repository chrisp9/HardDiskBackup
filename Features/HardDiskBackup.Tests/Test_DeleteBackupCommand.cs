using Domain;
using HardDiskBackup.Commands;
using Moq;
using NUnit.Framework;
using Services.Disk.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace HardDiskBackup.Tests
{
    public class Test_DeleteBackupCommand
    {
        public void Executing_delete_backup_asks_fileSystem_to_delete_it()
        {
            _sut.Execute(_existingBackup);

            _mockBackupFileSystem.Verify(x => x.Delete(_existingBackup), Times.Once());
        }

        [SetUp]
        public void Setup()
        {
            _mockBackupFileSystem = new Mock<IBackupFileSystem>();
            _sut = new DeleteBackupCommand(_mockBackupFileSystem.Object);

            _existingBackup = new ExistingBackup(
                new BackupDate(DateTime.Now), new BackupTime(TimeSpan.FromSeconds(1)),
                new TimestampedBackupRoot(Mock.Of<IDirectoryInfoWrap>()),
                20L);

        }

        private ExistingBackup _existingBackup;
        private Mock<IBackupFileSystem> _mockBackupFileSystem; 
        private IDeleteBackupCommand _sut;
    }
}
