using Moq;

namespace Domain.Tests.BackupSchedule
{
    public class BackupScheduleTestBase
    {
        protected BackupDate ActualBackupDate = null;
        protected BackupTime ActualBackupTime = null;

        protected INextBackupDateTimeFactory SetupFactory()
        {
            var mockNextBackupDateTimeFactory = new Mock<INextBackupDateTimeFactory>();

            mockNextBackupDateTimeFactory.Setup(x => x.Create(
                It.IsAny<BackupDate>(), It.IsAny<BackupTime>()))
                .Callback<BackupDate, BackupTime>(
                    (d, t) => { ActualBackupDate = d; ActualBackupTime = t; });

            return mockNextBackupDateTimeFactory.Object;
        }
    }
}