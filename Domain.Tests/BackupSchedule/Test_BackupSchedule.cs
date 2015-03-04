using Domain.BackupSchedule;
using Moq;
using NUnit.Framework;
using Services.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper;
using TestHelpers;

namespace Domain.Tests.BackupSchedule
{
    public class Test_BackupSchedule
    {
        [TestCase(DayOfWeek.Monday, "2015/03/03", "2015/03/09", "10:29:43")]
        [TestCase(DayOfWeek.Tuesday, "2015/03/03", "2015/03/10", "11:29:42")]
        [TestCase(DayOfWeek.Wednesday, "2015/03/03", "2015/03/04", "23:29:42")]
        [TestCase(DayOfWeek.Thursday, "2015/03/03", "2015/03/05", "00:29:42")]
        [TestCase(DayOfWeek.Friday, "2015/03/03", "2015/03/06", "00:00:59")]
        [TestCase(DayOfWeek.Saturday, "2015/03/03", "2015/03/07", "00:00:01")]
        [TestCase(DayOfWeek.Sunday, "2015/03/03", "2015/03/08", "12:29:42")]
        public void next_backup_datetime_is_calculated_correctly(
            DayOfWeek next,
            string currentDate,
            string expectedDate,
            string expectedTime)
        {
            BackupDate actualBackupDate = null;
            BackupTime actualBackupTime = null;

            var mockNextBackupDateTimeFactory = new Mock<INextBackupDateTimeFactory>();

            mockNextBackupDateTimeFactory.Setup(x => x.Create(
                It.IsAny<BackupDate>(), It.IsAny<BackupTime>()))
                .Callback<BackupDate, BackupTime>(
                    (d, t) => { actualBackupDate = d; actualBackupTime = t; });
               
            var expectedBackupTime = new BackupTime(TimeSpan.Parse(expectedTime));
            var expectedBackupDate = new BackupDate(DateTime.Parse(expectedDate));

            var sut = SetupSut(
                dayOfWeek: next, 
                nextBackupDateTimeFactory: mockNextBackupDateTimeFactory.Object,
                currentDateTime: new FakeDateTimeProvider(currentDate),
                backupTime: new BackupTime(TimeSpan.Parse(expectedTime))); 

            sut.CalculateNextBackupDateTime();

            var y = expectedBackupDate.Equals(actualBackupDate);

            Assert.AreEqual(expectedBackupDate, actualBackupDate);
            Assert.AreEqual(expectedBackupTime, actualBackupTime);
        }

        private WeeklyBackupSchedule SetupSut(
            DayOfWeek dayOfWeek = DayOfWeek.Monday,
            INextBackupDateTimeFactory nextBackupDateTimeFactory = null,
            IDateTimeProvider currentDateTime = null,
            BackupTime backupTime = null)
        {
            return new WeeklyBackupSchedule(
                nextBackupDateTimeFactory ?? Mock.Of<INextBackupDateTimeFactory>(),
                currentDateTime ?? Mock.Of<IDateTimeProvider>(),
                dayOfWeek,
                backupTime ?? new BackupTime(TimeSpan.MinValue));
        }
    }
}
