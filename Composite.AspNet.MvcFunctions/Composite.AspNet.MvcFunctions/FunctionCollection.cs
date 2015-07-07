//using Composite.Plugins.Functions.FunctionProviders.FileBasedFunctionProvider;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Composite.AspNet.MvcFunctions.FunctionProvider;
using Composite.Core.Extensions;
using Composite.Core.Linq;
using Composite.Functions;
using Composite.Plugins.Functions.FunctionProviders.MvcFunctions;

namespace Composite.AspNet.MvcFunctions
{
    public static class FunctionCollection
    {
        public static RouteCollection RouteCollection = new RouteCollection();

        internal static IList<MvcFunctionBase> Functions { get; private set; }

        static FunctionCollection()
        {
            Functions = new List<MvcFunctionBase>();
        }

        public static void AutoDiscoverFunctions(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Where(IsController))
            {
                // Searching for [C1Function] attribute on controller or on its methods
                var attribute = type.GetCustomAttributes<C1FunctionAttribute>(false).SingleOrDefault();
                if (attribute != null)
                {
                    RegisterController(type, attribute, null);
                }

                foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    attribute = method.GetCustomAttributes<C1FunctionAttribute>(false).SingleOrDefault();
                    if (attribute == null) continue;

                    var routeAttribute = method.GetCustomAttributes<RouteAttribute>(false)
                        .SingleOrDefaultOrException("Multiple [Route] attributes defined on method '{0}' of controller class '{1}'", 
                                                     method.Name, type.FullName);

                    // If [Route()] attribute is present, registering a "controller function", otherwise - an "action function"
                    if (routeAttribute != null)
                    {
                        var parameters = GetFunctionParameters(method, routeAttribute);
                        RegisterController(type, attribute, parameters);
                    }
                    else
                    {
                        var actionNameAttribute = method.GetCustomAttributes<ActionNameAttribute>(false).SingleOrDefault();
                        string actionName = actionNameAttribute != null ? actionNameAttribute.Name : method.Name;

                        var parameters = GetFunctionParameters(method);
                        RegisterActionFunction(type, actionName, attribute, parameters);
                    }
                }
            }

