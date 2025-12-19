using System.Collections.Generic;
using Composite.AspNet.Security;
using Composite.C1Console.Security;
using Composite.Functions;

namespace Composite.Tools.PackageCreator.Types
{
    internal partial class PCFunctions
    {
        public static IEnumerable<IPackItem> CreateRazor(EntityToken entityToken)
        {
            if (!(entityToken is FileBasedFunctionEntityToken functionEntityToken)) 
                yield break;

            if (functionEntityToken.FunctionProviderName == PackageCreatorFacade.RazorFunctionProviderName)
                yield return new PCFunctions(functionEntityToken.FunctionName);
        }

        public void PackRazorFunction(PackageCreator pc, IFunction function)
        {
            if (!(function.EntityToken is FileBasedFunctionEntityToken functionEntityToken))
                return;

            if (functionEntityToken.FunctionProviderName != PackageCreatorFacade.RazorFunctionProviderName) 
                return;

            var virtualPath = function.GetProperty("VirtualPath");
            pc.AddFile(virtualPath);
        }
    }
}