using Domain.Exceptions;
using Registrar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Services
{
    public interface IErrorLogger
    {
        void SubscribeToErrors();

        void UnsubscribeFromErrors();

        IReadOnlyCollection<Exception> Errors { get; }
    }

    [Register(LifeTime.SingleInstance)]
    public class ErrorLogger : IErrorLogger
    {
        private ISafeActionPerformer _safeActionPerformer;
        private List<Exception> _exceptions;
        private object _locker = new Object();

        public IReadOnlyCollection<Exception> Errors
        {
            get
            {
                lock (_locker)
                {
                    return new ReadOnlyCollection<Exception>(_exceptions);
                }
            }
        }

        public ErrorLogger(ISafeActionPerformer safeActionPerformer)
        {
            _safeActionPerformer = safeActionPerformer;
            _exceptions = new List<Exception>();
        }

        public void SubscribeToErrors()
        {
            _safeActionPerformer.OnError += OnError;
        }

        public void UnsubscribeFromErrors()
        {
            _safeActionPerformer.OnError -= OnError;
        }

        private void OnError(object o, ExceptionEventArgs e)
        {
            lock (_locker)
            {
                _exceptions.Add(e.Exception);
            }
        }
    }
}