            MvcFunctionProvider.Reload();
        }

        private static MvcActionFunction RegisterActionFunction(Type controllerType, string actionName, C1FunctionAttribute attribute, IEnumerable<ParameterProfile> functionParameters)
        {
            var controllerDescriptor = new ReflectedControllerDescriptor(controllerType);

            string @namespace = String.IsNullOrEmpty(attribute.Namespace) ? controllerDescriptor.ControllerType.Namespace : attribute.Namespace;
            string name = String.IsNullOrEmpty(attribute.Name) ? controllerDescriptor.ControllerName : attribute.Name;
            string description = attribute.Description ?? String.Empty;

            var function = new MvcActionFunction(controllerType, actionName, @namespace, name, description);

            if (functionParameters != null)
            {
                foreach (var param in functionParameters)
                {
                    function.AddParameter(param);
                }
            }

            Functions.Add(function);

            return function;
        }

        private static bool IsController(Type type)
        {
            return type.IsPublic && type.Name.EndsWith("Controller") && typeof (Controller).IsAssignableFrom(type);
        }

        public static MvcFunctionBuilder RegisterAction<T>(string actionName, string functionName = null, string description = null) where T : Controller
        {
            var controllerType = typeof (T);

            Verify.That(IsController(controllerType), "Type '{0}' is not public or not its name does not have the 'Controller' ending", controllerType.FullName);

            //var methodInfo = StaticReflection.GetMethodInfo(method);

            string @namespace = null;
            string name = null;

            if (string.IsNullOrWhiteSpace(functionName))
            {
                string controllerName = controllerType.FullName.Substring(0,
                    controllerType.FullName.Length - "Controller".Length);


                if (string.Equals(actionName, "Index", StringComparison.OrdinalIgnoreCase))
                {
                    name = actionName;
                    @namespace = controllerType.Namespace + "." + controllerName;
                }
                else
                {
                    name = controllerName;
                    @namespace = controllerType.Namespace;
                }
            }
            else
            {
                ParseFunctionName(functionName, out @namespace, out name);
            }
           

            var function = new MvcActionFunction(controllerType, actionName, @namespace, name, description);

            Functions.Add(function);

            MvcFunctionProvider.Reload();

            return new MvcFunctionBuilder(function);
        }

        private static void ParseFunctionName(string functionName, out string ns, out string name)
        {
            string[] parts = functionName.Split(new[] {'.'});

            Verify.That(parts.Length > 1, "Missing a function namespace in function name '{0}'", functionName);

            Verify.IsFalse(parts.Any(string.IsNullOrWhiteSpace), "Empty full name parts is not allowed");

            ns = string.Join(".", parts.Take(parts.Length - 1));
            name = parts.Last();
        }

        public static MvcFunctionBuilder RegisterController<T>(string functionName = null, string description = null) where T : Controller
        {
            return RegisterController(typeof (T), functionName, description);
        }

        public static MvcFunctionBuilder RegisterController(Type controllerType, string functionName = null, string description = null)
        {
            string @namespace = null, name = null;
            if(functionName != null) ParseFunctionName(functionName, out @namespace, out name);

            var attribute = new C1FunctionAttribute
            {
                Namespace = @namespace,
                Name = name,
                Description = description
            };

            var function = RegisterController(controllerType, attribute, new List<ParameterProfile>());

            MvcFunctionProvider.Reload();

            return new MvcFunctionBuilder(function);
        }

        private static MvcControllerFunction RegisterController(Type controllerType, C1FunctionAttribute attribute, IEnumerable<ParameterProfile> functionParameters)
        {
            var controllerDescriptor = new ReflectedControllerDescriptor(controllerType);

            string @namespace = String.IsNullOrEmpty(attribute.Namespace) ? controllerDescriptor.ControllerType.Namespace : attribute.Namespace;
            string name = String.IsNullOrEmpty(attribute.Name) ? controllerDescriptor.ControllerName : attribute.Name;
            string description = attribute.Description ?? String.Empty;

            var function = new MvcControllerFunction(controllerDescriptor, @namespace, name, description);

            if (functionParameters != null)
            {
                foreach (var param in functionParameters)
                {
                    function.AddParameter(param);
                }
            }

            Functions.Add(function);

            return function;
        }

        private static IEnumerable<ParameterProfile> GetFunctionParameters(MethodInfo methodInfo, RouteAttribute routeAttribute)
        {
            var parameterAttributes = methodInfo.GetCustomAttributes<FunctionParameterAttribute>(false).ToList();
            if (parameterAttributes.Count == 0) return null;

            string route = routeAttribute.Template;
            var parameters = methodInfo.GetParameters();
            Type controllerType = methodInfo.DeclaringType;

            Func<string, int> parameterIndex =
                parameterName => route.IndexOf("{" + parameterName + "}", StringComparison.OrdinalIgnoreCase);

            var result = new List<ParameterProfile>();

            foreach (var parameterAttribute in parameterAttributes)
            {
                string parameterName = parameterAttribute.Name;
                Verify.StringNotIsNullOrWhiteSpace(parameterAttribute.Name,
                    "Missing parameter name on [FunctionParameter] attribute. Method '{0}' of the controller class '{1}.",
                    methodInfo.Name, controllerType.FullName);
                Verify.That(parameterIndex(parameterAttribute.Name) >= 0,
                    "The [Route] does not contain a reference to parameter named '{2}'. Method '{0}' of the controller class '{1}.",
                    methodInfo.Name, controllerType.FullName, parameterName);

                var matchingParameter =
                    parameters.FirstOrDefault(
                        p => string.Equals(parameterName, p.Name, StringComparison.OrdinalIgnoreCase));
                Verify.IsNotNull(matchingParameter,
                    "There should be a method parameter that would match C1 function parameter '{2}'. Method '{0}' of the controller class '{1}.",
                    methodInfo.Name, controllerType.FullName, parameterName);

                Type parameterType = matchingParameter.ParameterType;

                result.Add(BuildParameterProfile(parameterName, parameterType, parameterAttribute, controllerType));
            }

            result.Sort((a,b) => parameterIndex(a.Name).CompareTo(parameterIndex(b.Name)));

            return result;
        }

        private static IEnumerable<ParameterProfile> GetFunctionParameters(MethodInfo methodInfo)
        {
            var parameterAttributes = methodInfo.GetCustomAttributes<FunctionParameterAttribute>(false).ToList();
            if (parameterAttributes.Count == 0) return null;


            var parameters = methodInfo.GetParameters();
            Type controllerType = methodInfo.DeclaringType;

            var parameterIndexMap = new Dictionary<string, int>();
            for (int i = 0; i < parameters.Length; i++)
            {
                parameterIndexMap.Add(parameters[i].Name.ToLowerInvariant(), i);
            }

            Func<string, int> getIndexFunc = name => parameterIndexMap[name.ToLowerInvariant()];

            var result = new List<ParameterProfile>();

            foreach (var parameterAttribute in parameterAttributes)
            {
                string name = parameterAttribute.Name;
                Verify.StringNotIsNullOrWhiteSpace(name,
                    "Missing parameter name on [FunctionParameter] attribute. Method '{0}' of the controller class '{1}.",
                    methodInfo.Name, controllerType.FullName);

                Verify.That(parameterIndexMap.ContainsKey(name.ToLowerInvariant()),
                    "There should be a method parameter that would match C1 function parameter '{2}'. Method '{0}' of the controller class '{1}.",
                    methodInfo.Name, controllerType.FullName, name);

                var matchingParameter = parameters[getIndexFunc(name)];

                Type parameterType = matchingParameter.ParameterType;

                result.Add(BuildParameterProfile(name, parameterType, parameterAttribute, controllerType));
            }

            result.Sort((a, b) => getIndexFunc(a.Name).CompareTo(getIndexFunc(b.Name)));

            return result;
        }



        internal static ParameterProfile BuildParameterProfile(string name, Type type, FunctionParameterAttribute attribute, Type controllerType)
        {
            BaseValueProvider defaultValueProvider = new NoValueValueProvider();
            string label = name;
            string helpText = String.Empty;

            if (!attribute.Label.IsNullOrEmpty())
            {
                label = attribute.Label;
            }

            if (!attribute.Help.IsNullOrEmpty())
            {
                helpText = attribute.Help;
            }

            bool isRequired = !attribute.HasDefaultValue;
            if (!isRequired)
            {
                defaultValueProvider = new ConstantValueProvider(attribute.DefaultValue);
            }

            WidgetFunctionProvider widgetProvider = attribute.GetWidgetFunctionProvider(controllerType, null);

            bool hideInSimpleView = attribute.HideInSimpleView;

            if (widgetProvider == null)
            {
                widgetProvider = StandardWidgetFunctions.GetDefaultWidgetFunctionProviderByType(type, isRequired);
            }

            return new ParameterProfile(name, type, isRequired, defaultValueProvider, widgetProvider, label,
                new HelpDefinition(helpText),
                hideInSimpleView);
        }
    }
}