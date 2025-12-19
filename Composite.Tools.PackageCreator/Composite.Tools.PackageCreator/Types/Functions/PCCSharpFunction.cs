using System;
using System.Collections.Generic;
using System.Linq;
using Composite.C1Console.Security;
using Composite.Data;
using Composite.Data.Types;

namespace Composite.Tools.PackageCreator.Types
{
    internal partial class PCFunctions
    {
        public static IEnumerable<IPackItem> CreateCSharp(EntityToken entityToken)
        {
            if (!(entityToken is DataEntityToken dataEntityToken)) 
                yield break;

            if (dataEntityToken.Data is IMethodBasedFunctionInfo data)
            {
                yield return new PCFunctions($"{data.Namespace}.{data.UserMethodName}");
            }
        }

        public void PackCSharpFunction(PackageCreator pc)
        {
            IMethodBasedFunctionInfo csharpFunction;
            try
            {
                csharpFunction = (from i in DataFacade.GetData<IMethodBasedFunctionInfo>()
                    where i.Namespace + "." + i.UserMethodName == Id
                    select i).First();
            }
            catch (Exception)
            {
                throw new ArgumentException($@"C# Function '{Id}' doesn't exists");
            }

            pc.AddData(csharpFunction);
        }
    }
}