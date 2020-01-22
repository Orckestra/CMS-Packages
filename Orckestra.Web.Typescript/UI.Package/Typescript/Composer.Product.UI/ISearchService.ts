/// <reference path='../Composer.UI/IEventInformation.ts' />
/// <reference path='../Composer.UI/IHashTable.ts' />
/// <reference path='./ISearchCriteriaOptions.ts' />

module Orckestra.Composer {
    export interface ISearchService {
        initialize(options: ISearchCriteriaOptions);

        singleFacetsChanged(eventInformation: IEventInformation);

        multiFacetChanged(eventInformation: IEventInformation);

        clearFacets(eventInformation: IEventInformation);

        removeFacet(eventInformation: IEventInformation);

        getSelectedFacets(): IHashTable<string|string[]>;
    }
}
