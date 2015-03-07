using Domain;
using Domain.Scheduling;
using Moq;
using NUnit.Framework;
using Services.Factories;
using Services.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Tests
{
    public class Test_SetScheduleModel
    {
        private Mock<IBackupScheduleFactory> _mockBackupScheduleFactory;
        private SetScheduleModel _sut;
        private TimeSpan _backupTimeSpan;
        private BackupTime _backupTime;

        [SetUp]
        public void Setup()
        {
            _mockBackupScheduleFactory = new Mock<IBackupScheduleFactory>();
            _sut = new SetScheduleModel(_mockBackupScheduleFactory.Object);
            _backupTimeSpan = TimeSpan.Parse("20:00:00");
            _backupTime = new BackupTime(_backupTimeSpan);
        }

        // ScheduleIsValid

        [TestCase(BackupScheduleType.Daily)]
        [TestCase(BackupScheduleType.Weekly)]
        [TestCase(BackupScheduleType.Monthly)]
        public void Factory_is_called_with_correct_arguments(BackupScheduleType backupScheduleType)
        {
            _sut.SetScheduleType(backupScheduleType);
            _sut.DayOfWeek = (DayOfWeek?) DayOfWeek.Monday;
            _sut.Time = _backupTimeSpan;

            var result =_sut.CreateSchedule();

            _mockBackupScheduleFactory.Verify(
                x => x.Create(backupScheduleType, _backupTime), Times.Once());
        }

        [Test]
        public void Schedule_is_valid_for_weekly_when_a_day_and_time_are_specified()
        {
            _sut.SetScheduleType(BackupScheduleType.Weekly);
            _sut.DayOfWeek = (DayOfWeek?) DayOfWeek.Tuesday;
            _sut.Time = _backupTimeSpan;

            var result = _sut.IsScheduleValid();

            Assert.IsTrue(result);
        }

        [Test]
        public void Schedule_is_invalid_for_weekly_when_time_is_not_specified()
        {
            _sut.SetScheduleType(BackupScheduleType.Weekly);
            _sut.DayOfWeek = (DayOfWeek?) DayOfWeek.Monday;
            var result = _sut.IsScheduleValid();

            Assert.IsFalse(result);
        }

        [Test]
        public void Schedule_is_invalid_for_weekly_when_day_is_not_specified()
        {
            _sut.SetScheduleType(BackupScheduleType.Weekly);
            _sut.Time = _backupTimeSpan;
            var result = _sut.IsScheduleValid();

            Assert.IsFalse(result);
        }

        [Test]
        public void Schedule_is_valid_for_daily_when_time_is_specified()
        {
            _sut.SetScheduleType(BackupScheduleType.Daily);
            _sut.Time = _backupTimeSpan;

            var result = _sut.IsScheduleValid();

            Assert.IsTrue(result);
        }

        [Test]
        public void Schedule_is_invalid_for_daily_when_time_is_not_specified()
        {
            _sut.SetScheduleType(BackupScheduleType.Daily);
            var result = _sut.IsScheduleValid();

            Assert.IsFalse(result);
        }

        [Test]
        public void Schedule_is_valid_for_monthly_when_day_and_time_are_specified()
        {
            _sut.SetScheduleType(BackupScheduleType.Monthly);
            _sut.Time = _backupTimeSpan;
            _sut.DayOfMonth = 28;
               
            var result = _sut.IsScheduleValid();

            Assert.IsTrue(result);
        }

        [TestCase(29)]
        [TestCase(0)]
        public void Schedule_is_invalid_for_monthly_when_day_is_out_of_bounds(int day)
        {
            _sut.SetScheduleType(BackupScheduleType.Monthly);
            _sut.Time = _backupTimeSpan;
            _sut.DayOfMonth = day;

            var result = _sut.IsScheduleValid();

            Assert.IsFalse(result);
        }

        [Test]
        public void Schedule_is_invalid_for_monthly_when_day_is_not_specified()
        {
            _sut.SetScheduleType(BackupScheduleType.Monthly);
            _sut.Time = _backupTimeSpan;
            var result = _sut.IsScheduleValid();
            
            Assert.IsFalse(result);
        }

        [Test]
        public void Schedule_is_invalid_for_monthly_when_time_is_not_specified()
        {
            _sut.SetScheduleType(BackupScheduleType.Monthly);
            _sut.DayOfMonth = 1;
            var result = _sut.IsScheduleValid();

            Assert.IsFalse(result);
        }

        [TestCase(BackupScheduleType.Daily)]
        [TestCase(BackupScheduleType.Weekly)]
        [TestCase(BackupScheduleType.Monthly)]
        public void Schedule_is_invalid_when_day_and_time_are_not_specified(BackupScheduleType backupScheduleType)
        {
            _sut.SetScheduleType(backupScheduleType);
            var result = _sut.IsScheduleValid();

            Assert.IsFalse(result);
        }

        [Test]
        public void Schedule_is_invalid_before_a_schedule_type_is_selected()
        {
            var result = _sut.IsScheduleValid();

            Assert.IsFalse(result);
        }
        // **************************
        // ****** Validation ********
        // **************************

        [Test]
        public void No_validation_error_for_valid_DayOfWeek()
        {
            _sut.DayOfWeek = (DayOfWeek?)DayOfWeek.Tuesday;
            Assert.IsNull(_sut["DayOfWeek"]);
        }

        [Test]
        public void Validation_error_if_DayOfWeek_has_not_been_set()
        {
            var result = _sut["DayOfWeek"];
            Assert.AreEqual("Day of Week must be set",result);
        }

        [Test]
        public void No_validation_error_for_valid_DayOfMonth()
        {
            _sut.DayOfMonth = 1;
            Assert.IsNull(_sut["DayOfMonth"]);
        }

        [Test]
        public void Validation_error_if_DayOfMonth_has_not_been_set()
        {
            Assert.AreEqual("Day of Month must be between 1 and 28", _sut["DayOfMonth"]);
        }

        [Test]
        public void Validation_error_if_DayOfMonth_is_invalid()
        {
            _sut.DayOfMonth = 29;
            Assert.AreEqual("Day of Month must be between 1 and 28", _sut["DayOfMonth"]);
        }

        [Test]
        public void No_validation_error_for_valid_TimeOfDay()
        {
            _sut.Time = TimeSpan.Parse("21:00:00");
            Assert.IsNull(_sut["Time"]);
        }

        [Test]
        public void Validation_error_if_TimeOfDay_has_not_been_set()
        {
            Assert.AreEqual("Time of Day must be set", _sut["Time"]);
        }
    }
}
