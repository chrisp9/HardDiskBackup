using Domain;
using System;
using SystemWrapper.IO;
using Domain.Exceptions;

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
                return Result<IFileInfoWrap[]>.Fail(new Error(e, directory.FullName));
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
                return Result<IDirectoryInfoWrap[]>.Fail(new Error(e, directory.FullName));
            }
        }
    }
}
