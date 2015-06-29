using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Result<T>
    {
        public IReadOnlyCollection<Error> Errors
        {
            get
            {
                return new ReadOnlyCollection<Error>(
                    _exceptions.Select(x => new Error(x)).ToArray());
            }
        }

        public IReadOnlyCollection<Exception> Exceptions
        {
            get
            {
                return new ReadOnlyCollection<Exception>(_exceptions.ToArray());
            }
        }

        public T Value { get; private set; }

        public bool IsSuccess
        {
            get
            {
                return Value != null;
            }
        }

        public bool IsFail
        {
            get
            {
                return Value == null;
            }
        }

        private IEnumerable<Exception> _exceptions;

        private Result(T arg)
        {
            _exceptions = Enumerable.Empty<Exception>();
            Value = arg;
        }

        private Result(params Exception[] exceptions)
        {
            _exceptions = exceptions;
        }

        public Result ToUnit()
        {
            if (IsSuccess)
                return Result.Success();
            else
                return Result.Fail(
                    Errors.Select(x => x.UnderlyingException).ToArray());
        }

        public static Result<T> Success(T arg)
        {
            return new Result<T>(arg);
        }

        public static Result<T> Fail(params Exception[] errors)
        {
            return new Result<T>(errors);
        }

        public static Result<T> Combine(Result<T> result1, Result<T> result2)
        {
            var exceptions1 = result1.Errors.Select(x => x.UnderlyingException);
            var exceptions2 = result2.Errors.Select(x => x.UnderlyingException);

            return new Result<T>(exceptions1.Concat(exceptions2).ToArray());
        }
    }

    // C# sucks... Doesn't have unit...
    public class Result
    {
        public IReadOnlyCollection<Error> Errors
        {
            get
            {
                return new ReadOnlyCollection<Error>(
                    _exceptions.Select(x => new Error(x)).ToArray());
            }
        }

        public IReadOnlyCollection<Exception> Exceptions
        {
            get
            {
                return new ReadOnlyCollection<Exception>(_exceptions.ToArray());
            }
        }

        public bool IsSuccess
        {
            get
            {
                return !_exceptions.Any();
            }
        }

        public bool IsFail
        {
            get
            {
                return _exceptions.Any();
            }
        }

        private Exception[] _exceptions;

        public Result()
        {
            _exceptions = Enumerable.Empty<Exception>().ToArray();
        }

        public Result(params Exception[] errors)
        {
            _exceptions = errors;
        }
        
        public static Result Success()
        {
            return new Result();
        }

        public static Result Fail(params Exception[] errors)
        {
            return new Result(errors);
        }

        public static Result Combine<T, U>(Result<T> result1, Result<U> result2)
        {
            var exceptions1 = result1.Errors.Select(x => x.UnderlyingException);
            var exceptions2 = result2.Errors.Select(x => x.UnderlyingException);

            return new Result(exceptions1.Concat(exceptions2).ToArray());
        }

        public static Result Combine(Result result1, Result result2)
        {
            var exceptions1 = result1.Errors.Select(x => x.UnderlyingException);
            var exceptions2 = result2.Errors.Select(x => x.UnderlyingException);

            return new Result(exceptions1.Concat(exceptions2).ToArray());
        }

    }
}
