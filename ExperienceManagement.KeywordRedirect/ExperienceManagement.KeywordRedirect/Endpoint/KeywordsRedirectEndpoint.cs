using Composite.Core.Application;
using Composite.Core.WebClient.Services.WampRouter;
using System;
using WampSharp.V2.Rpc;
using Composite.Data;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Composite.C1Console.Users;
using System.Threading;
using Composite.Core;
using Orckestra.ExperienceManagement.KeywordRedirect;

namespace Orckestra.ExperienceManagement.KeywordRedirect
{
    [ApplicationStartup]
    public class KeywordsRedirectEndpoint
    {
        public static void OnInitialized(KeywordManager keywordManager, KeywordChangeNotifier componentChangeNotifier)
        {
            WampRouterFacade.RegisterCallee(new KeywordsRpcService(keywordManager));
            WampRouterFacade.RegisterPublisher(new KeywordsPublisher(componentChangeNotifier));
        }
    }

    public class KeywordsRpcService : IRpcService
    {
        KeywordManager _keywordManager;

        internal KeywordsRpcService(KeywordManager keywordManager)
        {
            _keywordManager = keywordManager;
        }

        /// <summary>
        /// To get all components
        /// </summary>
        /// <returns>list of keywords</returns>
        [WampProcedure("keywords.get")]
        public async Task<IEnumerable<Model.RedirectKeyword>> GetKeywords()
        {
            try
            {
                return _keywordManager.GetKeywordRedirects(UserSettings.ActiveLocaleCultureInfo);
            }
            catch (Exception e) {
                Log.LogError(nameof(KeywordsRpcService), e);
            }
            return null;
        }
    }

    /// <summary>
    /// Publisher for interaction with components
    /// </summary>
    public class KeywordsPublisher : IWampEventHandler<KeywordChange, bool>
    {
        private readonly KeywordChangeNotifier _componentChangeNotifier;

        /// <summary>
        /// Change in keywords topic
        /// </summary>
        public static string Topic => "keywords.new";

        string IWampEventHandler<KeywordChange, bool>.Topic => Topic;

        /// <summary>
        /// Event to observe when there is any change in components
        /// </summary>
        public IObservable<KeywordChange> Event => _componentChangeNotifier;

        internal KeywordsPublisher(KeywordChangeNotifier componentChangeNotifier)
        {
            _componentChangeNotifier = componentChangeNotifier;
        }

        /// <summary>
        /// Data returning after any change happens in keywords
        /// </summary>
        public bool GetNewData()
        {
            return true;
        }
    }
}
