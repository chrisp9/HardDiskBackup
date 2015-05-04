using Domain;
using HardDiskBackup.Commands;
using Moq;
using NUnit.Framework;
using Services.Disk.FileSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace HardDiskBackup.Tests
{
    public class Test_DeleteBackupCommand
    {
        [Test]
        public void Executing_delete_backup_asks_fileSystem_to_delete_it()
        {
            _sut.Execute(_formattedExistingBackup);

            _mockBackupFileSystem.Verify(x => x.Delete(_formattedExistingBackup.ExistingBackup, It.IsAny<Action>()), Times.Once());
        }
   
        [Test]
        public void Delete_in_progress_is_set_to_true_when_delete_in_progress()
        {
            _sut.Execute(_formattedExistingBackup);

            Assert.IsTrue(_formattedExistingBackup.DeleteIsInProgress);
        }

        [Test]
        public void Delete_in_progress_is_initially_false()
        {
            Assert.IsFalse(_formattedExistingBackup.DeleteIsInProgress);
        }

        [SetUp]
        public void Setup()
        {
            _mockBackupFileSystem = new Mock<IBackupFileSystem>();
            _mockExistingBackupModel = new Mock<IExistingBackupsModel>();
            _formattedExistingBackup = new FormattedExistingBackup(_existingBackup);

            _sut = new DeleteBackupCommand(_mockBackupFileSystem.Object, _mockExistingBackupModel.Object);

            _existingBackup = new ExistingBackup(
                new BackupDate(DateTime.Now), new BackupTime(TimeSpan.FromSeconds(1)),
                new TimestampedBackupRoot(Mock.Of<IDirectoryInfoWrap>()),
                20L);
        }

        private FormattedExistingBackup _formattedExistingBackup;
        private ExistingBackup _existingBackup;
        private Mock<IBackupFileSystem> _mockBackupFileSystem; 
        private IDeleteBackupCommand _sut;
        private Mock<IExistingBackupsModel> _mockExistingBackupModel;
    }
}
