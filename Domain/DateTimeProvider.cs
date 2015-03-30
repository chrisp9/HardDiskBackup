using Registrar;
using System;

namespace Domain
{
    public interface IDateTimeProvider 
    {
        DateTime Now { get; }
    }

    [Register(LifeTime.Transient)]
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now { get { return DateTime.Now; } }
    }
}