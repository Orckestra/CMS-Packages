using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using Composite.AspNet.MvcFunctions;
using Composite.Core.Routing.Pages;
using Composite.Functions;

namespace Composite.Plugins.Functions.FunctionProviders.MvcFunctions
{
    internal class MvcControllerFunction: MvcFunctionBase
    {
        private readonly ReflectedControllerDescriptor _controllerDescriptor;

        public MvcControllerFunction(ReflectedControllerDescriptor controllerDescriptor,
            string @namespace, string name, string description, FunctionCollection functionCollection)
            : base(@namespace, name, description, functionCollection)
        {
            Verify.ArgumentNotNull(controllerDescriptor, "controllerDescriptor");

            _controllerDescriptor = controllerDescriptor;

            RequireAsyncHandler = _controllerDescriptor.ControllerType
                            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                            .Any(method => method.ReturnType == typeof(Task)
                                           || (method.ReturnType.IsGenericType
                                               && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)));
        }

        override internal IEnumerable<ParameterInfo> GetParameterInformation()
        {
            return _controllerDescriptor
                .GetCanonicalActions()
                .SelectMany(action => action.GetParameters()
                .Select(p => ((ReflectedParameterDescriptor)p).ParameterInfo));
        }

        private string GetParametersUrlString(ParameterList parameterList)
        {
            var result = "";
            foreach (var parameter in ParameterProfiles)
            {
                string value = parameterList.GetParameter(parameter.Name).ToString();

                result += "/" + value;
            }
            return result;
        }

        protected override string GetMvcRoute(ParameterList parameterList)
        {
            string pathInfo = C1PageRoute.GetPathInfo();

            return GetBaseMvcRoute(parameterList) + pathInfo;
        }

        protected override string GetBaseMvcRoute(ParameterList parameterList)
        {
            return "~/" + _controllerDescriptor.ControllerName + GetParametersUrlString(parameterList);
        }

        protected override bool HandlesPathInfo { get { return true; } }
    }
}
