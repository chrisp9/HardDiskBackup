using Domain;
using Domain.Scheduling;
using Moq;
using NUnit.Framework;
using Services.Factories;
using System;

namespace Services.Tests
{
    public class Test_BackupScheduleFactory
    {
        [Test]
        public void DailyBackupSchedule_is_returned_for_BackupScheduleType_Daily()
        {
            var sut = SetupSut();

            var result = sut.Create(
                BackupScheduleType.Daily,
                new BackupTime(TimeSpan.Parse("20:00:00")));

            Assert.IsInstanceOf<DailyBackupSchedule>(result);
        }

        [Test]
        public void WeeklyBackupSchedule_is_returned_for_BackupScheduleType_Weekly()
        {
            var sut = SetupSut();
            sut.DayOfWeek = DayOfWeek.Monday;

            var result = sut.Create(
                BackupScheduleType.Weekly,
                new BackupTime(TimeSpan.Parse("20:00:00")));

            Assert.IsInstanceOf<WeeklyBackupSchedule>(result);
        }

        [Test]
        public void MonthlyBackupSchedule_is_returned_for_BackupScheduleType_Monthly()
        {
            var sut = SetupSut();
            sut.DayOfMonth = 28;

            var result = sut.Create(
                BackupScheduleType.Monthly,
                new BackupTime(TimeSpan.Parse("20:00:00")));

            Assert.IsInstanceOf<MonthlyBackupSchedule>(result);
        }

        private BackupScheduleFactory SetupSut(
            INextBackupDateTimeFactory factory = null,
            IDateTimeProvider provider = null)
        {
            return new BackupScheduleFactory(
                    factory ?? Mock.Of<INextBackupDateTimeFactory>(),
                    provider ?? Mock.Of<IDateTimeProvider>()
                );
        }
    }
}