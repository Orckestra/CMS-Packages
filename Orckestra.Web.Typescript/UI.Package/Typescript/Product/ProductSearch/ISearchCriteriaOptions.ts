/// <reference path='../../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    export interface ISearchCriteriaOptions {
        facetRegistry: IHashTable<string>;
        correctedSearchTerm?: string;
    }
}
