using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.Data;
using Composite.Core.ResourceSystem;
using Composite.C1Console.Security;
using Composite.AspNet.Security;
using Composite.Functions;

namespace Composite.Tools.PackageCreator.Types
{
	partial class PCFunctions
	{
		
		public static IEnumerable<IPackageCreatorItem> CreateRazor(EntityToken entityToken)
		{
			if (entityToken is FileBasedFunctionEntityToken)
			{
				var functionEntityToken = (FileBasedFunctionEntityToken)entityToken;
				if (functionEntityToken.FunctionProviderName == PackageCreatorFacade.RazorFunctionProviderName)
				{
					yield return new PCFunctions(functionEntityToken.FunctionName);
				}
			}
			yield break;
		}

		public void PackRazorFunction(PackageCreator pc, IFunction function) 
		{
			if (function.EntityToken is FileBasedFunctionEntityToken)
			{
				var functionEntityToken = (FileBasedFunctionEntityToken)function.EntityToken;
				if (functionEntityToken.FunctionProviderName == PackageCreatorFacade.RazorFunctionProviderName)
				{
					var virtualPath = function.GetProperty("VirtualPath");
					pc.AddFile(virtualPath);
				}
			}
		}
	}
}
