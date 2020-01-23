///<reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    export class AutocompleteSearchService extends SearchService {
        private categoryFacet = /^CategoryLevel(\d+)_Facet$/;

        public initialize(options: Orckestra.Composer.ISearchCriteriaOptions) {
            super.initialize(options);
            this['_eventHub'].subscribe('categorySuggestionClicked', this.categorySuggestionClicked.bind(this));
            this['_eventHub'].subscribe('brandSuggestionClicked', this.brandSuggestionClicked.bind(this));
        }

        public singleFacetsChanged(eventInformation: Orckestra.Composer.IEventInformation) {
            var facetKey: string = eventInformation.data.facetKey,
                facetValue: string = eventInformation.data.facetValue;
            var matches = facetKey.match(/\d+/);
            if (matches) {
                var selectedCategoryRank = +matches[0];
                var selectedFacets = this['_searchCriteria']['selectedFacets'];
                Object.keys(selectedFacets).filter((facet) => {
                    var categoryFacetMatches = facet.match(this.categoryFacet);
                    return categoryFacetMatches && +categoryFacetMatches[1] > selectedCategoryRank;
                }).forEach((facet) => {
                    delete selectedFacets[facet];
                });
            }
            this['_searchCriteria'].addSingleFacet(facetKey, facetValue);
            this['search']();
        }

        public removeCategories() {
            var selectedFacets = this['_searchCriteria']['selectedFacets'];
            Object.keys(selectedFacets).filter((facet) => {
                return this.categoryFacet.test(facet);
            }).forEach((facet) => {
                delete selectedFacets[facet];
            });
            this['search']();
        }

        public categorySuggestionClicked(eventInformation: Orckestra.Composer.IEventInformation) {
            this['_searchCriteria'].clearFacets();
            var suggestion = eventInformation.data.suggestion;
            var parents = eventInformation.data.parents;
            for (let i = 0; i < parents.length; ++i) {
                this['_searchCriteria'].addSingleFacet(`CategoryLevel${i + 1}_Facet`, parents[i]);
            }
            this['_searchCriteria'].addSingleFacet(`CategoryLevel${parents.length + 1}_Facet`, suggestion);
            this['_searchCriteria'].keywords = '*';
            this['_searchCriteria'].correctedSearchTerm = '*';
            this['search']();
        }

        public brandSuggestionClicked(eventInformation: Orckestra.Composer.IEventInformation) {
            var suggestion = eventInformation.data.suggestion;
            this['_searchCriteria'].clearFacets();
            this['_searchCriteria'].addSingleFacet('Brand', suggestion);
            this['_searchCriteria'].keywords = '*';
            this['_searchCriteria'].correctedSearchTerm = '*';
            this['search']();
        }
    }
}
