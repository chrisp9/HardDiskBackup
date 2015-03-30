using Domain.Scheduling;
using Registrar;
using Services.Factories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Scheduling
{
    public interface ISetScheduleModel : IDataErrorInfo
    {
        TimeSpan? Time { get; set; }
        int? DayOfMonth { get; set; }
        DayOfWeek? DayOfWeek { get; set; }
        BackupScheduleType? ScheduleType { get; }
        BackupSchedule CreateSchedule();
        bool IsScheduleValid();
        void SetScheduleType(BackupScheduleType backupScheduleType);
    }

    [Register(LifeTime.SingleInstance)]
    public class SetScheduleModel : ISetScheduleModel
    {
        public TimeSpan? Time { get; set; }
        public int? DayOfMonth 
        {
            get 
            {
                return _dayOfMonth; 
            } 
            set 
            { 
                _dayOfMonth = value;
                _backupScheduleFactory.DayOfMonth = value;
            } 
        }

        public DayOfWeek? DayOfWeek
        {
            get
            {
                return _dayOfWeek;
            }
            set
            {
                _dayOfWeek = value;
                _backupScheduleFactory.DayOfWeek = value;
            }
        }

        public BackupScheduleType? ScheduleType { get; private set; }

        private IBackupScheduleFactory _backupScheduleFactory;

        private int? _dayOfMonth;
        private DayOfWeek? _dayOfWeek;

        public SetScheduleModel(IBackupScheduleFactory factory)
        {
            _backupScheduleFactory = factory;
        }

        public void SetScheduleType(BackupScheduleType backupScheduleType) 
        {
            ScheduleType = backupScheduleType;
        }

        public BackupSchedule CreateSchedule()
        {
            return _backupScheduleFactory.Create(ScheduleType.Value, new Domain.BackupTime(Time.Value));
        }

        public bool IsScheduleValid()
        {
            if (ScheduleType == null) return false;

            switch (ScheduleType)
            {
                case BackupScheduleType.Daily:
                    return ValidateTimeOfDay() == null;
                case BackupScheduleType.Weekly:
                    return ValidateDayOfWeek() == null && ValidateTimeOfDay() == null;
                case BackupScheduleType.Monthly:
                    return ValidateDayOfMonth() == null && ValidateTimeOfDay() == null;
            }

            throw new InvalidOperationException("Unknown schedule type");
        }

        public string Error
        {
            get { return string.Empty; }
        }

        public string this[string columnName]
        {
            get 
            {
                if (columnName == "DayOfMonth")
                    return ValidateDayOfMonth();
                else if (columnName == "DayOfWeek")
                    return ValidateDayOfWeek();
                else if (columnName == "Time")
                    return ValidateTimeOfDay();

                throw new ArgumentException("Invalid columnName: " + columnName);
            }
        }

        private string ValidateDayOfMonth()
        {
            return DayOfMonth.HasValue && DayOfMonth.Value > 0 && DayOfMonth.Value <= 28
                ? null
                : "Day of Month must be between 1 and 28";
        }

        private string ValidateDayOfWeek() 
        {
            return DayOfWeek.HasValue
                ? null
                : "Day of Week must be set";
        }

        private string ValidateTimeOfDay()
        {
            return Time.HasValue
                ? null
                : "Time of Day must be set";
        }
    }
}
