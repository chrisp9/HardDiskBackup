/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:HardDiskBackup"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using Autofac;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace HardDiskBackup.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator // TODO: I don't like this class.
    {
        //TODO: Remove this.
        public ViewModelLocator()
        {
            Ioc.ContainerBuilder.RegisterType<MainWindowViewModel>();
            Ioc.ContainerBuilder.RegisterType<FirstRunViewModel>();
            Ioc.ContainerBuilder.RegisterType<SetScheduleViewModel>();
            Ioc.Build();
        }

        public MainWindowViewModel Main
        {
            get
            {
                return Ioc.Container.Resolve<MainWindowViewModel>();
            }
        }

        public FirstRunViewModel FirstRun
        {
            get
            {
                return Ioc.Container.Resolve<FirstRunViewModel>();
            }
        }

        public SetScheduleViewModel SetSchedule
        {
            get
            {
                return Ioc.Container.Resolve<SetScheduleViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}