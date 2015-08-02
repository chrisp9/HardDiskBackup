using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Registrar;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
    public interface IDialogService
    {
        Task<MessageDialogResult> PresentDialog<T>(string title, string message) where T : MetroWindow;
    }

    [Register(LifeTime.Transient)]
    public class DialogService : IDialogService
    {
        private ICurrentApplication _currentApplication;

        public DialogService(ICurrentApplication currentApplication)
        {
            _currentApplication = currentApplication;
        }

        public async Task<MessageDialogResult> PresentDialog<T>(string title, string message) where T : MetroWindow
        {
            var window = _currentApplication.Windows.OfType<T>().FirstOrDefault();
            return await DialogManager.ShowMessageAsync(window, title, message, MessageDialogStyle.AffirmativeAndNegative);
        }
    }
}
