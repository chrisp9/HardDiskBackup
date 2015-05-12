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
    [Register(LifeTime.SingleInstance)]
    public class SetScheduleViewModel : ViewModelBase, IDataErrorInfo
    {
        public int? DayOfMonth 
        {
            get { return _setScheduleModel.DayOfMonth; }
            set 
            { 
                if(value != null)
                    _setScheduleModel.DayOfMonth = value; 
            }
        }

        public int? DayOfWeek
        {
            get { return (int?) _setScheduleModel.DayOfWeek; }
            set 
            { 
                if(value != null)
                    _setScheduleModel.DayOfWeek = (DayOfWeek?) value; 
            }
        }

        public DateTime? TimeOfDay 
        {
            get 
            {
                if (_setScheduleModel.Time == null)
                    return null;

                return DateTime.MinValue.Add(_setScheduleModel.Time.Value);
            }
            set 
            {
                if (value.HasValue)
                    _setScheduleModel.Time = value.Value.TimeOfDay;
            } 
        }

        public bool IsDaily 
        {
            get { return _setScheduleModel.ScheduleType == BackupScheduleType.Daily; }
            set { _isDaily = value; if(value) _setScheduleModel.SetScheduleType(BackupScheduleType.Daily); }
        }

        public bool IsWeekly
        {
            get { return _setScheduleModel.ScheduleType == BackupScheduleType.Weekly; }
            set { _isWeekly = value; if(value) _setScheduleModel.SetScheduleType(BackupScheduleType.Weekly); }
        }

        public bool IsMonthly
        {
            get { return _setScheduleModel.ScheduleType == BackupScheduleType.Monthly; }
            set { _isMonthly = value; if(value) _setScheduleModel.SetScheduleType(BackupScheduleType.Monthly); }
        }

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
            _setScheduleModel.PropertyChanged += (o, e) =>
            {
                RaisePropertyChanged<bool>();
                RaisePropertyChanged<DayOfWeek>();
                RaisePropertyChanged<DateTime?>();
                RaisePropertyChanged<int?>();
            };
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