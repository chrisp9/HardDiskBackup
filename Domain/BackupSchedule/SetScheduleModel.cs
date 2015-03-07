using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.BackupSchedule
{
    public interface ISetScheduleModel
    {
        TimeSpan? Time { get; set; }
        int? DayOfMonth { get; set; }
        DayOfWeek? DayOfWeek { get; set; }

        bool IsScheduleValid();
    }

    public class SetScheduleModel : ISetScheduleModel, IDataErrorInfo
    {
        public TimeSpan? Time { get; set; }
        public int? DayOfMonth { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }

        public BackupScheduleType ScheduleType { get; private set; }

        public SetScheduleModel()
        {
        }

        public void SetScheduleType(BackupScheduleType backupScheduleType) 
        {
            ScheduleType = backupScheduleType;
        }

        public bool IsScheduleValid()
        {
            if (ScheduleType == BackupScheduleType.Daily)
                return ValidateTimeOfDay() == null;
            if (ScheduleType == BackupScheduleType.Weekly)
                return ValidateDayOfWeek() == null && ValidateTimeOfDay() == null;
            if (ScheduleType == BackupScheduleType.Monthly)
                return ValidateDayOfMonth() == null && ValidateTimeOfDay() == null;

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
                else if (columnName == "TimeOfDay")
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
