using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Tests
{
    public class Test_SafeActionPerformer
    {
        [Test]
        public void Event_is_raised_when_exception_is_thrown_by_action_through_InvokeSafely()
        {
            Action action = () => { throw new UnauthorizedAccessException(); };

            _sut.OnError += (_, __) => { Assert.Pass(); };

            _sut.InvokeSafely(action);
        }

        [Test]
        public void Event_is_raised_when_exception_is_thrown_by_action_through_SafeGet()
        {
            Action action = () => { throw new UnauthorizedAccessException(); };

            _sut.OnError += (_, __) => { Assert.Pass(); };

            var fun = new Func<IEnumerable<object>>(() => { throw new UnauthorizedAccessException(); });

            _sut.SafeGet(fun);
        }

        [SetUp]
        public void Setup()
        {
            _sut = new SafeActionPerformer();
        }

        private SafeActionPerformer _sut;
    }
}
