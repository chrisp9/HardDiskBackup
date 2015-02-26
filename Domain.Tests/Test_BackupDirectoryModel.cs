using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using HardDiskBackup;
    using Domain;
    using Services;
    using Services.Disk;
    using Moq;
    using NUnit.Framework;
    using SystemWrapper.IO;
    using Services.Factories;

    namespace HardDiskBackup.Tests
    {
        public class Test_BackupDirectoryModel
        {
            private BackupDirectory _backupDirectory;
            private BackupDirectoryModel _sut;

            [SetUp]
            public void Setup()
            {
                // Arrange
                _sut = new BackupDirectoryModel();
                var mockDirectoryInfoWrap = new Mock<IDirectoryInfoWrap>();
                mockDirectoryInfoWrap.Setup(x => x.FullName).Returns(@"c:\");

                _backupDirectory = new BackupDirectory(mockDirectoryInfoWrap.Object);
            }

            [Test]
            public void New_BackupDirectories_are_added_to_collection()
            {
                // Act
                _sut.Add(_backupDirectory);

                // Assert
                Assert.Contains(_backupDirectory, _sut.BackupDirectories);
            }

            [Test]
            public void BackupDirectories_can_be_removed_from_collection()
            {
                // Act
                _sut.Add(_backupDirectory);
                _sut.Remove(_backupDirectory);

                // Assert
                Assert.AreEqual(0, _sut.BackupDirectories.Count);
            }

            [Test]
            public void Nothing_happens_if_a_nonexistent_BackupDirectory_is_removed_from_collection()
            {
                // Act
                _sut.Remove(_backupDirectory);

                // Assert
                Assert.AreEqual(0, _sut.BackupDirectories.Count);
            }

            [Test]
            public void Adding_multiple_BackupDirectories_then_removing_one()
            {
                // Act
                _sut.Add(_backupDirectory);
                _sut.Add(new BackupDirectory(_backupDirectory.Directory));
                _sut.Remove(_backupDirectory);

                // Assert
                Assert.AreEqual(1, _sut.BackupDirectories.Count);
            }

            [Test]
            public void BackupDirectories_is_initially_empty()
            {
                // Assert
                Assert.AreEqual(0, _sut.BackupDirectories.Count);
            }

            private IBackupDirectoryFactory SetupFactory(BackupDirectory backupDirectory)
            {
                var mockBackupDirectoryFactory = new Mock<IBackupDirectoryFactory>();

                mockBackupDirectoryFactory
                    .Setup(x => x.Create(It.IsAny<string>()))
                    .Returns(backupDirectory);

                return mockBackupDirectoryFactory.Object;
            }
        }
    }
}
