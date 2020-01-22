/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../Composer.UI/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../Composer.UI/Controller.ts' />
/// <reference path='../Composer.UI/IControllerActionContext.ts' />
/// <reference path='../Composer.UI/IControllerContext.ts' />
/// <reference path='./SearchService.ts' />
/// <reference path='./ISearchService.ts' />
///<reference path='../Composer.UI/IControllerActionContext.ts' />
/// <reference path='./UrlHelper.ts' />

module Orckestra.Composer {
    'use strict';

    interface SerializeObject extends JQuery {
        serializeObject(): any;
    }

    export class SelectedFacetSearchController extends Orckestra.Composer.Controller {

        public initialize() {
            super.initialize();
        }

        public removeSelectedFacet(actionContext: IControllerActionContext) {
            var removeFacetButton = actionContext.elementContext;

            actionContext.event.preventDefault();
            actionContext.event.stopPropagation();

            this.eventHub.publish('facetRemoved', {
                data: {
                    facetFieldName: removeFacetButton.data('facetfieldname'),
                    facetValue: removeFacetButton.data('facetvalue'),
                    facetType: removeFacetButton.data('facettype'),
                    facetLandingPageUrl: removeFacetButton.data('facetlandingpageurl')
                }
            });
        }

        public clearSelectedFacets(actionContext: IControllerActionContext) {
            var clearFacetsButton = actionContext.elementContext;

            actionContext.event.preventDefault();
            actionContext.event.stopPropagation();

            this.eventHub.publish('facetsCleared', {
               data: { landingPageUrl: clearFacetsButton.data('landingpageurl') }
            });
        }

        public addSingleSelectCategory(actionContext: IControllerActionContext) {
            var singleSelectCategory = actionContext.elementContext,
                anchorContext = actionContext.elementContext,
                facetFieldName = anchorContext.data('facetfieldname'),
                facetValue = anchorContext.data('facetvalue');

            actionContext.event.preventDefault();
            actionContext.event.stopPropagation();

            this.eventHub.publish('singleCategoryAdded', {
                data: {
                    categoryUrl: singleSelectCategory.data('categoryurl'),
                    facetKey: facetFieldName,
                    facetValue: facetValue,
                    pageType: UrlHelper.resolvePageType()
                }
            });
        }
    }
}
