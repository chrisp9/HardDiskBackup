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
        private BackupFileSystem _sut;
        private Mock<IDirectoryWrap> _directoryWrap;
        private Mock<IFileWrap> _fileWrap;
        private Mock<IDirectoryFactory> _directoryFactory;

        private Mock<IDirectoryInfoWrap> _backupRoot;
        private BackupRootDirectory _backupRootDirectory;

        private BackupDirectory _directory;
        private Mock<IDirectoryInfoWrap> _root;

        [SetUp]
        public void Setup()
        {
            _directoryWrap = new Mock<IDirectoryWrap>();
            _fileWrap = new Mock<IFileWrap>();
         
            _directoryFactory = new Mock<IDirectoryFactory>();
            _root = CreateDirectoryStructure();
            _backupRootDirectory = new BackupRootDirectory(_root.Object);

             var fun = new Func<string, MirroredDirectory>((x) => {return new MirroredDirectory(new DirectoryInfoWrap(x));});
             _directoryFactory.Setup(x => x.CreateMirroredDirectory(It.IsAny<string>())).Returns(fun);

            _backupRoot = FakeDirectoryInfoBuilder.Create(@"e:\backups");
            _backupRootDirectory = new BackupRootDirectory(_backupRoot.Object);

            _directory = new BackupDirectory(_root.Object);

            _sut = new BackupFileSystem(_backupRootDirectory, _directoryWrap.Object, _fileWrap.Object, _directoryFactory.Object);
        }

        [Test]
        public void CreateMirroredDirectory_creates_correct_mirrored_directory()
        {
            _sut.Copy(new[] { _directory });

            _directoryWrap.Verify(x => x.CreateDirectory(@"e:\backups\foo"), Times.Once());
            _directoryWrap.Verify(x => x.CreateDirectory(@"e:\backups\bar"), Times.Once());
            _directoryWrap.Verify(x => x.CreateDirectory(@"e:\backups\foo\fizz"), Times.Once());

            _fileWrap.Verify(x => x.Copy(@"c:\bar\test.txt", @"e:\backups\bar\test.txt"), Times.Once());
        }

        private Mock<IDirectoryInfoWrap> CreateDirectoryStructure()
        {
            var root = FakeDirectoryInfoBuilder.Create(@"c:\");

            var foo = FakeDirectoryInfoBuilder.Create(@"c:\foo");
            var fizz = FakeDirectoryInfoBuilder.Create(@"c:\foo\fizz");

            var bar = FakeDirectoryInfoBuilder.Create(@"c:\bar");
            bar.WithFiles("test.txt");
            bar.WithFiles("buzz.txt");

            foo.WithSubDirectories(fizz);
            root.WithSubDirectories(new[] { foo, bar });
            return root;
        }
    }
}
