/// <reference path='../../../../Typings/tsd.d.ts' />
/// <reference path='../../../Events/IEventHub.ts' />
/// <reference path='../../../Events/IEventInformation.ts' />
/// <reference path='../SearchCriteria.ts' />
/// <reference path='./ISearchService.ts' />
/// <reference path='../IFacet.ts' />
/// <reference path='../ISingleSelectCategory.ts' />
///
module Orckestra.Composer {
    'use strict';

    // TODO: Decouple window object from search service.
    export class SearchService implements ISearchService {
        private _searchCriteria: SearchCriteria;
        private _baseSearchUrl: string = window.location.href.replace(window.location.search, '');
        private _baseUrl: string = this._baseSearchUrl.replace(window.location.pathname, '');
        private _facetRegistry: IHashTable<string> = {};

        constructor(private _eventHub: IEventHub, private _window: Window) {
             this._searchCriteria = new SearchCriteria(_eventHub, _window);
        }

        /**
         * Initializes the search service.
         *
         * param facetRegistry Facets available to the search service.
         */
        public initialize(options: ISearchCriteriaOptions) {
            this.registerSubscriptions();
            this._searchCriteria.initialize(options);
        }

        public singleFacetsChanged(eventInformation: IEventInformation) {
            var facetKey: string = eventInformation.data.facetKey,
                facetValue: string = eventInformation.data.facetValue;

            this._searchCriteria.addSingleFacet(facetKey, facetValue);
            this.search();
        }

        public sortingChanged(eventInformation: IEventInformation) {
            var dataUrl = eventInformation.data.url;
            this._window.location.href = dataUrl;
        }

        public getSelectedFacets(): IHashTable<string|string[]> {
            return this._searchCriteria.selectedFacets;
        }

        public multiFacetChanged(eventInformation: IEventInformation) {
            this._searchCriteria.updateMultiFacets(eventInformation.data.filter);
            this.search();
        }

        public clearFacets(eventInformation: IEventInformation) {
            var landingPageUrl: string = eventInformation.data.landingPageUrl;

            this._searchCriteria.clearFacets();

            if (landingPageUrl) {
                this._baseSearchUrl = landingPageUrl;
            }

            this.search();
        }

        public removeFacet(eventInformation: IEventInformation) {
            var facet: IFacet = <IFacet>eventInformation.data;

            this._searchCriteria.removeFacet(facet);

            if (facet.facetLandingPageUrl && facet.facetType === 'SingleSelect') {
                this._baseSearchUrl = facet.facetLandingPageUrl;
            }

            this.search();
        }

        public addSingleSelectCategory(eventInformation: IEventInformation) {
            var singleSelectCategory: ISingleSelectCategory = <ISingleSelectCategory>eventInformation.data;

            this._baseSearchUrl = singleSelectCategory.categoryUrl;

            this.search();
        }

        private registerSubscriptions() {
            this._eventHub.subscribe('sortingChanged', this.sortingChanged.bind(this));
            this._eventHub.subscribe('singleFacetsChanged', this.singleFacetsChanged.bind(this));
            this._eventHub.subscribe('multiFacetChanged', this.multiFacetChanged.bind(this));
            this._eventHub.subscribe('facetsCleared', this.clearFacets.bind(this));
            this._eventHub.subscribe('facetRemoved', this.removeFacet.bind(this));
            this._eventHub.subscribe('singleCategoryAdded', this.addSingleSelectCategory.bind(this));
        }

        private search() {
            this._window.location.href = this._baseSearchUrl + this._searchCriteria.toQuerystring();
        }
    }
}
