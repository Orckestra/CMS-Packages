using System.Web.Hosting;
using System.Web.Mvc;
using Composite.AspNet.MvcFunctions;
using Composite.C1Console.Elements;
using Composite.Core.Application;
using Composite.Data;
using Composite.Data.DynamicTypes;
using Composite.Data.Types;
using Microsoft.Extensions.DependencyInjection;
using Orckestra.Search.KeywordRedirect.C1Console;
using Orckestra.Search.KeywordRedirect.Controllers;
using Orckestra.Search.KeywordRedirect.Data.Types;
using Orckestra.Search.KeywordRedirect.Endpoint;

namespace Orckestra.Search.KeywordRedirect
{
    [ApplicationStartup]
    public class KeywordsStartupHandler
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            if (!HostingEnvironment.IsHosted) return;
            services.AddSingleton(typeof(KeywordManager));
        }

        public static void OnBeforeInitialize()
        {
            if (!HostingEnvironment.IsHosted) return;
        }

        public static void OnInitialized(KeywordChangeNotifier keywordChangeNotifier, PageChangeNotifier pageChangeNotifier,
            BeforeKeywordChangeNotifier beforeKeywordChangeNotifier)
        {
            if (!HostingEnvironment.IsHosted) return;

            DynamicTypeManager.EnsureCreateStore(typeof(RedirectKeyword));

            DataEvents<RedirectKeyword>.OnAfterAdd += keywordChangeNotifier.OnChange;
            DataEvents<RedirectKeyword>.OnAfterUpdate += keywordChangeNotifier.OnChange;
            DataEvents<RedirectKeyword>.OnDeleted += keywordChangeNotifier.OnChange;
            DataEvents<IPage>.OnAfterAdd += pageChangeNotifier.OnChange;
            DataEvents<IPage>.OnAfterUpdate += pageChangeNotifier.OnChange;
            DataEvents<IPage>.OnDeleted += pageChangeNotifier.OnChange;
            DataEvents<RedirectKeyword>.OnBeforeAdd += beforeKeywordChangeNotifier.OnChange;
            DataEvents<RedirectKeyword>.OnBeforeUpdate += beforeKeywordChangeNotifier.OnChange;

            var functions = MvcFunctionRegistry.NewFunctionCollection();
            RegisterFunctions(functions);
            RegisterFunctionRoutes(functions);

            UrlToEntityTokenFacade.Register(new KeywordsUrlToEntityTokenMapper());
        }


        private static void RegisterFunctions(FunctionCollection functions)
        {
            functions.RegisterAction<KeywordController>("RedirectByKeyword", "Composer.Search.RedirectByKeyword", "${Orckestra.Search.KeywordRedirect,RedirectByKeywordDescription}")
                .AddParameter("ParamName",
                    typeof(string),
                    label: Localization.RedirectByKeywordParamLabel,
                    helpText: Localization.RedirectByKeywordParamTooltip
                ); ;
        }

        private static void RegisterFunctionRoutes(FunctionCollection functions)
        {
            functions.RouteCollection.MapMvcAttributeRoutes();
            functions.RouteCollection.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { action = "Index", id = UrlParameter.Optional } // Parameter defaults
                );
        }
    }
}
