using GalaSoft.MvvmLight;
using Services;
using System;

namespace HardDiskBackup.ViewModel
{
    public interface IWindowPresenter<T, U>
    {
        IView Present();
    }

    public class WindowPresenter<T, U> : IWindowPresenter<T, U>
        where T : ViewModelBase 
        where U : IView
    {
        private Func<T> _mainWindowViewModelFactory;
        private Func<U> _mainWindowViewFactory;

        public WindowPresenter(
            Func<T> mainWindowViewModelFactory,
            Func<U> mainWindowFactory)
        {
            _mainWindowViewModelFactory = mainWindowViewModelFactory;
            _mainWindowViewFactory = mainWindowFactory;
        }

        public IView Present()
        {
            var viewModel = _mainWindowViewModelFactory();
            var view = _mainWindowViewFactory();

            view.DataContext = viewModel;
            return view;
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}