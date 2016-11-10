using System.Collections.Generic;
using System.Linq;
using Composite.C1Console.Security;
using Composite.Data;
using Composite.Data.Types;



namespace Composite.Tools.PackageCreator.Types
{
    partial class PCFunctions
    {


        public static IEnumerable<IPackItem> CreateInline(EntityToken entityToken)
        {
            if (entityToken is DataEntityToken)
            {
                DataEntityToken dataEntityToken = (DataEntityToken)entityToken;
                if (dataEntityToken.Data is IInlineFunction)
                {
                    IInlineFunction data = (IInlineFunction)dataEntityToken.Data;
                    yield return new PCFunctions(data.Namespace + "." + data.Name);
                    yield break;
                }
            }
        }

        public void PackInlineFunction(PackageCreator creator)
        {
            var inlineFunction = DataFacade.GetData<IInlineFunction>(data => (data.Namespace + "." + data.Name) == this.Name).FirstOrDefault();
            if (inlineFunction != null)
            {

                var inlineFunctionId = inlineFunction.Id;
                creator.AddData(inlineFunction);
                creator.AddData<IInlineFunctionAssemblyReference>(d => d.Function == inlineFunctionId);
                creator.AddData<IParameter>(d => d.OwnerId == inlineFunctionId);

                creator.AddFile(string.Format(@"App_Data\Composite\InlineCSharpFunctions\{0}", inlineFunction.CodePath), this.AllowOverwrite);
            }
        }
    }

}

