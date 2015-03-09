using Domain;
using HardDiskBackup.ViewModel;
using Moq;
using NUnit.Framework;
using Services.Factories;
using Services.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void Setting_day_of_month_sets_day_of_month_on_model()
        {
            _sut.DayOfWeek = 5;

            _mockSetScheduleModel
                .VerifySet(x => x.DayOfWeek = DayOfWeek.Friday);
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
