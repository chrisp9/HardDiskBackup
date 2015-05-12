using Queries;
using Services.Disk;
using System.Reactive.Concurrency;

namespace TestApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var x = new DriveNotifier(NewThreadScheduler.Default, new DriveInfoService(new DriveInfoQuery()));
            //x.Subscribe(y => Console.WriteLine(y));
            // System.Threading.Thread.Sleep(1000000);
        }
    }
}