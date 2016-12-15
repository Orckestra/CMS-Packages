using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Composite.Core.Extensions;
using Composite.Core.Routing.Pages;
using Composite.Functions;
using Composite.Plugins.Functions.FunctionProviders.MvcFunctions;

namespace Composite.AspNet.MvcFunctions.FunctionProvider
{
    internal class MvcActionFunction : MvcFunctionBase
    {
        private readonly ReflectedControllerDescriptor _controllerDescriptor;
        private readonly string _actionName;
        private readonly string _routeToRender;
        private bool _handlePathInfo;

        public MvcActionFunction(Type controllerType, string actionName, string @namespace, string name, string description,
            FunctionCollection functionCollection)
            : base(@namespace, name, description, functionCollection)
        {
            _controllerDescriptor = new ReflectedControllerDescriptor(controllerType);
            _actionName = actionName;

            var actions = _controllerDescriptor.GetCanonicalActions().Where(a => a.ActionName == actionName);
            Verify.That(actions.Any(), "Action name '{0}' isn't recognized", actionName);

            _routeToRender = "~/{0}/{1}".FormatWith(_controllerDescriptor.ControllerName, actionName);
        }

        override internal IEnumerable<ParameterInfo> GetParameterInformation()
        {
            return _controllerDescriptor
                .GetCanonicalActions()
                .Where(a => a.ActionName == _actionName)
                .SelectMany(action => action.GetParameters()
                .Select(p => ((ReflectedParameterDescriptor)p).ParameterInfo));
        }

        protected override string GetMvcRoute(ParameterList parameters)
        {
            string route = _routeToRender;

            if (_handlePathInfo)
            {
                route += C1PageRoute.GetPathInfo() ?? string.Empty;
            }
            return route;
        }

        protected override string GetBaseMvcRoute(ParameterList parameters)
        {
            return _routeToRender;
        }

        protected override bool HandlesPathInfo => _handlePathInfo;

        public override void UsePathInfoForRouting()
        {
            _handlePathInfo = true;
        }
    }
}
