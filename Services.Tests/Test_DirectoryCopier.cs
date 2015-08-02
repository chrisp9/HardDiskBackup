using Domain;
using Moq;
using NUnit.Framework;
using Services.Disk.FileSystem;
using System;
using System.Linq;
using SystemWrapper.IO;
using System.IO;
using Domain.Exceptions;

namespace Services.Tests
{
    public class Test_DirectoryCopier
    {
        private Mock<IFileCopier> _mockFileCopier;
        private Mock<IDirectoryWrap> _mockDirectoryWrap;
        private Mock<IDirectoryCreator> _mockDirectoryCreator;
        private Mock<IFileWrap> _mockFileWrap;
        private DirectoryCopier _sut;

        private Mock<IFileInfoWrap> _rootFile1;
        private Mock<IFileInfoWrap> _rootFile2;

        private Mock<IFileInfoWrap> _subDirectoryFile1;
        private Mock<IFileInfoWrap> _subDirectoryFile2;

        private Exception _exceptionToThrow = new UnauthorizedAccessException("uh..");
        private string _testRootDirectory = @"c:\test";
        private string _testSubDirectory = @"c:\test\subdir1";

        private string _testMirroredDirectory = @"e:\test\c\test";
        private string _mirroredRootDirectory = @"e:\test";

        private IFileInfoWrap[] _filesToCopy; 
        private IFileInfoWrap[] _subDirFilesToCopy;

        private Mock<IDirectoryInfoWrap> _mockDirectoryInfoWrap;

        private string _destinationDirectory = @"e:\test";

        [SetUp]
        public void Setup()
        {
            _mockFileCopier = new Mock<IFileCopier>();
            _mockDirectoryWrap = new Mock<IDirectoryWrap>();
            _mockDirectoryCreator = new Mock<IDirectoryCreator>();
            _mockFileWrap = new Mock<IFileWrap>();
            _mockDirectoryInfoWrap = new Mock<IDirectoryInfoWrap>();

            _mockDirectoryInfoWrap.Setup(x => x.FullName).Returns(_testRootDirectory);
            _mockDirectoryInfoWrap.Setup(x => x.Name).Returns(@"test");

            _rootFile1 = new Mock<IFileInfoWrap>();
            _rootFile2 = new Mock<IFileInfoWrap>();

            _subDirectoryFile1 = new Mock<IFileInfoWrap>();
            _subDirectoryFile2 = new Mock<IFileInfoWrap>();

            _rootFile1.Setup(x => x.FullName).Returns(Path.Combine(_testRootDirectory, "rootFile1"));
            _rootFile2.Setup(x => x.FullName).Returns(Path.Combine(_testRootDirectory, "rootFile2"));

            _subDirectoryFile1.Setup(x => x.FullName).Returns(Path.Combine(_testSubDirectory, "subFile1"));
            _subDirectoryFile2.Setup(x => x.FullName).Returns(Path.Combine(_testRootDirectory, "subFile2"));

            _subDirectoryFile1.Setup(x => x.Name).Returns("subFile1");
            _subDirectoryFile2.Setup(x => x.Name).Returns("subFile2");

            _filesToCopy = new[] { _rootFile1.Object, _rootFile2.Object }.ToArray();
            _subDirFilesToCopy = new[] { _subDirectoryFile1.Object, _subDirectoryFile2.Object }.ToArray();
          
            _sut = new DirectoryCopier(
                _mockFileCopier.Object,
                _mockFileWrap.Object,
                _mockDirectoryWrap.Object,
                _mockDirectoryCreator.Object);
        }

        [Test]
        public async void Fail_is_returned_when_copying_if_CreateDirectory_fails()
        {
            _mockDirectoryInfoWrap.Setup(x => x.GetFiles())
                .Throws(_exceptionToThrow);

            var errorToGenerate = new Error(_exceptionToThrow, "");

            _mockDirectoryCreator.Setup(x => x.CreateDirectoryIfNotExist(_testMirroredDirectory))
                .Returns(Result.Fail(errorToGenerate));

            var result = await _sut.CopySafe(_mockDirectoryInfoWrap.Object, _mirroredRootDirectory, (_) => { });
            Assert.AreEqual(true, result.IsFail);
            Assert.Contains(errorToGenerate, result.Errors.ToArray());
        }

