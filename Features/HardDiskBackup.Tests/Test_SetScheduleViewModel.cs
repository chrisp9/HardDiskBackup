using Domain;
using Domain.Scheduling;
using HardDiskBackup.ViewModel;
using Moq;
using NUnit.Framework;
using Services.Factories;
using Services.Scheduling;
using System;

namespace HardDiskBackup.Tests
{
    public class Test_SetScheduleViewModel
    {
        private SetScheduleViewModel _sut;
        private Mock<IDateTimeProvider> _mockDateTimeProvider;
        private Mock<IBackupScheduleFactory> _mockBackupScheduleFactory;
        private Mock<ISetScheduleModel> _mockSetScheduleModel;

        [SetUp]
        public void Setup()
        {
            _mockDateTimeProvider = new Mock<IDateTimeProvider>();
            _mockBackupScheduleFactory = new Mock<IBackupScheduleFactory>();
            _mockSetScheduleModel = new Mock<ISetScheduleModel>();
            SetupSut();
        }

        [Test]
        public void Setting_day_of_week_sets_day_of_week_on_model()
        {
            _sut.DayOfWeek = 5;

            _mockSetScheduleModel
                .VerifySet(x => x.DayOfWeek = DayOfWeek.Friday, Times.Once());
        }

        [Test]
        public void Setting_day_of_month_sets_day_of_month_on_model()
        {
            _sut.DayOfMonth = 28;

            _mockSetScheduleModel
                .VerifySet(x => x.DayOfMonth = 28, Times.Once());
        }

        [Test]
        public void Setting_time_of_day_sets_time_of_day_on_model()
        {
            _sut.TimeOfDay = DateTime.Parse("20:00:00");

            _mockSetScheduleModel
                .VerifySet(x => x.Time = TimeSpan.Parse("20:00:00"), Times.Once());
        }

        [Test]
        public void Is_valid_asks_model_for_validity()
        {
            _sut.IsValid();

            _mockSetScheduleModel
                .Verify(x => x.IsScheduleValid(), Times.Once());
        }

        [Test]
        public void Error_checking_for_DayOfWeek_is_piped_through_to_model()
        {
            var result = _sut["DayOfWeek"];
            _mockSetScheduleModel.Verify(x => x["DayOfWeek"], Times.Once());
        }

        [Test]
        public void Error_checking_for_DayOfMonth_is_piped_through_to_model()
        {
            var result = _sut["DayOfMonth"];
            _mockSetScheduleModel.Verify(x => x["DayOfMonth"], Times.Once());
        }

        [Test]
        public void Error_checking_for_TimeOfDay_is_piped_through_to_model()
        {
            var result = _sut["TimeOfDay"];
            _mockSetScheduleModel.Verify(x => x["Time"], Times.Once());
        }

        [Test]
        public void Setting_daily_sets_correct_BackupScheduleType_on_model()
        {
            _sut.IsDaily = true;
            _mockSetScheduleModel.Verify(x => x.SetScheduleType(BackupScheduleType.Daily), Times.Once());
        }

        [Test]
        public void Setting_weekly_sets_correct_BackupScheduleType_on_model()
        {
            _sut.IsWeekly = true;
            _mockSetScheduleModel.Verify(x => x.SetScheduleType(BackupScheduleType.Weekly), Times.Once());
        }

        [Test]
        public void Setting_monthly_sets_correct_BackupScheduleType_on_model()
        {
            _sut.IsMonthly = true;
            _mockSetScheduleModel.Verify(x => x.SetScheduleType(BackupScheduleType.Monthly), Times.Once());
        }

        [Test]
        public void Unsetting_daily_sets_correct_BackupScheduleType_on_model()
        {
            _sut.IsDaily = false;
            _mockSetScheduleModel.Verify(x => x.SetScheduleType(BackupScheduleType.Daily), Times.Never());
        }

        [Test]
        public void Unsetting_weekly_sets_correct_BackupScheduleType_on_model()
        {
            _sut.IsWeekly = false;
            _mockSetScheduleModel.Verify(x => x.SetScheduleType(BackupScheduleType.Weekly), Times.Never());
        }

        [Test]
        public void Unsetting_monthly_sets_correct_BackupScheduleType_on_model()
        {
            _sut.IsMonthly = false;
            _mockSetScheduleModel.Verify(x => x.SetScheduleType(BackupScheduleType.Monthly), Times.Never());
        }

        [Test]
        public void CreateSchedule_asks_model_for_schedule()
        {
            _sut.CreateSchedule();
            _mockSetScheduleModel.Verify(x => x.CreateSchedule(), Times.Once());
        }

        private void SetupSut()
        {
            _sut = new SetScheduleViewModel(
                _mockDateTimeProvider.Object,
                _mockBackupScheduleFactory.Object,
                _mockSetScheduleModel.Object
                );
        }
    }
}