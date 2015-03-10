using Domain;
using Moq;
using NUnit.Framework;
using Services.Disk.FileSystem;
using Services.Factories;
using SystemWrapper.IO;

namespace Services.Tests
{
    public class Test_BackupFileSystem
    {
        private BackupFileSystem _sut;
        private Mock<IDirectoryWrap> _directoryWrap;
        private Mock<IDirectoryInfoWrap> _backupRoot;
        private Mock<IDirectoryFactory> _directoryFactory;
        private BackupRootDirectory _backupRootDirectory;

        [SetUp]
        public void Setup()
        {
            _directoryWrap = new Mock<IDirectoryWrap>();
            _backupRoot = new Mock<IDirectoryInfoWrap>();
            _directoryFactory = new Mock<IDirectoryFactory>();
            _backupRootDirectory = new BackupRootDirectory(_backupRoot.Object);
            _sut = new BackupFileSystem(_backupRootDirectory, _directoryWrap.Object, _directoryFactory.Object);
        }

        //[TestCase(@"c:\users\chris\desktop", @"e:\backuproot", @"e:\backuproot\users\chris\desktop")]
        //[TestCase(@"e:\users\chris\desktop\backuproot", @"f:\backuproot", @"f:\backuproot\users\chris\desktop\backuproot")]
        //public void CreateMirroredDirectory_creates_correct_mirrored_directory(string source, string backupRoot, string expected)
        //{
        //    _backupRoot.Setup(x => x.FullName).Returns(backupRoot);
        //    var directoryToBackup = new BackupDirectory(new DirectoryInfoWrap(source));
        //
        //    _sut.CreateMirroredDirectory(directoryToBackup);
        //
        //    _directoryWrap.Verify(x => x.CreateDirectory(expected), Times.Once());
        //}
    }
}