        [Test]
        public async void Fail_is_returned_when_copying_if_GetFiles_fails()
        {
            _mockDirectoryInfoWrap.Setup(x => x.GetFiles())
                .Throws(_exceptionToThrow);

            _mockDirectoryCreator.Setup(x => x.CreateDirectoryIfNotExist(_testMirroredDirectory))
                .Returns(Result.Success());

            var result = await _sut.CopySafe(_mockDirectoryInfoWrap.Object, _mirroredRootDirectory, (_) => { });
            Assert.AreEqual(true, result.IsFail);
            Assert.AreEqual(_exceptionToThrow, result.Errors.Single().UnderlyingException);
            Assert.AreEqual(@"c:\test", result.Errors.Single().Location);
        }

        [Test]
        public async void Success_is_returned_if_everything_goes_well()
        {
            var filesToCopy = new[] { _rootFile1.Object, _rootFile2.Object }.ToArray();

            _mockDirectoryInfoWrap.Setup(x => x.GetFiles())
                .Returns(filesToCopy);

            _mockDirectoryCreator.Setup(x => x.CreateDirectoryIfNotExist(It.IsAny<string>()))
                .Returns(Result.Success());

            _mockFileCopier.Setup(x => x.CopyFiles(
                It.IsAny<IFileInfoWrap[]>(),
                It.IsAny<string>(),
                It.IsAny<Action<IFileInfoWrap>>()))
                .Returns(Result.Success());

            _mockDirectoryInfoWrap.Setup(x => x.GetDirectories())
                .Returns(Enumerable.Empty<IDirectoryInfoWrap>().ToArray());

            var result = await _sut.CopySafe(_mockDirectoryInfoWrap.Object, @"e:\c\test", (_) => { });

            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public async void Files_within_directory_are_copied()
        {
            var filesToCopy = new[] { _rootFile1.Object, _rootFile2.Object}.ToArray();

            _mockDirectoryInfoWrap.Setup(x => x.GetFiles())
                .Returns(filesToCopy);

            _mockDirectoryCreator.Setup(x => x.CreateDirectoryIfNotExist(It.IsAny<string>()))
                .Returns(Result.Success());

            CopyFilesReturnsSuccess();

            _mockDirectoryInfoWrap.Setup(x => x.GetDirectories())
                .Returns(Enumerable.Empty<IDirectoryInfoWrap>().ToArray());

            var result = await _sut.CopySafe(_mockDirectoryInfoWrap.Object, _mirroredRootDirectory, (_) => {});

            _mockFileCopier.Verify(x => x.CopyFiles(
                filesToCopy,  _testMirroredDirectory, It.IsAny<Action<IFileInfoWrap>>()), Times.Once());
        }

        [Test]
        public async void Files_in_subdirectories_are_also_copied()
        {
            _mockDirectoryInfoWrap.Setup(x => x.GetFiles())
                .Returns(_filesToCopy);

            var subDirectoryInfoWrap = new Mock<IDirectoryInfoWrap>();
            subDirectoryInfoWrap.Setup(x => x.GetFiles())
                .Returns(_subDirFilesToCopy);

            subDirectoryInfoWrap.Setup(x => x.Name).Returns("subdir1");
            subDirectoryInfoWrap.Setup(x => x.FullName).Returns(_testSubDirectory);

            _mockDirectoryInfoWrap.Setup(x => x.GetDirectories()).Returns(
                new[] { subDirectoryInfoWrap.Object }.ToArray());

            _mockDirectoryCreator.Setup(x => x.CreateDirectoryIfNotExist(It.IsAny<string>()))
                .Returns(Result.Success());

            CopyFilesReturnsSuccess();

            _mockDirectoryInfoWrap.Setup(x => x.GetDirectories())
                .Returns(new[] {subDirectoryInfoWrap.Object}.ToArray());

            var result = await _sut.CopySafe(_mockDirectoryInfoWrap.Object, _destinationDirectory, (_) => { });

            _mockFileCopier.Verify(x => x.CopyFiles(
                _subDirFilesToCopy, 
                 @"e:\test\c\test\subdir1", 
                 It.IsAny<Action<IFileInfoWrap>>()), Times.Once());
        }

        private void CopyFilesReturnsSuccess()
        {
            _mockFileCopier.Setup(x => x.CopyFiles(
                It.IsAny<IFileInfoWrap[]>(),
                It.IsAny<string>(),
                It.IsAny<Action<IFileInfoWrap>>()))
                .Returns(Result.Success());
        }
    }
}