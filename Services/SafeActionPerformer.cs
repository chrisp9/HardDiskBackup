using Domain.Exceptions;
using Registrar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ISafeActionLogger
    {
        void InvokeSafely(Action action);
        IEnumerable<T> SafeGet<T>(Func<IEnumerable<T>> function);
    }

    [Register(LifeTime.Transient)]
    public class SafeActionPerformer : ISafeActionLogger
    {
        public delegate void OnErrorEventHandler(object e, ExceptionEventArgs args);
        public event OnErrorEventHandler OnError;

        public void InvokeSafely(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                OnError(this, new ExceptionEventArgs(e));
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
                return fun();
            }
            catch(Exception e)
            {
                OnError(this, new ExceptionEventArgs(e));
                return Enumerable.Empty<T>();
            }
        }
    }
}