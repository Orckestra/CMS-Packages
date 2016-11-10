using System.Collections.Generic;
using Composite.AspNet.Security;
using Composite.C1Console.Security;
using Composite.Core.IO;
using Composite.Functions;

namespace Composite.Tools.PackageCreator.Types
{
    partial class PCFunctions
    {

        public static IEnumerable<IPackItem> CreateUserControl(EntityToken entityToken)
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
                        pc.AddFile(codeFile, this.AllowOverwrite);
                    }
                }
            }
        }
    }
}
