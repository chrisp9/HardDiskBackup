using Domain;
using Moq;
using NUnit.Framework;
using Services.Disk.FileSystem;
using Services.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Services.Tests
{
    public class Test_ExistingBackupsFactory
    {
        private Mock<IBackupFileSystem> _mockBackupFileSystem;
        private ExistingBackupsFactory _sut;

        private IEnumerable<Mock<IDirectoryInfoWrap>> _subDirectories;  //
        private Mock<IDirectoryInfoWrap> _backupRootDirectoryWrap; //
        private BackupRootDirectory _backupRootDirectory; //

        [Test]
        public async void Factory_returns_ExistingBackups_with_correct_date()
        {
            var y = await _sut.Create(_backupRootDirectory);

            Console.WriteLine("");
        }

        [SetUp]
        public void SetupSut()
        {
            _subDirectories = SetupSubDirectories();

            _backupRootDirectoryWrap = new Mock<IDirectoryInfoWrap>();

            _backupRootDirectoryWrap.Setup(x => x.GetDirectories())
                .Returns(_subDirectories.Select(x => x.Object).ToArray());

            _backupRootDirectory = new BackupRootDirectory(_backupRootDirectoryWrap.Object);

            _mockBackupFileSystem = new Mock<IBackupFileSystem>();
            _mockBackupFileSystem.Setup(x => x.CalculateTotalSize(It.IsAny<BackupDirectory>()))
                .Returns(Task.FromResult(5L));

            _sut = new ExistingBackupsFactory(_mockBackupFileSystem.Object);
        }

        private IEnumerable<Mock<IDirectoryInfoWrap>> SetupSubDirectories()
        {
            return CreateMockDirectoryInfoWraps(
                new[] {
                (@"2015-04-04_19.59.05"),
                (@"2015-04-05_00.00.01")});
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
    }
}
