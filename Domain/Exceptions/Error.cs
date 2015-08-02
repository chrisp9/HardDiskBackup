using System;

namespace Domain.Exceptions
{
    public class Error
    {
        public Exception UnderlyingException { get; private set; }

        public Error(Exception e)
        {
            UnderlyingException = e;
        }
    }
}
