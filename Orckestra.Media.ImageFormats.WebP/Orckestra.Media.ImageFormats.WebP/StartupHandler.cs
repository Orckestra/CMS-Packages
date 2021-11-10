using System;
using Composite.Core.Application;
using Composite.Core.Application.Plugins.ApplicationStartupHandler;
using Composite.Core.WebClient.Media;
using Microsoft.Extensions.DependencyInjection;

namespace Orckestra.Media.ImageFormats.WebP
{
    [ApplicationStartup]
    public class StartupHandler : IApplicationStartupHandler
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(typeof(IImageFileFormatProvider), typeof(WebPImageFileFormatProvider));
        }

        public void OnBeforeInitialize(IServiceProvider serviceProvider)
        {
        }

        public void OnInitialized(IServiceProvider serviceProvider)
        {
        }
    }
}
