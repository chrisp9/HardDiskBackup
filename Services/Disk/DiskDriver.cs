using Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper.IO;

namespace Services.Disk
{
    public interface IDiskDriver
    {
        long CalculateTotalSize(IEnumerable<BackupDirectory> directories);
        bool IsFormatted(IDriveInfoWrap driveInfoWrap);
    }

    public class DiskDriver : IDiskDriver
    {
        private IDateTimeProvider _dateTimeProvider;
        private IDirectoryWrap _directoryWrap;

        public DiskDriver(
            IDateTimeProvider provider,
            IDirectoryWrap directoryWrap)
        {
            _dateTimeProvider = provider;
            _directoryWrap = directoryWrap;
        }

        private const string DiskBackup = "DiskBackupApp";

        public long CalculateTotalSize(IEnumerable<BackupDirectory> directories)
        {
            var currentSize = 0L;
            // Add file sizes.

            foreach (var directory in directories)
            {
                currentSize += CalculateSize(directory.Directory);
            }

            return currentSize;
        }

        public void CopyFiles(IEnumerable<BackupDirectory> directories, IDriveInfoWrap backupMedium)
        {
            var backupRoot = new DirectoryInfoWrap(
                Path.Combine(backupMedium.RootDirectory.FullName, DiskBackup));

            var targetDirectory = backupRoot
                .CreateSubdirectory(_dateTimeProvider.Now.ToString("dd-MM-yyyy HH:mm:ss"));


        }

        public void CreateMirroredDirectory(BackupDirectory directory, IDirectoryInfoWrap rootDirectory)
        {

        }

        // TODO: Doesn't deal with long filenames.
        private void CopyFiles(
            IDirectoryInfoWrap directory, 
            IDirectoryInfoWrap target)
        {
            

            foreach (var fi in directory.GetFiles())
            {
                
            }
        }

        private long CalculateSize(IDirectoryInfoWrap directory)
        {
            long currentSize = 0L;
            var fis = directory.GetFiles();
            foreach (var fi in fis)
            {
                currentSize += fi.Length;
            }
            // Add subdirectory sizes.
            var dis = directory.GetDirectories();
            foreach (var di in dis)
            {
                currentSize += CalculateSize(di);
            }
            return (currentSize);
        }

        public bool IsFormatted(IDriveInfoWrap driveInfoWrap)
        {
            throw new NotImplementedException();
        }
    }
}
