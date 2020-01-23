/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='./IFacet.ts' />
/// <reference path='./ISelectedFacet.ts' />
/// <reference path='../../Generics/Collections/IHashTable.ts' />
/// <reference path='./ISearchCriteriaOptions.ts' />

module Orckestra.Composer {
    export class SearchCriteria {
        private static facetFieldNameKeyPrefix: string = 'fn';
        private static facetValueKeyPrefix: string = 'fv';

        private _facetRegistry: IHashTable<string> = {};
        public keywords: string = '';
        public correctedSearchTerm: string;
        public page: number = 1;
        public sortBy: string = '';
        public sortDirection: string = '';
        public selectedFacets: IHashTable<string|string[]> = {};

        constructor(private eventHub, private _window: Window) {
        }

        public initialize(options: ISearchCriteriaOptions) {
            this._facetRegistry = options.facetRegistry;
            this.correctedSearchTerm = options.correctedSearchTerm;

            this.loadFromQuerystring(this._window.location.search);
        }

        public loadFromQuerystring(querystring: string) {
            this.loadNonFacetCriteria(querystring);
            this.loadFacetCriteria(querystring);
        }

        public toQuerystring(): string {
            var queryBuilder: string[] = [],
                facetKey: string,
                facetIndex: number = 1,
                facetValue: any,
                selectedFacets: IHashTable<string|string[]> = this.selectedFacets;

            if (!_.isEmpty(this.keywords) ||
                !_.isEmpty(this.sortBy) ||
                !_.isEmpty(this.sortDirection) ||
                !_.isEmpty(this.page) && this.page > 1 ||
                !_.isEmpty(this.selectedFacets)) {
                    queryBuilder.push('?');
                }

            if (!_.isEmpty(this.keywords) || !_.isEmpty(this.correctedSearchTerm)) {
                queryBuilder.push('keywords=');
                var keyword = _.isEmpty(this.correctedSearchTerm) ? this.keywords : this.correctedSearchTerm;
                queryBuilder.push(this.encodeQuerystringValue(keyword));
            }

            if (!_.isEmpty(this.sortBy)) {
                queryBuilder.push('&sortBy=');
                queryBuilder.push(this.encodeQuerystringValue(this.sortBy));
            }

            if (!_.isEmpty(this.sortDirection)) {
                queryBuilder.push('&sortDirection=');
                queryBuilder.push(this.encodeQuerystringValue(this.sortDirection));
            }

            if (!_.isEmpty(this.page) && this.page > 1) {
                queryBuilder.push('&page=');
                queryBuilder.push(this.page.toString());
            }

            for (facetKey in selectedFacets) {
                if (selectedFacets.hasOwnProperty(facetKey)) {
                    facetValue = selectedFacets[facetKey];

                    queryBuilder.push('&');
                    queryBuilder.push(SearchCriteria.facetFieldNameKeyPrefix);
                    queryBuilder.push(facetIndex.toString());
                    queryBuilder.push('=');
                    queryBuilder.push(this.encodeQuerystringValue(facetKey));
                    queryBuilder.push('&');
                    queryBuilder.push(SearchCriteria.facetValueKeyPrefix);
                    queryBuilder.push(facetIndex.toString());
                    queryBuilder.push('=');

                    queryBuilder.push(this.encodeQuerystringValue(_.isArray(facetValue) ? facetValue.join('|') : facetValue));
                    facetIndex++;
                }
            }

            return queryBuilder.join('');
        }

        public clearFacets() {
            this.resetPaging();
            this.selectedFacets = {};
        }

        public addSingleFacet(facetKey: string, facetValue: string) {
            this.selectedFacets[facetKey] = facetValue;
        }

        public updateMultiFacets(facets: IHashTable<string|string[]>) {
            var facetKey: string,
                facetValues: string[];

            this.resetPaging();
            this.clearSelectedMultiFacets();

            for (facetKey in facets) {
                if (facets.hasOwnProperty(facetKey)) {
                    this.selectedFacets[facetKey] = [];

                    facetValues = (typeof facets[facetKey] === 'string' ?
                        [<string>facets[facetKey]] : <string[]>facets[facetKey]);

                    facetValues.forEach((value: string, index: number, array: string[]) => {
                        (<string[]>this.selectedFacets[facetKey]).push(value);
                    });
                }
            }
        }

        public removeFacet(facetToRemove: Orckestra.Composer.IFacet) {
            var facet: ISelectedFacet;
            this.resetPaging();

            if (this.selectedFacets.hasOwnProperty(facetToRemove.facetFieldName)) {
                facet = this.getSelectedFacetsArray(facetToRemove.facetFieldName);

                if (facetToRemove.facetType.toLowerCase() === 'range') {
                    facet.selectedValues = undefined;
                } else {
                    // to string in case facetValue is a number
                    facet.selectedValues = _.without(facet.selectedValues, facetToRemove.facetValue.toString());
                }
                this.setSelectedFacet(facet);
            }
        }

