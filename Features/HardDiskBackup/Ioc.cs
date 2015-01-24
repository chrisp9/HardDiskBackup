using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardDiskBackup
{
    public class Ioc
    {
        public static ContainerBuilder ContainerBuilder;
        public static IContainer Container;

        static Ioc()
        {
            ContainerBuilder = new ContainerBuilder();
        }

        public static void Build()
        {
            if(Container == null)
                Container = ContainerBuilder.Build();
        }
    }
}
