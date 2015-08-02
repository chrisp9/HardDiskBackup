using System;

namespace Domain.Exceptions
{
    public class Error
    {
        public Exception UnderlyingException { get; private set; }
        public string Location { get; private set; }

        public Error(Exception e, string location)
        {
            UnderlyingException = e;
            Location = location;
        }
    }
}
