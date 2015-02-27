using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Domain;
using Services;
using Services.Persistence;

namespace HardDiskBackup
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    /// 

    public class MainWindowViewModel : ViewModelBase
    {
        public bool IsFirstStartup
        {
            get
            {
                return !_persistedOptions.FileExists;
            }
        }

        private IDateTimeProvider _dateTimeProvider;
        private IJsonSerializer _persistedOptions;

        public MainWindowViewModel(
            IDateTimeProvider dateTimeProvider,
            IJsonSerializer jsonSerializer)
        {
            _dateTimeProvider = dateTimeProvider;
            _persistedOptions = jsonSerializer;
        }
    }
}
