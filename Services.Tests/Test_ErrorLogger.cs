using Domain.Exceptions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Tests
{
    public class Test_ErrorLogger
    {
        [Test]
        public void When_actionPerformer_raises_OnError_after_subscription_then_the_exception_is_stored_in_the_collection()
        {
            _sut.SubscribeToErrors();
            var exceptionToThrow = new UnauthorizedAccessException();

            _mockSafeActionPerformer.Raise(x => x.OnError += null, new ExceptionEventArgs(exceptionToThrow));
            Assert.Contains(exceptionToThrow, _sut.Errors.ToArray());
        }

        [Test]
        public void No_exception_is_stored_if_OnError_is_raised_before_subscription()
        {
            var exceptionToThrow = new UnauthorizedAccessException();

            _mockSafeActionPerformer.Raise(x => x.OnError += null, new ExceptionEventArgs(exceptionToThrow));
            Assert.IsEmpty(_sut.Errors);
        }

        [Test]
        public void No_exception_is_stored_if_OnError_is_raised_after_unsubscription()
        {
            _sut.SubscribeToErrors();
            _sut.UnsubscribeFromErrors();
            var exceptionToThrow = new UnauthorizedAccessException();

            _mockSafeActionPerformer.Raise(x => x.OnError += null, new ExceptionEventArgs(exceptionToThrow));
            Assert.IsEmpty(_sut.Errors);
        }

        [SetUp]
        public void Setup()
        {
            _mockSafeActionPerformer = new Mock<ISafeActionPerformer>();

            _sut = new ErrorLogger(_mockSafeActionPerformer.Object);
        }

        private Mock<ISafeActionPerformer> _mockSafeActionPerformer;
        private ErrorLogger _sut;
    }
}
