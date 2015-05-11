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
        private Func<T> _viewModelFactory;
        private Func<U> viewFactory;

        public WindowPresenter(
            Func<T> mainWindowViewModelFactory,
            Func<U> mainWindowFactory)
        {
            _viewModelFactory = mainWindowViewModelFactory;
            viewFactory = mainWindowFactory;
        }

        public IView Present()
        {
            var viewModel = _viewModelFactory();
            var view = viewFactory();

            view.DataContext = viewModel;
            return view;
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels?
        }
    }
}