using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrar
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Register : System.Attribute
    {
        public LifeTime Scope { get; private set; }

        public Register(LifeTime scope)
        {
            Scope = scope;
        }
    }
}
