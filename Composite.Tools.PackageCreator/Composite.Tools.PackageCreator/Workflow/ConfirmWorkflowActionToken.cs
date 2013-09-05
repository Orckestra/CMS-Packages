using System;
using System.Text;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.Serialization;

namespace Composite.Tools.PackageCreator.Workflow
{
    public class ConfirmWorkflowActionToken : WorkflowActionToken
    {
        public ConfirmWorkflowActionToken(string confirmMessage, Type actionTokenType) :
            base(typeof(ConfirmPackageWorkflow), new PermissionType[] { PermissionType.Administrate })
        {
            StringBuilder sb = new StringBuilder();
            StringConversionServices.SerializeKeyValuePair(sb, "ConfirmMessage", confirmMessage);
            StringConversionServices.SerializeKeyValuePair<Type>(sb, "ActionToken", actionTokenType);
            Payload = sb.ToString();
        }

        public new static ActionToken Deserialize(string serialiedWorkflowActionToken)
        {
            return WorkflowActionToken.Deserialize(serialiedWorkflowActionToken);
        }
    }
}
