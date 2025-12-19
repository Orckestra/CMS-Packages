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
        public static IEnumerable<IPackItem> CreateXslt(EntityToken entityToken)
        {
            if (!(entityToken is DataEntityToken dataEntityToken)) 
                yield break;

            if (dataEntityToken.Data is IXsltFunction data)
            {
                yield return new PCFunctions($"{data.Namespace}.{data.Name}");
            }
        }

        public void PackXsltFunction(PackageCreator pc)
        {
            IXsltFunction xsltFunction;
            try
            {
                xsltFunction = DataFacade.GetData<IXsltFunction>().First(i => $"{i.Namespace}.{i.Name}" == Id);
            }
            catch (Exception)
            {
                throw new ArgumentException($@"XSLT Function '{Id}' doesn't exists");
            }

            var newXslFilePath = $"\\{xsltFunction.Namespace.Replace(".", "\\")}\\{xsltFunction.Name}.xsl";
            
            pc.AddFile($"App_Data\\Xslt{xsltFunction.XslFilePath}", "App_Data\\Xslt" + newXslFilePath);
            xsltFunction.XslFilePath = newXslFilePath;

            pc.AddData(xsltFunction);

            var parameters = DataFacade.GetData<IParameter>()
                .Where(i => i.OwnerId == xsltFunction.Id)
                .OrderBy(i => i.Position);

            foreach (var parameter in parameters) pc.AddData(parameter);

            var namedFunctionCalls = DataFacade.GetData<INamedFunctionCall>()
                .Where(i => i.XsltFunctionId == xsltFunction.Id)
                .OrderBy(i => i.Name);

            foreach (var namedFunctionCall in namedFunctionCalls) 
                pc.AddData(namedFunctionCall);
        }
    }
}