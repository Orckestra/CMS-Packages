using System.Text;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.Serialization;

namespace Composite.Tools.PackageCreator.Workflow
{
    public sealed class CreatePackageWorkflowActionToken : WorkflowActionToken
    {
        public CreatePackageWorkflowActionToken(string name, ActionToken actionToken) :
            base(typeof(CreatePackageWorkflow), new PermissionType[] { PermissionType.Administrate })
        {
            StringBuilder sb = new StringBuilder();
            StringConversionServices.SerializeKeyValuePair(sb, "Name", name);
            StringConversionServices.SerializeKeyValuePair(sb, "ActionToken", ActionTokenSerializer.Serialize(actionToken));
            Payload = sb.ToString();
        }

        public new static ActionToken Deserialize(string serialiedWorkflowActionToken)
        {
            return WorkflowActionToken.Deserialize(serialiedWorkflowActionToken);
        }
    }
}
