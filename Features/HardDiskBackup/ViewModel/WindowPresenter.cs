using GalaSoft.MvvmLight;
using Services;
using System;

namespace HardDiskBackup.ViewModel
{
    public interface IWindowPresenter<T, U>
    {
        IView Present();
    }

    public class WindowPresenter<TViewModel, TView> : IWindowPresenter<TViewModel, TView>
        where TViewModel : ViewModelBase
        where TView : IView
    {
        private Func<TViewModel> _viewModelFactory;
        private Func<TView> viewFactory;

        public WindowPresenter(
            Func<TViewModel> mainWindowViewModelFactory,
            Func<TView> mainWindowFactory)
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