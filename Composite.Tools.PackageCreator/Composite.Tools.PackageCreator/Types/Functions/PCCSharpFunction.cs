using System;
using System.Collections.Generic;
using System.Linq;
using Composite.C1Console.Security;
using Composite.Data;
using Composite.Data.Types;

namespace Composite.Tools.PackageCreator.Types
{
    partial class PCFunctions
    {

        public static IEnumerable<IPackItem> CreateCSharp(EntityToken entityToken)
        {
            if (entityToken is DataEntityToken)
            {
                DataEntityToken dataEntityToken = (DataEntityToken)entityToken;
                if (dataEntityToken.Data is IMethodBasedFunctionInfo)
                {
                    IMethodBasedFunctionInfo data = (IMethodBasedFunctionInfo)dataEntityToken.Data;
                    yield return new PCFunctions(data.Namespace + "." + data.UserMethodName);
                    yield break;
                }
            }
        }

        public void PackCSharpFunction(PackageCreator pc)
        {
            IMethodBasedFunctionInfo csharpFunction;
            try
            {
                csharpFunction = (from i in DataFacade.GetData<IMethodBasedFunctionInfo>()
                                  where i.Namespace + "." + i.UserMethodName == this.Id
                                  select i).First();
            }
            catch (Exception)
            {
                throw new ArgumentException(string.Format(@"C# Function '{0}' doesn't exists", this.Id));
            }

            pc.AddData(csharpFunction);
        }
    }

}
