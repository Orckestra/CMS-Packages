/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../../Typescript/Composer.Product/ProductSearch/SearchCriteria.ts' />
/// <reference path='../../../Typescript/Generics/Collections/IHashTable.ts' />
/// <reference path='../../../Typescript/Events/IEventHub.ts' />


(() => {

    'use strict';

    // Visit http://jasmine.github.io for more information on unit testing with Jasmine.
    // For more info on the Karma test runner, visit http://karma-runner.github.io

    describe('SearchCriteria_toQueryString.spec.ts', () => {
        var searchCriteria: Orckestra.Composer.SearchCriteria;
        var eventHub: Orckestra.Composer.IEventHub = Orckestra.Composer.EventHub.instance();


        beforeEach(() => {
            searchCriteria = new Orckestra.Composer.SearchCriteria(eventHub, <Window>{});
        });

        function encodeQuerystringValue(valueToEncode) {
            return encodeURIComponent(valueToEncode).replace(/%20/g, '+');
        }


        describe('WHEN setting the keywords property()', () => {
            it('SHOULD contain keywords=encodedKeyWordsValue', () => {
                var actual: number,
                    someKeyWords: string = 'some keywords';

                searchCriteria.keywords = someKeyWords;
                actual = searchCriteria.toQuerystring().indexOf('keywords=' + encodeQuerystringValue(someKeyWords));

                expect(actual).not.toBe(-1);
            });

            it('SHOULD not contain keywords=keyWordsValue', () => {
                var actual: number,
                    someKeyWords: string = 'some keywords';

                searchCriteria.keywords = someKeyWords;
                actual = searchCriteria.toQuerystring().indexOf('keywords=' + someKeyWords);

                expect(actual).toBe(-1);
            });
        });

        describe('WHEN setting the sortDirection property()', () => {
            it('SHOULD contain sortDirection=sortDirectionValue', () => {
                var actual: number,
                    sortDirection: string = 'asc';

                searchCriteria.sortDirection = sortDirection;
                actual = searchCriteria.toQuerystring().indexOf('sortDirection=' + encodeQuerystringValue(sortDirection));

                expect(actual).not.toBe(-1);
            });
        });

        describe('WHEN setting the sortBy property()', () => {
            it('SHOULD contain sortBy=sortByValue', () => {
                var actual: number,
                    sortBy: string = 'price';

                searchCriteria.sortBy = sortBy;
                actual = searchCriteria.toQuerystring().indexOf('sortBy=' + encodeQuerystringValue(sortBy));

                expect(actual).not.toBe(-1);
            });
        });

        describe('WHEN adding a single facet()', () => {
            it('SHOULD contain fn1=encodedFacetName and fv1=encodedFacetValue', () => {
                var actual: number,
                    facetName: string = 'some facet name',
                    facetValue: string = 'some facet value',
                    facets: Orckestra.Composer.IHashTable<string | string[]> = {};

                facets[facetName] = facetValue;

                searchCriteria.selectedFacets = facets;
                actual = searchCriteria.toQuerystring().indexOf('fn1=' + encodeQuerystringValue(facetName));
                expect(actual).not.toBe(-1);

                actual = searchCriteria.toQuerystring().indexOf('fv1=' + encodeQuerystringValue(facetValue));
                expect(actual).not.toBe(-1);
            });

            it('SHOULD not contain fn1=facetName and fv1=facetValue', () => {
                var actual: number,
                    facetName: string = 'some facet name',
                    facetValue: string = 'some facet value',
                    facets: Orckestra.Composer.IHashTable<string | string[]> = {};

                facets[facetName] = facetValue;

                searchCriteria.selectedFacets = facets;
                actual = searchCriteria.toQuerystring().indexOf('fn1=' + facetName);
                expect(actual).toBe(-1);

                actual = searchCriteria.toQuerystring().indexOf('fv1=' + facetValue);
                expect(actual).toBe(-1);
            });
        });

        describe('WHEN adding a multi-facet with multiple values()', () => {
            it('SHOULD contain fn1=encodedFacetName and fv1=encodedFacetValue1-EncodedPipe' +
                                    '-EncodedFacetValue2-EncodedPipe-EncodedFacetValueN.', () => {
                var actual: number,
                    facetName: string = 'some facet name',
                    facetValue1: string = 'some facet value',
                    facetValue2: string = 'some facet value 2',
                    facets: Orckestra.Composer.IHashTable<string | string[]> = {};

                facets[facetName] = [facetValue1, facetValue2];

                searchCriteria.selectedFacets = facets;
                actual = searchCriteria.toQuerystring().indexOf('fn1=' + encodeQuerystringValue(facetName));
                expect(actual).not.toBe(-1);

                actual = searchCriteria.toQuerystring().indexOf('fv1=' + encodeQuerystringValue(facetValue1 + '|' + facetValue2));
                expect(actual).not.toBe(-1);
            });

            it('SHOULD note contain fn1=facetName and fv1=facetValue1|facetValue2|facetValueN.', () => {
                var actual: number,
                    facetName: string = 'some facet name',
                    facetValue1: string = 'some facet value',
                    facetValue2: string = 'some facet value 2',
                    facets: Orckestra.Composer.IHashTable<string | string[]> = {};

                facets[facetName] = [facetValue1, facetValue2];

                searchCriteria.selectedFacets = facets;
                actual = searchCriteria.toQuerystring().indexOf('fn1=' + facetName);
                expect(actual).toBe(-1);

                actual = searchCriteria.toQuerystring().indexOf('fv1=' + facetValue1 + '|' + facetValue2);
                expect(actual).toBe(-1);
            });
        });
    });
})();
