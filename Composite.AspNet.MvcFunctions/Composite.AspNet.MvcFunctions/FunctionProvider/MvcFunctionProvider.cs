using System.Collections.Generic;
using System.Linq;
using Composite.AspNet.MvcFunctions;
using Composite.AspNet.MvcFunctions.FunctionProvider;
using Composite.Functions;
using Composite.Functions.Plugins.FunctionProvider;

namespace Composite.Plugins.Functions.FunctionProviders.MvcFunctions
{
    public class MvcFunctionProvider : IFunctionProvider
    {
        private static FunctionNotifier _functionNotifier;

        public FunctionNotifier FunctionNotifier
        {
            set { _functionNotifier = value; }
        }

        public IEnumerable<IFunction> Functions => 
            MvcFunctionRegistry.Functions.Select(mvcFunc => mvcFunc.RequireAsyncHandler ? (IFunction) new AsyncFunctionWrapper(mvcFunc) : mvcFunc);

        public static void Reload()
        {
            // Can be called before function provider initialization
            _functionNotifier?.FunctionsUpdated();
        }
    }
}
