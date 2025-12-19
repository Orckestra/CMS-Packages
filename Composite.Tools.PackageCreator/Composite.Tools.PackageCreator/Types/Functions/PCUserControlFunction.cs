using System.Collections.Generic;
using Composite.AspNet.Security;
using Composite.C1Console.Security;
using Composite.Core.IO;
using Composite.Functions;

namespace Composite.Tools.PackageCreator.Types
{
    internal partial class PCFunctions
    {
        public static IEnumerable<IPackItem> CreateUserControl(EntityToken entityToken)
        {
            if (!(entityToken is FileBasedFunctionEntityToken functionEntityToken)) 
                yield break;

            if (functionEntityToken.FunctionProviderName == PackageCreatorFacade.UserControlFunctionProviderName)
                yield return new PCFunctions(functionEntityToken.FunctionName);
        }

        public void PackUserControlFunction(PackageCreator pc, IFunction function)
        {
            if (!(function.EntityToken is FileBasedFunctionEntityToken functionEntityToken)) 
                return;

            if (functionEntityToken.FunctionProviderName != PackageCreatorFacade.UserControlFunctionProviderName) 
                return;

            var virtualPath = function.GetProperty("VirtualPath");
            pc.AddFile(virtualPath);

            var codeFile = $"{virtualPath}.cs";
            if (C1File.Exists(PathUtil.Resolve(codeFile))) 
                pc.AddFile(codeFile);
        }
    }
}