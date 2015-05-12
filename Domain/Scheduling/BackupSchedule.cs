namespace Domain.Scheduling
{
    public abstract class BackupSchedule
    {
        protected IDateTimeProvider DateTimeProvider { get; private set; }

        protected INextBackupDateTimeFactory NextBackupDateTimeFactory { get; private set; }

        protected BackupTime BackupTime { get; private set; }

        public abstract NextBackupDateTime CalculateNextBackupDateTime();

        protected BackupSchedule(
            IDateTimeProvider provider,
            INextBackupDateTimeFactory factory,
            BackupTime backupTime)
        {
            DateTimeProvider = provider;
            NextBackupDateTimeFactory = factory;
            BackupTime = backupTime;
        }
    }
}