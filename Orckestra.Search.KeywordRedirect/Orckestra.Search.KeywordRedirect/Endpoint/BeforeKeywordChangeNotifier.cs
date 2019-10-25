using System;
using System.Collections.Generic;
using System.Globalization;
using Composite.Core.Application;
using Composite.Data;
using Composite.Data.ProcessControlled;
using Microsoft.Extensions.DependencyInjection;
using Orckestra.Search.KeywordRedirect.Data.Types;

namespace Orckestra.Search.KeywordRedirect.Endpoint
{
    [ApplicationStartup()]
    internal class BeforeKeywordChangeNotifierRegistrator
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(typeof(BeforeKeywordChangeNotifier));
        }
    }

    public class BeforeKeywordChangeNotifier : ChangeNotifierBase<RedirectKeyword>
    {
    };
}
