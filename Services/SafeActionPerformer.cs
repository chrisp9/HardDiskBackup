using Domain.Exceptions;
using Registrar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public interface ISafeActionPerformer
    {
        event Services.SafeActionPerformer.OnErrorEventHandler OnError;

        void InvokeSafely(Action action);

        IEnumerable<T> SafeGet<T>(Func<IEnumerable<T>> function);
    }

    [Register(LifeTime.SingleInstance)]
    public class SafeActionPerformer : ISafeActionPerformer
    {
        private readonly object _locker = new Object();

        public delegate void OnErrorEventHandler(object e, ExceptionEventArgs args);

        public event OnErrorEventHandler OnError;

        public void InvokeSafely(Action action)
        {
            try
            {
                lock (_locker)
                {
                    action();
                }
            }
            catch (Exception e)
            {
                var evt = OnError;
                if (evt != null)
                    evt(this, new ExceptionEventArgs(e));
            }
        }

        /// <summary>
        /// Invokes a function which returns an IEnumerable
        /// Returns IEnumerable<T> if successful or Enumerable.Empty
        /// if any exception was encountered
        /// </summary>
        /// <param name="function">The function to execute</param>
        /// <returns>An enumerable</returns>
        public IEnumerable<T> SafeGet<T>(Func<IEnumerable<T>> fun)
        {
            try
            {
                lock (_locker)
                {
                    return fun();
                }
            }
            catch (Exception e)
            {
                OnError(this, new ExceptionEventArgs(e));
                return Enumerable.Empty<T>();
            }
        }
    }
}