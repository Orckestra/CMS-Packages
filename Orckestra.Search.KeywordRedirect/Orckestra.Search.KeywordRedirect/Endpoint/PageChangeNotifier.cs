using System;
using System.Collections.Generic;
using System.Globalization;
using Composite.Core.Application;
using Composite.Data;
using Composite.Data.ProcessControlled;
using Composite.Data.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Orckestra.Search.KeywordRedirect.Endpoint
{
    [ApplicationStartup()]
    internal class PageChangeNotifierRegistrator
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(typeof(PageChangeNotifier));
        }
    }

    /// <summary>
    /// Component change structure
    /// </summary>
    public class PageChange
    {
        /// <summary>
        /// CultureInfo
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

    }

    public class PageChangeNotifier : ChangeNotifierBase<PageChange>
    {
        public override void OnChange(object sender, DataEventArgs dataEventArgs)
        {
            if (dataEventArgs.Data is ILocalizedControlled localizedControlled)
            {
                var change = new PageChange { CultureInfo = new CultureInfo(localizedControlled.SourceCultureName) };
                foreach (var observer in Observers)
                {
                    observer.OnNext(change);
                }
            }
        }
    }
}
