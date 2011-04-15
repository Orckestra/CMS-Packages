using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.Core.ResourceSystem;
using Composite.C1Console.Security;
using Composite.Data;
using Composite.Data.Types;



namespace Composite.Tools.PackageCreator.Types
{
	[PCCategory("InlineFunctions")]
	class PCInlineFunction : SimplePackageCreatorItem, IPackagable
	{

		public PCInlineFunction(string name)
			: base(name)
		{
		}

		public override ResourceHandle ItemIcon
		{
			get { return new ResourceHandle("Composite.Icons", "base-function-function"); }
		}

		public static IEnumerable<IPackageCreatorItem> Create(EntityToken entityToken)
		{
			if (entityToken is DataEntityToken)
			{
				DataEntityToken dataEntityToken = (DataEntityToken)entityToken;
				if (dataEntityToken.Data is IInlineFunction)
				{
					IInlineFunction data = (IInlineFunction)dataEntityToken.Data;
					yield return new PCInlineFunction(data.Namespace + "." + data.Name);
					yield break;
				}
			}
		}

		public void Pack(PackageCreator creator)
		{
			var inlineFunction = DataFacade.GetData<IInlineFunction>(data => (data.Namespace + "." + data.Name) == this._name).FirstOrDefault();
			if (inlineFunction == null)
				throw new InvalidOperationException(string.Format("Inline function '{0}' does not exists", this._name));
			var inlineFunctionId = inlineFunction.Id;
			creator.AddData(inlineFunction);
			creator.AddData<IInlineFunctionAssemblyReference>(d => d.Function == inlineFunctionId);
			creator.AddData<IParameter>(d => d.OwnerId == inlineFunctionId);

			creator.AddFile(string.Format(@"App_Data\Composite\InlineCSharpFunctions\{0}", inlineFunction.CodePath));
		}
	}

}

