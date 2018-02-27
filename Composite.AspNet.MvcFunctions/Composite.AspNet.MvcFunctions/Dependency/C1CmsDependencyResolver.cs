using Composite.Core;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Composite.AspNet.MvcFunctions.Dependency
{
    internal class C1CmsDependencyResolver : IDependencyResolver
    {
        public C1CmsDependencyResolver(IDependencyResolver innerResolver)
        {
            InnerResolver = innerResolver;
        }

        public IDependencyResolver InnerResolver { get; }

        public  object GetService(Type serviceType)
        {
            return InnerResolver?.GetService(serviceType) ?? ServiceLocator.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return InnerResolver?.GetServices(serviceType) ?? ServiceLocator.GetServices(serviceType);
        }
    }
}
