using Domain;
using Registrar;
using System.Text;

namespace Services
{
    public interface IResultFormatter
    {
        string FormatResult(Result result);
    }

    [Register(LifeTime.Transient)]
    public class ResultFormatter : IResultFormatter
    {
        public string FormatResult(Result result)
        {
            var stringBuilder = new StringBuilder();

            foreach (var ex in result.Errors)
            {
                stringBuilder.Append(ex.UnderlyingException.Message + " at " + ex.Location);
                stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
        }
    }
}
