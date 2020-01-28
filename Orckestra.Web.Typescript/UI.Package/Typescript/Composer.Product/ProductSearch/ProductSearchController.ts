/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
/// <reference path='./Services/SearchService.ts' />
/// <reference path='./Services/ISearchService.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />

module Orckestra.Composer {
    'use strict';

    interface SerializeObject extends JQuery {
        serializeObject(): any;
    }

    export class ProductSearchController extends Orckestra.Composer.Controller {
        private _debounceHandle; // need to see how to fix the any for this.
        private _debounceTimeout: number = 500;
        private _searchService: ISearchService; // TODO: DI this, constructor injection via controller factory?

        public initialize() {
            super.initialize();
            this.initializeSearchService();
        }

        public multiFacetChanged(actionContext: IControllerActionContext) {
            if (!_.isEmpty(this._debounceHandle)) {
                this._debounceHandle.cancel();
            }

            var anchorContext = actionContext.elementContext,
                facetFieldName = anchorContext.attr('name'),
                facetValue = anchorContext.attr('value');

            this._debounceHandle = _.debounce(() => {
                this.eventHub.publish('multiFacetChanged', {
                    data: {
                        facetKey: facetFieldName,
                        facetValue: facetValue,
                        pageType: 'browse',
                        filter: (<ISerializeObjectJqueryPlugin>$('form[name="searchFacets"]', this.context.container)).serializeObject()
                    }
                });
            }, 250);

            this._debounceHandle();
        }

        public singleFacetChanged(actionContext: IControllerActionContext) {
            var anchorContext = actionContext.elementContext,
                facetFieldName = anchorContext.data('facetfieldname'),
                facetValue = anchorContext.data('facetvalue'),
                facets = {};

            actionContext.event.preventDefault();
            actionContext.event.stopPropagation();

            this.eventHub.publish('singleFacetsChanged', {
                data: {
                    facetKey: facetFieldName,
                    facetValue: facetValue,
                    pageType: 'browse'
                }
            });
        }

        public removeSelectedFacet(actionContext: IControllerActionContext) {
            var removeFacetButton = actionContext.elementContext;

            actionContext.event.preventDefault();
            actionContext.event.stopPropagation();

            this.eventHub.publish('facetRemoved', {
                data: {
                    facetFieldName: removeFacetButton.data('facetfieldname'),
                    facetValue: removeFacetButton.data('facetvalue'),
                    facetType: removeFacetButton.data('facettype')
                }
            });
        }

        public clearSelectedFacets(actionContext: IControllerActionContext) {
            actionContext.event.preventDefault();
            actionContext.event.stopPropagation();

            this.eventHub.publish('facetsCleared', null);
        }

        private initializeSearchService() {
            this._searchService = new SearchService(this.eventHub, window);
            this._searchService.initialize({
                facetRegistry: this.buildFacetRegistry()
            });
        }

        private buildFacetRegistry(): IHashTable<string> {
            var facetRegistry: IHashTable<string> = {};

            $('[data-facettype]', this.context.container)
                .add($('#selectedFacets [data-facetfieldname]', this.context.container))
                .each((index: number, item: HTMLElement) => {
                    var facetType: string,
                        facetFieldName: string,
                        facetGroup: JQuery = $(item);

                    facetFieldName = facetGroup.data('facetfieldname');
                    facetType = (<string>facetGroup.data('facettype')).toLowerCase();
                    facetRegistry[facetFieldName] = facetType;
                });

            return facetRegistry;
        }
    }
}