        private getSelectedFacetsArray(facetFieldName: string) : ISelectedFacet {
            var isSelectedFacetArray: boolean;
            var selectedFacet: any = this.selectedFacets[facetFieldName];
            var selectedFacetArray: Array<string>;

            if (_.isArray(selectedFacet)) {
                isSelectedFacetArray = true;
                selectedFacetArray = selectedFacet;
            } else if (_.isString(selectedFacet)) {
                isSelectedFacetArray = false;
                selectedFacetArray = (<string>selectedFacet).split('|');
            } else {
                throw new Error(`The selected facet ${facetFieldName} is not an array or a string`);
            }

            return {
                facetFieldName: facetFieldName,
                selectedValues: selectedFacetArray,
                isFacetArray: isSelectedFacetArray
            };
        }

        private setSelectedFacet(selectedFacet: ISelectedFacet): void {
            var facetStr: string = '';

            if (_.isEmpty(selectedFacet.selectedValues)) {
                delete this.selectedFacets[selectedFacet.facetFieldName];
            } else {
                if (selectedFacet.isFacetArray) {
                    this.selectedFacets[selectedFacet.facetFieldName] = selectedFacet.selectedValues;
                } else {
                    _.each(selectedFacet.selectedValues, (v: string) => {
                        if (!_.isEmpty(facetStr)) {
                            facetStr = facetStr + '|';
                        }

                        facetStr = facetStr + v;
                    });

                    this.selectedFacets[selectedFacet.facetFieldName] = facetStr;
                }
            }
        }

        private clearSelectedMultiFacets() {
            var selectedFacets = this.selectedFacets,
                facetKey: string,
                facetKeysToDelete: string[] = [];

            for (facetKey in selectedFacets) {
                if (selectedFacets.hasOwnProperty(facetKey) && this._facetRegistry[facetKey] === 'multiselect') {
                    facetKeysToDelete.push(facetKey);
                }
            }

            facetKeysToDelete.forEach((facetKey: string) => {
                delete this.selectedFacets[facetKey];
            });
        }

        private resetPaging() {
            this.page = 1;
        }

        private loadFacetCriteria(querystring: string) {
            // TODO: Don't need to loop over querystring again. Should
            // be processing this in the same loop as the non-facet criteria.
            var facetFieldName: string,
                facetValue: string,
                key: string,
                keys: any = {},
                keyValues: any = {};

            if (querystring.length === 0) {
                return;
            }

            querystring.substring(1).split('&').forEach((value, index, array) => {
                var keyValue: string[] = value.split('='),
                    keyFound: string,
                    valueFound: string;

                if (keyValue.length === 2) {
                    keyFound = keyValue[0].toLowerCase();
                    valueFound = this.decodeQuerystringValue(keyValue[1]);

                    if (keyFound.indexOf(SearchCriteria.facetFieldNameKeyPrefix) === 0) {
                        keys[keyFound.replace(SearchCriteria.facetFieldNameKeyPrefix, '')] = valueFound;
                    }

                    if (keyFound.indexOf(SearchCriteria.facetValueKeyPrefix) === 0) {
                        keyValues[keyFound.replace(SearchCriteria.facetValueKeyPrefix, '')] = valueFound;
                    }
                }
            });

            for (key in keys) {
                if (keys.hasOwnProperty(key)) {
                    facetFieldName = this.decodeQuerystringValue(keys[key]);

                    if (keyValues.hasOwnProperty(key)) {
                        facetValue = this.decodeQuerystringValue(keyValues[key]);

                        switch (this._facetRegistry[facetFieldName]) {
                            case 'multiselect':
                                this.selectedFacets[facetFieldName] = facetValue.split('|');
                                break;

                            default:
                                this.selectedFacets[facetFieldName] = facetValue;
                                break;
                        }
                    }
                }
            }
        }

        private loadNonFacetCriteria(querystring: string) {
            if (querystring.length === 0) {
                return;
            }

            querystring.substring(1).split('&').forEach((value, index, array) => {
                var keyValue: string[] = value.split('='),
                    keyFound: string,
                    valueFound: string;

                if (keyValue.length === 2) {
                    keyFound = keyValue[0].toUpperCase();
                    valueFound = this.decodeQuerystringValue((keyValue[1] + ''));

                    switch (keyFound) {
                        case 'KEYWORDS':
                            this.keywords = valueFound;
                            break;

                        case 'SORTBY':
                            this.sortBy = valueFound;
                            break;

                        case 'SORTDIRECTION':
                            this.sortDirection = valueFound;
                            break;
                        case 'PAGE':
                            this.page = parseInt(valueFound, 10);
                            break;
                        default:
                            break;
                    }
                }
            });
        }

        private encodeQuerystringValue(valueToEncode: string) {
            return encodeURIComponent(valueToEncode).replace(/%20/g, '+');
        }

        private decodeQuerystringValue(valueToDecode) {
            return decodeURIComponent(valueToDecode).replace(/\+/g, ' ');
        }
    }
}
