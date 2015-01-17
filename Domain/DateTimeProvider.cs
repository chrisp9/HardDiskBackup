using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IDateTimeProvider 
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now { get { return DateTime.Now; } }
        public DateTime UtcNow { get { return DateTime.UtcNow; } }
    }
}
