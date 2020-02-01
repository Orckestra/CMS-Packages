using System.Text;

namespace Orckestra.Web.Typescript.Classes.Services
{
    public abstract class TypescriptService
    {
        public string ComposeExceptionInfo(string methodName, string taskName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine($"Service: {GetType().Name}.");
            sb.AppendLine($"Method: {methodName}.");
            if (!string.IsNullOrEmpty(taskName))
            {
                sb.AppendLine($"Task: {taskName}.");
            };
            return sb.ToString();
        }
    }
}