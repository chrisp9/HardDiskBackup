using System;

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