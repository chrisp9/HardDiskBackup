using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IResultFormatter
    {
        string FormatResult(Result result);
    }

    public class ResultFormatter : IResultFormatter
    {
        public string FormatResult(Result result)
        {
            var stringBuilder = new StringBuilder();

            foreach (var ex in result.Errors)
            {
                stringBuilder.Append(ex.UnderlyingException.Message);
                stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
        }
    }
}
