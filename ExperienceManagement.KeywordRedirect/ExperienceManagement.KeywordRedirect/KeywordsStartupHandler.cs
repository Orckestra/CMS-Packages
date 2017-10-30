using Composite.AspNet.MvcFunctions;
using Composite.C1Console.Elements;
using Composite.Core.Application;
using Composite.Data;
using Composite.Data.DynamicTypes;
using Composite.Data.Types;
using Microsoft.Extensions.DependencyInjection;
using Orckestra.ExperienceManagement.KeywordRedirect;
using Orckestra.ExperienceManagement.KeywordRedirect.C1Console;
using Orckestra.ExperienceManagement.KeywordRedirect.Controllers;
using Orckestra.ExperienceManagement.KeywordRedirect.DataTypes;
using System.Web.Mvc;

namespace Orckestra.ExperienceManagement.KeywordRedirect
{
    [ApplicationStartup]
    public class KeywordsStartupHandler
    {
        public static void ConfigureServices(IServiceCollection services)
        {
        }

        public static void OnBeforeInitialize()
        {
        }

        public static void OnInitialized(KeywordChangeNotifier keywordChangeNotifier)
        {
            DynamicTypeManager.EnsureCreateStore(typeof(RedirectKeyword));

            DataEventSystemFacade.SubscribeToDataAfterAdd<RedirectKeyword>(keywordChangeNotifier.KeywordChange, true);
            DataEventSystemFacade.SubscribeToDataAfterUpdate<RedirectKeyword>(keywordChangeNotifier.KeywordChange, true);
            DataEventSystemFacade.SubscribeToDataDeleted<RedirectKeyword>(keywordChangeNotifier.KeywordChange, true);
            DataEventSystemFacade.SubscribeToDataAfterAdd<IPage>(keywordChangeNotifier.KeywordChange, true);
            DataEventSystemFacade.SubscribeToDataAfterUpdate<IPage>(keywordChangeNotifier.KeywordChange, true);
            DataEventSystemFacade.SubscribeToDataDeleted<IPage>(keywordChangeNotifier.KeywordChange, true);

            var functions = MvcFunctionRegistry.NewFunctionCollection();
            RegisterFunctions(functions);
            RegisterFunctionRoutes(functions);

            UrlToEntityTokenFacade.Register(new KeywordsUrlToEntityTokenMapper());
        }


        private static void RegisterFunctions(FunctionCollection functions)
        {
            functions.RegisterAction<KeywordController>("RedirectByKeyword", "Composer.Search.RedirectByKeyword", "${Orckestra.ExperienceManagement.KeywordRedirect,RedirectByKeywordDescription}")
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
