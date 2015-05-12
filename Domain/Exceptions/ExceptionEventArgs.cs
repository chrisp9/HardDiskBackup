using System;

namespace Domain.Exceptions
{
    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }

        public ExceptionEventArgs(Exception e)
        {
            Exception = e;
        }
    }
}