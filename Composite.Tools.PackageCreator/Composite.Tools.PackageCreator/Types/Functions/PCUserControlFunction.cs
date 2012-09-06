using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.Data;
using Composite.Core.ResourceSystem;
using Composite.C1Console.Security;
using Composite.AspNet.Security;
using Composite.Functions;
using Composite.Core.IO;

namespace Composite.Tools.PackageCreator.Types
{
	partial class PCFunctions
	{

		public static IEnumerable<IPackageCreatorItem> CreateUserControl(EntityToken entityToken)
		{
			if (entityToken is FileBasedFunctionEntityToken)
			{
				var functionEntityToken = (FileBasedFunctionEntityToken)entityToken;
				if (functionEntityToken.FunctionProviderName == PackageCreatorFacade.UserControlFunctionProviderName)
				{
					yield return new PCFunctions(functionEntityToken.FunctionName);
				}
			}
			yield break;
		}

		public void PackUserControlFunction(PackageCreator pc, IFunction function)
		{
			if (function.EntityToken is FileBasedFunctionEntityToken)
			{
				var functionEntityToken = (FileBasedFunctionEntityToken)function.EntityToken;
				if (functionEntityToken.FunctionProviderName == PackageCreatorFacade.UserControlFunctionProviderName)
				{
					var virtualPath = function.GetProperty("VirtualPath");
					pc.AddFile(virtualPath);

					string codeFile = virtualPath + ".cs";
					if (C1File.Exists(PathUtil.Resolve(codeFile)))
					{
						pc.AddFile(codeFile);
					}
				}
			}
		}
	}
}
