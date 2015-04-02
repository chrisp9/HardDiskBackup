using Registrar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ISafeActionPerformer
    {
        void InvokeSafely(Action action, Action onFailure);
        IEnumerable<T> SafeGet<T>(Func<IEnumerable<T>> function);
    }

    [Register(LifeTime.Transient)]
    public class SafeActionPerformer : ISafeActionPerformer
    {
        private IEnumerable<Exception> _exceptions;

        /// <summary>
        /// Invokes an action safely, invoking the onFailure action
        /// if an exception was encountered whilst performing the action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="onFailure"></param>
        public void InvokeSafely(Action action, Action onFailure)
        {
            try
            {
                action();
            } 
            catch(Exception e)
            {
                onFailure();
            }
        }

        /// <summary>
        /// Invokes a function which returns an IEnumerable
        /// Returns IEnumerable<T> if successful or Enumerable.Empty
        /// if any exception was encountered
        /// </summary>
        /// <param name="function">The function to execute</param>
        /// <returns>An enumerable</returns>
        public IEnumerable<T> SafeGet<T>(Func<IEnumerable<T>> function)
        {
            try
            {
                return function();
            }
            catch(Exception e)
            {
                return Enumerable.Empty<T>();
            }
        }
    }
}