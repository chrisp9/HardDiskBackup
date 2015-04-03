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
        void InvokeSafely(Action action, Action onFailure);
        IEnumerable<T> SafeGet<T>(Func<IEnumerable<T>> function);
        IReadOnlyCollection<Exception> FlushExceptionLog();
    }

    [Register(LifeTime.Transient)]
    public class SafeActionLogger : ISafeActionLogger
    {
        private IList<Exception> _exceptions = new List<Exception>();

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
                _exceptions.Add(e);
                onFailure();
            }
        }

        public void InvokeSafely(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                _exceptions.Add(e);
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
                _exceptions.Add(e);
                return Enumerable.Empty<T>();
            }
        }

        public IReadOnlyCollection<Exception> FlushExceptionLog()
        {
            var coll = new ReadOnlyCollection<Exception>(_exceptions);
            _exceptions.Clear();

            return coll;
        }
    }
}