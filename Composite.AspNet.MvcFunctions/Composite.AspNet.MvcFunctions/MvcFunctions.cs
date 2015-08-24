using System.Collections.Generic;
using Composite.Plugins.Functions.FunctionProviders.MvcFunctions;

namespace Composite.AspNet.MvcFunctions
{
    public static class MvcFunctionRegistry
    {
        private static readonly List<FunctionCollection> FunctionCollections = new List<FunctionCollection>();
        internal static IList<MvcFunctionBase> Functions = new List<MvcFunctionBase>();

        public static FunctionCollection NewFunctionCollection()
        {
            var result = new FunctionCollection();
            FunctionCollections.Add(result);

            return result;
        }
    }
}
