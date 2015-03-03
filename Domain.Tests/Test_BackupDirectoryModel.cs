using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Services;
using Services.Disk;
using Moq;
using NUnit.Framework;
using SystemWrapper.IO;
using Services.Factories;

namespace Domain.Tests
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

        [TestCase(@"c:\blah", @"c:\blah\test", true)]
        [TestCase(@"c:\blah", @"c:/blah/foo", true)]
        [TestCase(@"c:\blah", @"c:/blah/foo/bar\fizz/buzz", true)]
        [TestCase(@"c:\blah\", @"c:/blah/test\foo/bar\fizz/buzz", true)]
        [TestCase(@"c:/blah/test\foo/bar\fizz/buzz", @"c:\blah\", false)]
        [TestCase(@"c:\blah\test", @"c:\blah", false)]
        [TestCase(@"c:/blah/foo", @"c:\blah", false)]
        [TestCase(@"c:/blah/foo/bar\fizz/buzz", @"c:\blah", false)]
        [TestCase(@"c:/foo", @"c:/bar", false)]
        [TestCase(@"c:/foo", @"c:/foo", true)]
        public void Subdirectory_check_is_correct(string directory1, string directory2, bool expected)
        {
            // Arrange
            _sut.Add(CreateBackupDirectory(directory1));

            // Act
            var isSubDirectory =_sut.IsSubdirectoryOfExisting(directory2);

            // Assert
            Assert.AreEqual(expected, isSubDirectory);
        }

        [TestCase(@"c:\foo\bar", @"c:\foo")]
        [TestCase(@"c:\blah\test", @"c:\blah")]
        [TestCase(@"c:/blah/foo", @"c:\blah")]
        [TestCase(@"c:/blah/foo/bar\fizz/buzz", @"c:\blah")]
        [TestCase(@"c:/blah/test\foo/bar\fizz/buzz", @"c:\blah\")]
        public void Subdirectory_is_removed_if_parent_directory_is_added(string directory1, string directory2)
        {
            // Arrange
            var backupDirectory1 = CreateBackupDirectory(directory1);
            var backupDirectory2 = CreateBackupDirectory(directory2);

            // Act
            _sut.Add(backupDirectory1);
            _sut.Add(backupDirectory2);

            //Assert
            Assert.Contains(backupDirectory2, _sut.BackupDirectories);
            Assert.AreEqual(_sut.BackupDirectories.Count, 1);
        }

        private BackupDirectory CreateBackupDirectory(string path)
        {
            var mockDirectoryInfoWrap = new Mock<IDirectoryInfoWrap>();
            mockDirectoryInfoWrap.Setup(x => x.FullName).Returns(path);

            return new BackupDirectory(mockDirectoryInfoWrap.Object);
        }
    }
}

