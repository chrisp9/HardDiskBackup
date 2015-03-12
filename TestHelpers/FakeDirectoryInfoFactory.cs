using Domain;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace TestHelpers
{
    public static class FakeDirectoryInfoBuilder
    {
        public static Mock<IDirectoryInfoWrap> Create(string path)
        {
            return CreateMock(path);
        }

        private static Mock<IDirectoryInfoWrap> CreateMock(string path)
        {
            var mock = new Mock<IDirectoryInfoWrap>();
            mock.Setup(x => x.FullName).Returns(path);

            return mock;
        }

        public static Mock<IDirectoryInfoWrap> WithSubDirectories(this Mock<IDirectoryInfoWrap> parent, params Mock<IDirectoryInfoWrap>[] children)
        {
            parent.Setup(x => x.GetDirectories()).Returns(children.Select(x => x.Object).ToArray());
            return parent;
        }

        public static Mock<IDirectoryInfoWrap> WithFiles(this Mock<IDirectoryInfoWrap> parent, params string[] children)
        {
            var files = parent.Object.GetFiles().ToList() ?? new List<IFileInfoWrap>();
            foreach (var child in children)
            {
                var mockFileInfoWrap = new Mock<IFileInfoWrap>();
                mockFileInfoWrap.Setup(x => x.FullName).Returns(parent.Object.FullName + @"\" + child);
                mockFileInfoWrap.Setup(x => x.Name).Returns(child);
                files.Add(mockFileInfoWrap.Object);
            }

            parent.Setup(x => x.GetFiles()).Returns(files.ToArray());
            return parent;
        }
    }
}
