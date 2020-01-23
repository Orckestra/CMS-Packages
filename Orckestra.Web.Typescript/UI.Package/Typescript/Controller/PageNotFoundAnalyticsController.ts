///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Mvc/Controller.ts' />

module Orckestra.Composer {
    export class PageNotFoundAnalyticsController extends Controller {
        public initialize() {
            super.initialize();

            var pageUrl = decodeURIComponent(urlHelper.getURLParameter(window.location.href, 'errorpath'));

            if (!pageUrl || pageUrl === 'null') {
                pageUrl = window.location.href;
            }

            this.eventHub.publish('pageNotFound', { data: { PageUrl: pageUrl, ReferrerUrl: document.referrer } });

        }
    }

}