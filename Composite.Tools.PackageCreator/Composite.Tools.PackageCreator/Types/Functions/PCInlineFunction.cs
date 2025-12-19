using System.Collections.Generic;
using System.Linq;
using Composite.C1Console.Security;
using Composite.Data;
using Composite.Data.Types;

namespace Composite.Tools.PackageCreator.Types
{
    internal partial class PCFunctions
    {
        public static IEnumerable<IPackItem> CreateInline(EntityToken entityToken)
        {
            if (!(entityToken is DataEntityToken dataEntityToken)) 
                yield break;

            if (dataEntityToken.Data is IInlineFunction data)
            {
                yield return new PCFunctions($"{data.Namespace}.{data.Name}");
            }
        }

        public void PackInlineFunction(PackageCreator creator)
        {
            var inlineFunction = DataFacade.GetData<IInlineFunction>(data => $"{data.Namespace}.{data.Name}" == Name)
                .FirstOrDefault();

            if (inlineFunction == null) 
                return;

            var inlineFunctionId = inlineFunction.Id;
            creator.AddData(inlineFunction);
            creator.AddData<IInlineFunctionAssemblyReference>(d => d.Function == inlineFunctionId);
            creator.AddData<IParameter>(d => d.OwnerId == inlineFunctionId);

            creator.AddFile($@"App_Data\Composite\InlineCSharpFunctions\{inlineFunction.CodePath}");
        }
    }
}