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

namespace HardDiskBackup.Tests
{
    public class Test_ExistingBackupsModel
    {
        [Test]
        public void ExistingBackups_is_initially_empty()
        {
            Assert.IsEmpty(_sut.ExistingBackups);
        }

        [Test]
        public void ExistingBackups_is_empty_after_adding_then_removing()
        {
            _sut.Add(_formattedExistingBackup);
            _sut.Remove(_formattedExistingBackup);

            Assert.IsEmpty(_sut.ExistingBackups);
        }
        
        [SetUp]
        public void SetupSut()
        {
            _sut = new ExistingBackupsModel();
            _mockDirectory = new Mock<IDirectoryInfoWrap>();
            _mockDirectory.Setup(x => x.FullName).Returns(@"e:\test");

            _existingBackup = new ExistingBackup(new BackupDate(DateTime.Now), new BackupTime(TimeSpan.FromSeconds(1)), new TimestampedBackupRoot(_mockDirectory.Object), 100L);
            _formattedExistingBackup = new FormattedExistingBackup(_existingBackup);
            _backupRootDirectory = new BackupRootDirectory(_mockDirectory.Object);
        }

        private IExistingBackupsModel _sut;
        private BackupRootDirectory _backupRootDirectory;
        private FormattedExistingBackup _formattedExistingBackup;
        private ExistingBackup _existingBackup;
        private Mock<IDirectoryInfoWrap> _mockDirectory;
    }
}
