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
        public static IEnumerable<IPackItem> CreateVisual(EntityToken entityToken)
        {
            if (!(entityToken is DataEntityToken dataEntityToken)) 
                yield break;

            if (dataEntityToken.Data is IVisualFunction data)
            {
                yield return new PCFunctions($"{data.Namespace}.{data.Name}");
            }
        }

        public void PackVisualFunction(PackageCreator pc)
        {
            IVisualFunction visualFunction;
            try
            {
                visualFunction = DataFacade.GetData<IVisualFunction>()
                    .First(i => $"{i.Namespace}.{i.Name}" == Id);
            }
            catch (Exception)
            {
                throw new ArgumentException($@"Visual Function '{Id}' doesn't exists");
            }

            pc.AddData(visualFunction);
        }
    }
}