using Domain.Scheduling;
using Moq;
using NUnit.Framework;
using System;
using TestHelpers;

namespace Domain.Tests.BackupSchedule
{
    public class Test_MonthlyBackupSchedule : BackupScheduleTestBase
    {
        [TestCase(1, "2015/03/03", "2015/04/01", "10:29:43")]
        [TestCase(2, "2015/03/03", "2015/04/02", "11:29:42")]
        [TestCase(3, "2015/03/03 23:29:41", "2015/03/03", "23:29:42")]
        [TestCase(3, "2015/03/03 23:29:43", "2015/04/03", "23:29:42")]
        [TestCase(4, "2015/03/03", "2015/03/04", "23:29:42")]
        [TestCase(5, "2015/03/03", "2015/03/05", "23:29:42")]
        [TestCase(2, "2015/03/03", "2015/04/02", "23:29:42")]
        // [TestCase(DayOfWeek.Thursday, "2015/03/03", "2015/03/05", "00:29:42")]
        // [TestCase(DayOfWeek.Friday, "2015/03/03", "2015/03/06", "00:00:59")]
        // [TestCase(DayOfWeek.Saturday, "2015/03/03", "2015/03/07", "00:00:01")]
        // [TestCase(DayOfWeek.Sunday, "2015/03/03", "2015/03/08", "12:29:42")]
        public void monthly_next_backup_datetime_is_calculated_correctly(
            int nextDay,
            string currentDate,
            string expectedDate,
            string expectedTime)
        {
            // Arrange
            var mockNextBackupDateTimeFactory = SetupFactory();

            var expectedBackupTime = new BackupTime(TimeSpan.Parse(expectedTime));
            var expectedBackupDate = new BackupDate(DateTime.Parse(expectedDate));

            var sut = SetupSut(
                dayOfMonth: nextDay,
                nextBackupDateTimeFactory: mockNextBackupDateTimeFactory,
                currentDateTime: new FakeDateTimeProvider(currentDate),
                backupTime: new BackupTime(TimeSpan.Parse(expectedTime)));

            // Act
            sut.CalculateNextBackupDateTime();

            // Assert
            Assert.AreEqual(expectedBackupDate, ActualBackupDate);
            Assert.AreEqual(expectedBackupTime, ActualBackupTime);
        }

        private MonthlyBackupSchedule SetupSut(
            int dayOfMonth = 1,
            INextBackupDateTimeFactory nextBackupDateTimeFactory = null,
            IDateTimeProvider currentDateTime = null,
            BackupTime backupTime = null)
        {
            return new MonthlyBackupSchedule(
                nextBackupDateTimeFactory ?? Mock.Of<INextBackupDateTimeFactory>(),
                currentDateTime ?? Mock.Of<IDateTimeProvider>(),
                dayOfMonth,
                backupTime ?? new BackupTime(TimeSpan.MinValue));
        }
    }
}