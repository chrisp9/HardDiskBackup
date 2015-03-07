using Domain.Scheduling;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestHelpers;

namespace Domain.Tests.BackupSchedule
{
    public class Test_DailyBackupSchedule : BackupScheduleTestBase
    {
        [TestCase("2015/03/03 10:29:42", "2015/03/03", "10:29:43")]
        [TestCase("2015/03/03 10:29:44", "2015/03/04", "10:29:43")]
        [TestCase("2015/03/03 00:00:01", "2015/03/03", "23:59:59")]
        public void daily_next_backup_datetime_is_calculated_correctly(
            string currentDate,
            string expectedDate,
            string expectedTime)
        {
            // Arrange
            var mockNextBackupDateTimeFactory = SetupFactory();

            var expectedBackupTime = new BackupTime(TimeSpan.Parse(expectedTime));
            var expectedBackupDate = new BackupDate(DateTime.Parse(expectedDate));

            var sut = SetupSut(
                nextBackupDateTimeFactory: mockNextBackupDateTimeFactory,
                currentDateTime: new FakeDateTimeProvider(currentDate),
                backupTime: new BackupTime(TimeSpan.Parse(expectedTime)));

            // Act
            sut.CalculateNextBackupDateTime();

            // Assert
            Assert.AreEqual(expectedBackupDate, ActualBackupDate);
            Assert.AreEqual(expectedBackupTime, ActualBackupTime);
        }

        private DailyBackupSchedule SetupSut(
            INextBackupDateTimeFactory nextBackupDateTimeFactory = null,
            IDateTimeProvider currentDateTime = null,
            BackupTime backupTime = null)
        {
            return new DailyBackupSchedule(
                nextBackupDateTimeFactory ?? Mock.Of<INextBackupDateTimeFactory>(),
                currentDateTime ?? Mock.Of<IDateTimeProvider>(),
                backupTime ?? new BackupTime(TimeSpan.MinValue));
        }
    }
}
