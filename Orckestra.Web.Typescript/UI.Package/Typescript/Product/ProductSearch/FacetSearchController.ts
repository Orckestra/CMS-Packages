/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
/// <reference path='./Services/SearchService.ts' />
/// <reference path='./Services/ISearchService.ts' />
/// <reference path='./Services/SliderService.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
/// <reference path='./UrlHelper.ts' />

module Orckestra.Composer {
    'use strict';

    interface SerializeObject extends JQuery {
        serializeObject(): any;
    }

    export class FacetSearchController extends Orckestra.Composer.Controller {
        private _debounceHandle; // need to see how to fix the any for this.
        private _debounceTimeout: number = 500;
        private _searchService: ISearchService; // TODO: DI this, constructor injection via controller factory?
        private sliderService: SliderService; // TODO: DI this, constructor injection via controller factory?
        private sliderServicesInstances: IHashTable<SliderService> = {};

        public initialize() {
            super.initialize();
            this.initializeServices();
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
                        pageType: UrlHelper.resolvePageType(),
                        filter: (<ISerializeObjectJqueryPlugin>$('form[name="searchFacets"]', this.context.container)).serializeObject()
                    }
                });
            }, 250);

            this._debounceHandle();
        }

        public dispose() {
            super.dispose();
            Object.keys(this.sliderServicesInstances).forEach(sliderServiceKey => this.sliderServicesInstances[sliderServiceKey].dispose());
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
                    pageType: UrlHelper.resolvePageType()
                }
            });
        }

        public toggleFacetList(actionContext: IControllerActionContext) {
            actionContext.event.preventDefault();

            var buttonContext = actionContext.elementContext,
                label: string = buttonContext.html(),
                showMoreLabel: string = buttonContext.data('label-showmore'),
                showLessLabel: string = buttonContext.data('label-showless');

            buttonContext.html(label === showMoreLabel ? showLessLabel : showMoreLabel);
        }

        public refineByRange(actionContext: IControllerActionContext) {
            actionContext.event.preventDefault();
            var container = actionContext.elementContext.closest('[data-facetfieldname]');
            var sliderServiceInstance = this.sliderServicesInstances[container.data('facetfieldname')];

            var values = sliderServiceInstance.getValues();
            var key = sliderServiceInstance.getKey();

            this.eventHub.publish('singleFacetsChanged', {
                data: {
                    facetKey: key,
                    facetValue: values.join('|')
                }
            });
        }

        private initializeServices() {
            var selectedFacets: IHashTable<string|string[]>;
            var correctedSearchTerm: string = this.context.container.attr('data-corrected-search-term');

            this._searchService = new SearchService(this.eventHub, window);
            this._searchService.initialize({
                facetRegistry: this.buildFacetRegistry(),
                correctedSearchTerm: correctedSearchTerm
            });
            selectedFacets = this._searchService.getSelectedFacets();

            this.context.container.find('[data-facettype="Range"]').each((index, element) => {
                var facetFieldName = $(element).data('facetfieldname');
                var serviceInstance = new SliderService($(element), this.eventHub);

                serviceInstance.initialize(selectedFacets[facetFieldName]);
                this.sliderServicesInstances[facetFieldName] = serviceInstance;
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
