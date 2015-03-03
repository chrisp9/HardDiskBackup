using Domain;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardDiskBackup.ViewModel
{
    public class SetScheduleViewModel : ViewModelBase
    {
        private IDateTimeProvider _dateTimeProvider;

        public SetScheduleViewModel(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }
    }
}
