using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
