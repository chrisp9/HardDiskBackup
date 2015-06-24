using Domain;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Services.Disk.FileSystem
{
    public interface IDirectoryCreator
    {
        Result CreateDirectoryIfNotExist(string directory);
    }

    public class DirectoryCreator : IDirectoryCreator
    {
        private IDirectoryWrap _directoryWrap;

        public DirectoryCreator(IDirectoryWrap directoryWrap)
        {
            _directoryWrap = directoryWrap;
        }

        public Result CreateDirectoryIfNotExist(string directory)
        {
            try
            {
                if (!_directoryWrap.Exists(directory))
                    _directoryWrap.CreateDirectory(directory);
            }
            catch (Exception e)
            {
                return Result.Fail(e);
            }

            return Result.Success();
        }
    }
}