/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
/// <reference path='./UrlHelper.ts' />

module Orckestra.Composer {
    'use strict';

    export class SortBySearchController extends Orckestra.Composer.Controller {

        public sortingChanged(actionContext: IControllerActionContext) {
            var anchorContext = actionContext.elementContext,
                dataSortingType = anchorContext.data('sorting'),
                dataUrl = anchorContext.data('url'),
                resolvePageType = UrlHelper.resolvePageType();

            actionContext.event.preventDefault();
            actionContext.event.stopPropagation();

            this.eventHub.publish('sortingChanged', {
                data: {
                    sortingType: dataSortingType,
                    pageType: resolvePageType,
                    url: dataUrl
                }
            });
        }
    }
}
