///<reference path='./IFacet.ts' />

module Orckestra.Composer {
    export interface ISelectedFacet {
        facetFieldName: string;
        selectedValues: string[];
        isFacetArray: boolean;
    }
}
