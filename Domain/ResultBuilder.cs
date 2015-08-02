using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Exceptions;

namespace Domain
{
    public class ResultBuilder
    {
        private IList<Error> _exceptions;

        private ResultBuilder()
        {
            _exceptions = new List<Error>();
        }

        public static ResultBuilder Create()
        {
            return new ResultBuilder();
        }

        public void Add(Error e)
        {
            _exceptions.Add(e);
        }

        public Result Build()
        {
            return _exceptions.Any()
                ? Result.Fail(_exceptions.ToArray())
                : Result.Success();
        }
    }
}
