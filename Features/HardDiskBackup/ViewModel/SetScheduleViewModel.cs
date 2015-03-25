using Domain;
using Domain.Scheduling;
using GalaSoft.MvvmLight;
using Registrar;
using Services.Factories;
using Services.Scheduling;
using System;
using System.ComponentModel;

namespace HardDiskBackup.ViewModel
{
    [Register(Scope.SingleInstance)]
    public class SetScheduleViewModel : ViewModelBase, IDataErrorInfo
    {
        public int? DayOfMonth 
        {
            get { return _dayOfMonth; }
            set 
            { 
                _dayOfMonth = value; 
                if(_dayOfMonth != null)
                    _setScheduleModel.DayOfMonth = _dayOfMonth; 
            }
        }

        public int? DayOfWeek
        {
            get { return _dayOfWeek; }
            set 
            { 
                _dayOfWeek = value; 
                if(_dayOfWeek != null)
                    _setScheduleModel.DayOfWeek = (DayOfWeek?) _dayOfWeek; 
            }
        }

        public DateTime? TimeOfDay 
        { 
            get { return _timeOfDay; }
            set 
            { 
                _timeOfDay = value; 
                if(_timeOfDay.HasValue)
                    _setScheduleModel.Time = _timeOfDay.Value.TimeOfDay; 
            } 
        }

        public bool IsDaily 
        { 
            get { return _isDaily; }
            set { _isDaily = value; if(value) _setScheduleModel.SetScheduleType(BackupScheduleType.Daily); }
        }

        public bool IsWeekly
        {
            get { return _isWeekly; }
            set { _isWeekly = value; if(value) _setScheduleModel.SetScheduleType(BackupScheduleType.Weekly); }
        }

        public bool IsMonthly
        {
            get { return _isMonthly; }
            set { _isMonthly = value; if(value) _setScheduleModel.SetScheduleType(BackupScheduleType.Monthly); }
        }

        private int? _dayOfMonth;
        private int? _dayOfWeek;
        private DateTime? _timeOfDay;

        private bool _isDaily;
        private bool _isWeekly;
        private bool _isMonthly;

        private IDateTimeProvider _dateTimeProvider;
        private IBackupScheduleFactory _backupScheduleFactory;
        private ISetScheduleModel _setScheduleModel;

        public SetScheduleViewModel(
            IDateTimeProvider dateTimeProvider, 
            IBackupScheduleFactory backupScheduleFactory,
            ISetScheduleModel setScheduleModel)
        {
            _dateTimeProvider = dateTimeProvider;
            _backupScheduleFactory = backupScheduleFactory;
            _setScheduleModel = setScheduleModel;
        }

        public BackupSchedule CreateSchedule()
        {
            return _setScheduleModel.CreateSchedule();
        }

        public string Error
        {
            get { return string.Empty; }
        }

        public bool IsValid()
        {
            return _setScheduleModel.IsScheduleValid();
        }

        public string this[string columnName]
        {
            get 
            {
                switch (columnName)
                {
                    case "DayOfWeek":
                        return _setScheduleModel["DayOfWeek"];
                    case "DayOfMonth":
                        return _setScheduleModel["DayOfMonth"];
                    case "TimeOfDay":
                        return _setScheduleModel["Time"];
                    default:
                        throw new ArgumentException("Invalid ColumnName");
                }
            }
        }
    }
}