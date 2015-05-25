﻿using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IDialogService
    {
        Task<MessageDialogResult> PresentDialog<T>(string title, string message) where T : MetroWindow;
    }

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
