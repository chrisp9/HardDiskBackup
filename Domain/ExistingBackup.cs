namespace Domain
{
    public class ExistingBackup
    {
        public BackupDate BackupDate { get; private set; }

        public BackupTime BackupTime { get; private set; }

        public TimestampedBackupRoot BackupDirectory { get; private set; }

        public long SizeInBytes { get; private set; }

        public ExistingBackup(
            BackupDate backupDate,
            BackupTime backupTime,
            TimestampedBackupRoot directory,
            long sizeInBytes)
        {
            BackupDate = backupDate;
            BackupTime = backupTime;
            BackupDirectory = directory;
            SizeInBytes = sizeInBytes;
        }
    }
}