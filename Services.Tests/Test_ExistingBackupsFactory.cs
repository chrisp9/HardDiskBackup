using Domain;
using Moq;
using NUnit.Framework;
using Services.Disk.FileSystem;
using Services.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Services.Tests
{
    public class Test_ExistingBackupsFactory
    {
        [Test]
        public async void Factory_returns_ExistingBackups_with_correct_date()
        {
            var existingBackups = await _sut.Create(_backupRootDirectory);

            var date = ParseDate(_firstDirectoryName);

            Assert.AreEqual(date.Day, existingBackups.First().BackupDate.Day);
            Assert.AreEqual(date.Month, existingBackups.First().BackupDate.Month);
            Assert.AreEqual(date.Year, existingBackups.First().BackupDate.Year);
        }

        [Test]
        public async void Factory_returns_ExistingBackups_with_correct_time()
        {
            var existingBackups = await _sut.Create(_backupRootDirectory);

            var date = ParseDate(_firstDirectoryName);

            Assert.AreEqual(date.Hour, existingBackups.First().BackupTime.Hours);
            Assert.AreEqual(date.Minute, existingBackups.First().BackupTime.Minutes);
            Assert.AreEqual(date.Second, existingBackups.First().BackupTime.Seconds);
        }

        [Test]
        public async void Factory_asks_fileSystem_for_size_of_directories()
        {
            var existingBackups = await _sut.Create(_backupRootDirectory);

            _mockBackupFileSystem.Verify(x => x.CalculateTotalSize(It.IsAny<IDirectoryInfoWrap>()), Times.Exactly(2));
        }

        [Test]
        public async void A_Timestamped_Directory_is_returned()
        {
            var existingBackups = await _sut.Create(_backupRootDirectory);

            Assert.AreEqual(_firstDirectoryName, existingBackups.First().BackupDirectory.Directory.Name);
            Assert.AreEqual(_secondDirectoryName, existingBackups.Last().BackupDirectory.Directory.Name);
        }

        [SetUp]
        public void SetupSut()
        {
            _subDirectories = SetupSubDirectories();

            _backupRootDirectoryWrap = new Mock<IDirectoryInfoWrap>();

            _backupRootDirectoryWrap.Setup(x => x.GetDirectories())
                .Returns(_subDirectories.Select(x => x.Object).ToArray());

            _backupRootDirectory = new BackupRootDirectory(_backupRootDirectoryWrap.Object);

            _mockBackupFileSystem = new Mock<IBackupFileSystem2>();
            _mockBackupFileSystem.Setup(x => x.CalculateTotalSize(It.IsAny<IDirectoryInfoWrap>()))
                .Returns(Task.FromResult(Result<long>.Success(5L)));

            _sut = new ExistingBackupsFactory(_mockBackupFileSystem.Object);
        }

        private DateTime ParseDate(string path)
        {
            return DateTime.ParseExact(path, "yyyy-MM-dd_HH.mm.ss", null);
        }

        private IEnumerable<Mock<IDirectoryInfoWrap>> SetupSubDirectories()
        {
            return CreateMockDirectoryInfoWraps(
                new[] {
                (_firstDirectoryName),
                (_secondDirectoryName)});
        }

        private IEnumerable<Mock<IDirectoryInfoWrap>> CreateMockDirectoryInfoWraps(IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                var mock = new Mock<IDirectoryInfoWrap>();
                mock.Setup(x => x.Name).Returns(name);
                yield return mock;
            }
        }

        private Mock<IBackupFileSystem2> _mockBackupFileSystem;
        private ExistingBackupsFactory _sut;

        private IEnumerable<Mock<IDirectoryInfoWrap>> _subDirectories;
        private Mock<IDirectoryInfoWrap> _backupRootDirectoryWrap;
        private BackupRootDirectory _backupRootDirectory;
        private string _firstDirectoryName = @"2015-04-04_19.59.05";
        private string _secondDirectoryName = @"2015-04-05_00.00.01";
    }
}