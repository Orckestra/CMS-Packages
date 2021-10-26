using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Composite.C1Console.Security;
using Composite.Functions;
using Composite.Plugins.Functions.FunctionProviders.MvcFunctions;

namespace Composite.AspNet.MvcFunctions.FunctionProvider
{
    /// <summary>
    /// A wrapper for an <see cref="MvcFunctionBase"/> that implements <see cref="IAsyncFunction"/>
    /// </summary>
    internal class AsyncFunctionWrapper : IAsyncFunction
    {
        private readonly MvcFunctionBase _mvcFunction;

        public AsyncFunctionWrapper(MvcFunctionBase mvcFunction)
        {
            _mvcFunction = mvcFunction ?? throw new ArgumentNullException(nameof(mvcFunction));
        }

        public string Name => _mvcFunction.Name;
        public string Namespace => _mvcFunction.Namespace;
        public string Description => _mvcFunction.Description;
        public Type ReturnType => _mvcFunction.ReturnType;
        public IEnumerable<ParameterProfile> ParameterProfiles => _mvcFunction.ParameterProfiles;
        public EntityToken EntityToken => _mvcFunction.EntityToken;
        public object Execute(ParameterList parameters, FunctionContextContainer context)
        {
            throw new NotSupportedException(nameof(ExecuteAsync) + " should have been called instead.");
        }

        public Task<object> ExecuteAsync(ParameterList parameters, FunctionContextContainer context)
        {
            return _mvcFunction.ExecuteAsync(parameters, context);
        }
    }
}
