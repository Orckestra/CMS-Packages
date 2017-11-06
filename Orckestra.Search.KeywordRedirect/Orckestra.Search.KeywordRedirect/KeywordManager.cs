using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Composite.Core.Application;
using Composite.Core.Extensions;
using Composite.Core.Logging;
using Composite.Data;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Data.PublishScheduling;
using Microsoft.Extensions.DependencyInjection;
using Orckestra.Search.KeywordRedirect.Data.Types;
using Orckestra.Search.KeywordRedirect.Endpoint;

namespace Orckestra.Search.KeywordRedirect
{
    [ApplicationStartup()]
    internal class KeywordManagerRegistrator
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(typeof(KeywordManager));
        }
    }

    public class KeywordManager : IObserver<KeywordChange>
    {

        public KeywordManager(KeywordChangeNotifier changeNotifier, ILog log)
        {
            _log = log;
            _notifierUnsubscriber = changeNotifier.Subscribe(this);
        }

        private readonly ILog _log;
        private IDisposable _notifierUnsubscriber;
        private Dictionary<CultureInfo, List<Model.RedirectKeyword>> _keywordsCache = new Dictionary<CultureInfo, List<Model.RedirectKeyword>>();
        private Dictionary<CultureInfo, Dictionary<string, string>> _keywordRedirectCache = new Dictionary<CultureInfo, Dictionary<string, string>>();

        public void OnCompleted()
        {
            _log.LogWarning(nameof(KeywordManager), "Unexpected KeywordChange OnCompleted called");
        }

        public void OnError(Exception error)
        {
            _log.LogError(nameof(KeywordManager), error);
        }

        public void OnNext(KeywordChange value)
        {
            _keywordsCache.Remove(value.CultureInfo);
            _keywordRedirectCache.Remove(value.CultureInfo);
        }

        public IEnumerable<Model.RedirectKeyword> GetKeywordRedirects(CultureInfo cultureInfo)
        {

            var result = _keywordsCache.ContainsKey(cultureInfo) ? _keywordsCache[cultureInfo] : null;
            if (result == null)
            {
                result = new List<Model.RedirectKeyword>();
                using (var connection = new DataConnection(PublicationScope.Unpublished, cultureInfo))
                {
                    foreach (var redirectKeyword in connection.Get<RedirectKeyword>())
                    {
                        var interfaceType = redirectKeyword.DataSourceId.InterfaceType;
                        var stringKey = redirectKeyword.GetUniqueKey().ToString();
                        var locale = redirectKeyword.DataSourceId.LocaleScope.Name;

                        var existingPublishSchedule = PublishScheduleHelper.GetPublishSchedule(interfaceType, stringKey, locale);
                        var existingUnpublishSchedule = PublishScheduleHelper.GetUnpublishSchedule(interfaceType, stringKey, locale);

                        if (redirectKeyword.PublicationStatus == GenericPublishProcessController.Published)
                        {
                            result.Add(new Model.RedirectKeyword
                            {
                                Keyword = redirectKeyword.Keyword,
                                LandingPage = KeywordFacade.GetPageUrl(redirectKeyword.LandingPage, cultureInfo),
                                PublishDate = existingPublishSchedule?.PublishDate.ToTimeZoneDateTimeString(),
                                UnpublishDate = existingUnpublishSchedule?.UnpublishDate.ToTimeZoneDateTimeString(),

                            });
                        }
                        else
                        {
                            var publishedredirectKeyword = DataFacade.GetDataFromOtherScope(redirectKeyword, DataScopeIdentifier.Public).FirstOrDefault();

                            result.Add(new Model.RedirectKeyword
                            {
                                Keyword = publishedredirectKeyword?.Keyword,
                                LandingPage =
                                    publishedredirectKeyword != null
                                        ? KeywordFacade.GetPageUrl(publishedredirectKeyword.LandingPage, cultureInfo)
                                        : null,
                                KeywordUnpublished = redirectKeyword.Keyword,
                                LandingPageUnpublished = KeywordFacade.GetPageUrl(redirectKeyword.LandingPage, cultureInfo),
                                PublishDate = existingPublishSchedule?.PublishDate.ToTimeZoneDateTimeString(),
                                UnpublishDate = existingUnpublishSchedule?.UnpublishDate.ToTimeZoneDateTimeString(),
                            });
                        }

                    }
                }
                _keywordsCache[cultureInfo] = result;
            }
            return result;
        }

        public string GetPublicRedirect(string keyword, CultureInfo cultureInfo)
        {
            if (keyword == null)
            {
                return null;
            }

            if (!_keywordRedirectCache.ContainsKey(cultureInfo))
            {
                _keywordRedirectCache[cultureInfo] = new Dictionary<string, string>();
            }

            var result = _keywordRedirectCache[cultureInfo].ContainsKey(keyword) ? _keywordRedirectCache[cultureInfo][keyword] : null;
            if (result == null)
            {

                using (var connection = new DataConnection(PublicationScope.Published, cultureInfo))
                {
                    var landingPage = connection.Get<RedirectKeyword>().Where(d => d.Keyword.Equals(keyword, StringComparison.InvariantCultureIgnoreCase)).Select(d => d.LandingPage).FirstOrDefault();
                    if (landingPage != Guid.Empty)
                    {
                        result = Composite.Core.Routing.PageUrls.BuildUrl(new Composite.Core.Routing.PageUrlData(landingPage, PublicationScope.Published, System.Threading.Thread.CurrentThread.CurrentCulture));
                        _keywordRedirectCache[cultureInfo][keyword] = result;
                    }
                }
            }
            return result;
        }
    }
}
