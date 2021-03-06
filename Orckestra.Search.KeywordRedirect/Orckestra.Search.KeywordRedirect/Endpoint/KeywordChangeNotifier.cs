﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Composite.Core.Application;
using Composite.Data;
using Composite.Data.ProcessControlled;
using Microsoft.Extensions.DependencyInjection;

namespace Orckestra.Search.KeywordRedirect.Endpoint
{

    [ApplicationStartup()]
    internal class KeywordChangeNotifierRegistrator
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(typeof(KeywordChangeNotifier));
        }
    }

    /// <summary>
    /// Component change structure
    /// </summary>
    public class KeywordChange
    {
        /// <summary>
        /// CultureInfo
        /// </summary>
        public CultureInfo CultureInfo { get; set; }

    }

    public class KeywordChangeNotifier : ChangeNotifierBase<KeywordChange>
    {
        public override void OnChange(object sender, DataEventArgs dataEventArgs)
        {
            if (dataEventArgs.Data is ILocalizedControlled localizedControlled)
            {
                var change = new KeywordChange { CultureInfo = new CultureInfo(localizedControlled.SourceCultureName) };
                foreach (var observer in Observers)
                {
                    observer.OnNext(change);
                }
            }
        }
    }
}
