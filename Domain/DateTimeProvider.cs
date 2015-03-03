using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper;

namespace Domain
{
    public interface IDateTimeProvider 
    {
        IDateTimeWrap Now { get; }
        IDateTimeWrap UtcNow { get; }
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        public IDateTimeWrap Now { get { return new DateTimeWrap(DateTime.Now); } }
        public IDateTimeWrap UtcNow { get { return new DateTimeWrap(DateTime.UtcNow); } }
    }
}
