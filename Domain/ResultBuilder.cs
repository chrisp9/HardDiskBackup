using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ResultBuilder
    {
        private IList<Exception> _exceptions;

        private ResultBuilder()
        {
            _exceptions = new List<Exception>();
        }

        public static ResultBuilder Create()
        {
            return new ResultBuilder();
        }

        public void Add(Exception e)
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
