using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Services.Disk.FileSystem
{
    public static class FileSystemExtensions
    {
        public static Result<IFileInfoWrap[]> GetFilesSafe(this IDirectoryInfoWrap directory)
        {
            try
            {
                return Result<IFileInfoWrap[]>.Success(directory.GetFiles());
            }
            catch (Exception e)
            {
                return Result<IFileInfoWrap[]>.Fail(e);
            }
        }

        public static Result<IDirectoryInfoWrap[]> GetDirectoriesSafe(this IDirectoryInfoWrap directory)
        {
            try
            {
                return Result<IDirectoryInfoWrap[]>.Success(directory.GetDirectories());
            }
            catch (Exception e)
            {
                return Result<IDirectoryInfoWrap[]>.Fail(e);
            }
        }
    }
}
