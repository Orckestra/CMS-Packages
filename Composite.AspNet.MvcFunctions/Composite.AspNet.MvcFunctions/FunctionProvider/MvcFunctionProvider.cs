using System.Collections.Generic;
using Composite.AspNet.MvcFunctions;
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

        public IEnumerable<IFunction> Functions
        {
            get { return MvcFunctionRegistry.Functions; }
        }

        public static void Reload()
        {
            // Can be called before function provider initialization
            if (_functionNotifier != null)
            {
                _functionNotifier.FunctionsUpdated();
            }
        }
    }
}
