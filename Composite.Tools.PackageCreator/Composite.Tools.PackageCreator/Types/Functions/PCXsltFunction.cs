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
        public static IEnumerable<IPackItem> CreateXslt(EntityToken entityToken)
        {
            if (entityToken is DataEntityToken)
            {
                DataEntityToken dataEntityToken = (DataEntityToken)entityToken;
                if (dataEntityToken.Data is IXsltFunction)
                {
                    IXsltFunction data = (IXsltFunction)dataEntityToken.Data;
                    yield return new PCFunctions(data.Namespace + "." + data.Name);
                    yield break;
                }
            }
        }

        public void PackXsltFunction(PackageCreator pc)
        {
            IXsltFunction xsltFunction;
            try
            {
                xsltFunction = (from i in DataFacade.GetData<IXsltFunction>()
                                where i.Namespace + "." + i.Name == this.Id
                                select i).First();
            }
            catch (Exception)
            {
                throw new ArgumentException(string.Format(@"XSLT Function '{0}' doesn't exists", this.Id));
            }

            var newXslFilePath = "\\" + xsltFunction.Namespace.Replace(".", "\\") + "\\" + xsltFunction.Name + ".xsl";


            pc.AddFile("App_Data\\Xslt" + xsltFunction.XslFilePath, "App_Data\\Xslt" + newXslFilePath);
            xsltFunction.XslFilePath = newXslFilePath;

            pc.AddData(xsltFunction);

            var parameters = from i in DataFacade.GetData<IParameter>()
                             where i.OwnerId == xsltFunction.Id
                             orderby i.Position
                             select i;

            foreach (var parameter in parameters) pc.AddData(parameter);

            var namedFunctionCalls = from i in DataFacade.GetData<INamedFunctionCall>()
                                     where i.XsltFunctionId == xsltFunction.Id
                                     orderby i.Name
                                     select i;

            foreach (var namedFunctionCall in namedFunctionCalls) pc.AddData(namedFunctionCall);
        }
    }
}
