using Domain;
using Registrar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Services.Disk.FileSystem
{
    public interface IFileCopier
    {
        Result CopyFiles(
            IFileInfoWrap[] files, 
            string destination,
            Action<IFileInfoWrap> onFileCopied);
    }

    [Register(LifeTime.Transient)]
    public class FileCopier : IFileCopier
    {
        private readonly IFileWrap _fileWrap;

        public FileCopier(IFileWrap fileWrap)
        {
            _fileWrap = fileWrap;
        }

        public Result CopyFiles(IFileInfoWrap[] files, string destination, Action<IFileInfoWrap> onFileCopied)
        {
            var resultBuilder = ResultBuilder.Create();

            foreach (var file in files)
            {
                try
                {
                    var destinationPath = Path.Combine(destination, file.Name);
                    _fileWrap.Copy(file.FullName, destinationPath);
                    _fileWrap.SetAttributes(destinationPath, file.Attributes &= ~FileAttributes.ReadOnly);
                }
                catch (Exception e)
                {
                    resultBuilder.Add(e);
                }

                onFileCopied(file);
            }

            return resultBuilder.Build();
        }
    }
}