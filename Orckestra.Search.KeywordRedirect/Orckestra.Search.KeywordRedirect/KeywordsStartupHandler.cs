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
            services.AddSingleton(typeof(KeywordManager));
        }

        public static void OnBeforeInitialize()
        {
        }

        public static void OnInitialized(KeywordChangeNotifier keywordChangeNotifier, BeforeKeywordChangeNotifier beforeKeywordChangeNotifier)
        {
            DynamicTypeManager.EnsureCreateStore(typeof(RedirectKeyword));

            DataEvents<RedirectKeyword>.OnAfterAdd += keywordChangeNotifier.KeywordChange;
            DataEvents<RedirectKeyword>.OnAfterUpdate += keywordChangeNotifier.KeywordChange;
            DataEvents<RedirectKeyword>.OnDeleted += keywordChangeNotifier.KeywordChange;
            DataEvents<IPage>.OnAfterAdd += keywordChangeNotifier.KeywordChange;
            DataEvents<IPage>.OnAfterUpdate += keywordChangeNotifier.KeywordChange;
            DataEvents<IPage>.OnDeleted += keywordChangeNotifier.KeywordChange;
            DataEvents<RedirectKeyword>.OnBeforeAdd += beforeKeywordChangeNotifier.BeforeKeywordChange;
            DataEvents<RedirectKeyword>.OnBeforeUpdate += beforeKeywordChangeNotifier.BeforeKeywordChange;

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
