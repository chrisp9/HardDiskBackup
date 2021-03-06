﻿using Domain.Scheduling;
using Newtonsoft.Json;
using Registrar;
using Services.Factories;
using System;
using System.ComponentModel;

namespace Services.Scheduling
{
    public interface ISetScheduleModel : IDataErrorInfo, INotifyPropertyChanged
    {
        TimeSpan? Time { get; set; }

        int? DayOfMonth { get; set; }

        DayOfWeek? DayOfWeek { get; set; }

        BackupScheduleType? ScheduleType { get; }

        BackupSchedule CreateSchedule();

        bool IsScheduleValid();

        void SetScheduleType(BackupScheduleType backupScheduleType);

        void Load(ISetScheduleModel setScheduleModel);
    }

    [Register(LifeTime.SingleInstance), JsonObject(MemberSerialization.OptIn)]
    public class SetScheduleModel : ISetScheduleModel
    {
        [JsonProperty]
        public TimeSpan? Time { get; set; }

        [JsonProperty]
        public int? DayOfMonth
        {
            get
            {
                return _dayOfMonth;
            }
            set
            {
                _dayOfMonth = value;

                if (_backupScheduleFactory != null)
                    _backupScheduleFactory.DayOfMonth = value;
            }
        }

        [JsonProperty]
        public DayOfWeek? DayOfWeek
        {
            get
            {
                return _dayOfWeek;
            }
            set
            {
                _dayOfWeek = value;

                if (_backupScheduleFactory != null)
                    _backupScheduleFactory.DayOfWeek = value;
            }
        }

        [JsonProperty]
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

        public void Load(ISetScheduleModel toLoad)
        {
            Time = toLoad.Time;
            DayOfMonth = toLoad.DayOfMonth;
            DayOfWeek = toLoad.DayOfWeek;
            ScheduleType = toLoad.ScheduleType;
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

        public void OnPropertyChanged(string propertyName)
        {
            var evt = PropertyChanged;

            if (evt != null)
                evt(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}