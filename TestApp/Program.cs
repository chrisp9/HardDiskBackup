using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.PlatformServices;
using Services.Disk;
using Queries;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = new DriveNotifier(NewThreadScheduler.Default, new DriveInfoService(new DriveInfoQuery()));
            x.Subscribe(y => Console.WriteLine(y));
            System.Threading.Thread.Sleep(1000000);
        }
    }
}
