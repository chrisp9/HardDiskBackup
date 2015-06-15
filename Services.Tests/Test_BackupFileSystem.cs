using Domain;
using Moq;
using NUnit.Framework;
using Services.Disk.FileSystem;
using Services.Factories;
using System;
using SystemWrapper.IO;
using TestHelpers;

namespace Services.Tests
{
    public class Test_BackupFileSystem
    {
        [TestCase(@"e:\backups\now\c\foo")]
        [TestCase(@"e:\backups\now\c\bar")]
        [TestCase(@"e:\backups\now\c\foo\fizz")]
        public async void Copy_creates_correct_mirrored_directory(string directoryPath)
        {
            _sut.Target(_backupRootDirectory);
            await _sut.Copy(new[] { _testDirectory }, _doNothingAction);

            _directoryWrap.Verify(x => x.CreateDirectory(directoryPath), Times.Once());
        }

        [TestCase(@"c:\bar\test.txt", @"e:\backups\now\c\bar\test.txt")]
        [TestCase(@"c:\foo\amazing.txt", @"e:\backups\now\c\foo\amazing.txt")]
        public async void Copy_copies_files_to_correct_directory(string sourceFileName, string destFileName)
        {
            _sut.Target(_backupRootDirectory);

            await _sut.Copy(new[] { _testDirectory }, _doNothingAction);

            _fileWrap.Verify(x => x.Copy(sourceFileName, destFileName), Times.Once());
        }

        [Test]
        public void Total_size_is_calculated_correctly()
        {
            _sut.Target(_backupRootDirectory);
            var totalSize = _sut.CalculateTotalSize(new[] { _testDirectory }).Result;

            Assert.AreEqual(450L, totalSize);
        }

        [Test]
        public async void Callback_is_executed_when_copying_file()
        {
            _sut.Target(_backupRootDirectory);
            bool hasBeenCalled = false;

            Action<IFileInfoWrap> callback = _ => hasBeenCalled = true;
            await _sut.Copy(new[] { _testDirectory }, callback);

            Assert.IsTrue(hasBeenCalled);
        }

        private Mock<IDirectoryInfoWrap> CreateDirectoryStructure()
        {
            var root = FakeDirectoryInfoBuilder.Create(@"c:\");
            var foo = FakeDirectoryInfoBuilder.Create(@"c:\foo");
            foo.WithFiles(new Tuple<string, long>("amazing.txt", 250L));

            var fizz = FakeDirectoryInfoBuilder.Create(@"c:\foo\fizz");
            var bar = FakeDirectoryInfoBuilder.Create(@"c:\bar");

            bar.WithFiles(new Tuple<string, long>("test.txt", 150L));
            bar.WithFiles(new Tuple<string, long>("buzz.txt", 50L));

            foo.WithSubDirectories(fizz);
            root.WithSubDirectories(new[] { foo, bar });

            return root;
        }

        [SetUp]
        public void Setup()
        {
            _directoryWrap = new Mock<IDirectoryWrap>();
            _fileWrap = new Mock<IFileWrap>();
            _doNothingAction = _ => { };

            _directoryFactory = new Mock<IDirectoryFactory>();
            _safeActionLogger = new Mock<ISafeActionPerformer>();

            _timestampedBackupRootProvider = new Mock<ITimestampedBackupRootProvider>();
            _root = CreateDirectoryStructure();
            _backupRootDirectory = new BackupRootDirectory(_root.Object);

            var fun = new Func<string, MirroredDirectory>((x) => { return new MirroredDirectory(new DirectoryInfoWrap(x)); });
            _directoryFactory.Setup(x => x.GetMirroredDirectoryFor(It.IsAny<string>())).Returns(fun);

            _backupRoot = FakeDirectoryInfoBuilder.Create(@"e:\backups");
            _backupRootDirectory = new BackupRootDirectory(_backupRoot.Object);

            var mockDirectoryInfo = new Mock<IDirectoryInfoWrap>();
            mockDirectoryInfo.Setup(x => x.FullName).Returns(@"e:\backups\now");

            _timestampedBackupRoot = new TimestampedBackupRoot(mockDirectoryInfo.Object);

            _timestampedBackupRootProvider.Setup(x => x.CreateTimestampedBackup(It.IsAny<BackupRootDirectory>()))
                .Returns(_timestampedBackupRoot);

            _testDirectory = new BackupDirectory(_root.Object);

            _sut = new BackupFileSystem(
                _directoryWrap.Object,
                _fileWrap.Object,
                _directoryFactory.Object,
                _timestampedBackupRootProvider.Object,
                new SafeActionPerformer(),
                Mock.Of<IErrorLogger>());
        }

        private Action<IFileInfoWrap> _doNothingAction;
        private BackupFileSystem _sut;
        private Mock<IDirectoryWrap> _directoryWrap;
        private Mock<IFileWrap> _fileWrap;
        private Mock<IDirectoryFactory> _directoryFactory;
        private Mock<ISafeActionPerformer> _safeActionLogger;

        private TimestampedBackupRoot _timestampedBackupRoot;
        private Mock<ITimestampedBackupRootProvider> _timestampedBackupRootProvider;

        private Mock<IDirectoryInfoWrap> _backupRoot;
        private BackupRootDirectory _backupRootDirectory;

        private BackupDirectory _testDirectory;
        private Mock<IDirectoryInfoWrap> _root;
    }
}