using Composite.AspNet.MvcFunctions.Dependency;
using Composite.Core.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Composite.AspNet.MvcFunctions
{
    [ApplicationStartup(AbortStartupOnException = true)]
    public static class StartupHandler
    {
        public static void OnBeforeInitialize()
        {
        }

        public static void OnInitialized()
        {
            DependencyResolver.SetResolver(new C1CmsDependencyResolver(DependencyResolver.Current));
        }
    }
}
