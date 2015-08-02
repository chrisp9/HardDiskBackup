using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Domain
{
    public class Result<T>
    {
        public IReadOnlyCollection<Error> Errors
        {
            get
            {
                return new ReadOnlyCollection<Error>(
                    _exceptions.ToArray());
            }
        }

        public T Value { get; private set; }

        public virtual bool IsSuccess
        {
            get
            {
                return Value != null;
            }
        }

        public virtual bool IsFail
        {
            get
            {
                return Value == null;
            }
        }

        protected IEnumerable<Error> _exceptions;

        private Result(T arg)
        {
            _exceptions = Enumerable.Empty<Error>();
            Value = arg;
        }

        protected Result(params Error[] exceptions)
        {
            _exceptions = exceptions;
        }

        public Result ToUnit()
        {
            if (IsSuccess)
                return Result.Success();
            else
                return Result.Fail(
                    Errors.ToArray());
        }

        public static Result<T> Success(T arg)
        {
            return new Result<T>(arg);
        }

        public static Result<T> Fail(params Error[] errors)
        {
            return new Result<T>(errors);
        }

        public static Result<T> Combine(Result<T> result1, Result<T> result2)
        {
            var exceptions1 = result1.Errors;
            var exceptions2 = result2.Errors;

            return new Result<T>(exceptions1.Concat(exceptions2).ToArray());
        }
    }

    // C# sucks... Doesn't have unit...
    public class Result : Result<Unit>
    {
        public override bool IsSuccess
        {
            get
            {
                return !_exceptions.Any();
            }
        }

        public override bool IsFail
        {
            get
            {
                return _exceptions.Any();
            }
        }

        private Result(params Error[] errors)
        {
            _exceptions = errors;
        } 

        public static Result Success()
        {
            return new Result();
        }

        public static new Result Fail(params Error[] errors)
        {
            return new Result(errors);
        }

        public static Result Combine(Result result1, Result result2)
        {
            var exceptions1 = result1.Errors;
            var exceptions2 = result2.Errors;

            return new Result(exceptions1.Concat(exceptions2).ToArray());
        }

    }
}
