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
    public enum BackupFileSystemServiceStatus
    {
        Idle,
        Calculating,
        Copying
    }

    public interface IBackupFileSystemService
    {

        BackupFileSystemService Status { get; }
        bool IsFormatted(IDriveInfoWrap backupMedium);
        void Format(IDriveInfoWrap backupMedium);
        Task<long> CalculateTotalSize(IEnumerable<BackupDirectory> backupDirectories);
        void PerformBackup(IDriveInfoWrap backupMedium, IEnumerable<BackupDirectory> backupDirectories);

    }

    public class BackupFileSystemService : IBackupFileSystemService
    {
        public BackupFileSystemServiceStatus Status { get; private set; }

        private const string DiskBackup = "DiskBackupApp";
        private IDateTimeProvider _dateTimeProvider;

        public BackupFileSystemService(
            IDateTimeProvider provider) 
        {
            _dateTimeProvider = provider;
        }

        public bool IsFormatted(IDriveInfoWrap backupMedium)
        {
            return
                backupMedium
                    .RootDirectory
                    .GetDirectories()
                    .Select(x => x.Name)
                    .Contains(DiskBackup);
        }

        public void Format(IDriveInfoWrap backupMedium)
        {
            backupMedium.RootDirectory.CreateSubdirectory(DiskBackup);
        }

        public async Task<long> CalculateTotalSize(IEnumerable<BackupDirectory> directories)
        {
            Status = BackupFileSystemServiceStatus.Calculating;
            var totalSize = await CalculateTotalSize(directories);
            Status = BackupFileSystemServiceStatus.Idle;
            return totalSize;
        }

        public void PerformBackup(IDriveInfoWrap backupMedium, IEnumerable<BackupDirectory> backupDirectories)
        {
            Status = BackupFileSystemServiceStatus.Copying; 

            var backupRoot = new DirectoryInfoWrap(
                Path.Combine(backupMedium.RootDirectory.FullName, DiskBackup));

            var targetDirectory = backupRoot
                .CreateSubdirectory(_dateTimeProvider.Now.ToString("dd-MM-yyyy HH:mm:ss"));


        }

        BackupFileSystemService IBackupFileSystemService.Status
        {
            get { throw new NotImplementedException(); }
        }
    }
}
