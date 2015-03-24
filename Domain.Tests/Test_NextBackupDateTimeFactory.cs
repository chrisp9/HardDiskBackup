using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tests
{
    public class Test_NextBackupDateTimeFactory
    {
        private NextBackupDateTimeFactory _sut =
            new NextBackupDateTimeFactory();

        [TestCase("2015/03/17", "20:00:00")]
        public void NextBackupDateTimeFactory_returns_correct_dateTime(string date, string time)
        {
            var d = DateTime.Parse(date);
            var t = TimeSpan.Parse(time);

            var backupDate = new BackupDate(d);
            var backupTime = new BackupTime(t);

            var expected = date + " " + time;
            var result = _sut.Create(backupDate, backupTime);
            var resultString = result.DateTime.ToString("yyyy/MM/dd HH:mm:ss");

            Assert.AreEqual(expected, resultString);
        }

    }
}
