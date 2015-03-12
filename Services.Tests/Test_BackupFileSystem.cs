using Domain;
using Moq;
using NUnit.Framework;
using Services.Disk.FileSystem;
using Services.Factories;
using System;
using System.Linq;
using SystemWrapper.IO;
using TestHelpers;

namespace Services.Tests
{
    public class Test_BackupFileSystem
    {
        [TestCase(@"e:\backups\foo")]
        [TestCase(@"e:\backups\bar")]
        [TestCase(@"e:\backups\foo\fizz")]
        public void Copy_creates_correct_mirrored_directory(string directoryPath)
        {
            _sut.Copy(new[] { _testDirectory });

            _directoryWrap.Verify(x => x.CreateDirectory(directoryPath), Times.Once());
        }

        [TestCase(@"c:\bar\test.txt", @"e:\backups\bar\test.txt")]
        [TestCase(@"c:\foo\amazing.txt", @"e:\backups\foo\amazing.txt")]
        public void Copy_copies_files_to_correct_directory(string sourceFileName, string destFileName)
        {
            _sut.Copy(new[] { _testDirectory });

            _fileWrap.Verify(x => x.Copy(sourceFileName, destFileName), Times.Once());
        }

        [Test]
        public void Total_size_is_calculated_correctly()
        {
            var totalSize = _sut.CalculateTotalSize(new[] {_testDirectory});

            Assert.AreEqual(450L, totalSize);
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

            _directoryFactory = new Mock<IDirectoryFactory>();
            _root = CreateDirectoryStructure();
            _backupRootDirectory = new BackupRootDirectory(_root.Object);

            var fun = new Func<string, MirroredDirectory>((x) => { return new MirroredDirectory(new DirectoryInfoWrap(x)); });
            _directoryFactory.Setup(x => x.CreateMirroredDirectory(It.IsAny<string>())).Returns(fun);

            _backupRoot = FakeDirectoryInfoBuilder.Create(@"e:\backups");
            _backupRootDirectory = new BackupRootDirectory(_backupRoot.Object);

            _testDirectory = new BackupDirectory(_root.Object);

            _sut = new BackupFileSystem(_backupRootDirectory, _directoryWrap.Object, _fileWrap.Object, _directoryFactory.Object);
        }

        private BackupFileSystem _sut;
        private Mock<IDirectoryWrap> _directoryWrap;
        private Mock<IFileWrap> _fileWrap;
        private Mock<IDirectoryFactory> _directoryFactory;

        private Mock<IDirectoryInfoWrap> _backupRoot;
        private BackupRootDirectory _backupRootDirectory;

        private BackupDirectory _testDirectory;
        private Mock<IDirectoryInfoWrap> _root;
    }
}
