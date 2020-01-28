/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../../Typescript/Composer.Product/ProductSearch/SearchCriteria.ts' />
/// <reference path='../../../Typescript/Generics/Collections/IHashTable.ts' />
/// <reference path='../../../Typescript/Events/IEventHub.ts' />

(() => {
    'use strict';

    // Visit http://jasmine.github.io for more information on unit testing with Jasmine.
    // For more info on the Karma test runner, visit http://karma-runner.github.io

    describe('SearchCriteria_constructor.spec.ts', () => {
        describe('WHEN instantiating SearchCriteria', () => {
            var searchCriteria: Orckestra.Composer.SearchCriteria;
            var eventHub: Orckestra.Composer.IEventHub = Orckestra.Composer.EventHub.instance();

            beforeEach(() => {

                searchCriteria = new Orckestra.Composer.SearchCriteria(eventHub, <Window>{});
            });

            it('SHOULD have it\'s keywords property set to an empty string.', () => {
                var actual: string,
                    expected: string;

                actual = searchCriteria.keywords;
                expected = '';

                expect(actual).toBe(expected);
            });

            it('SHOULD have it\'s page property set to 1.', () => {
                var actual: number,
                    expected: number;

                actual = searchCriteria.page;
                expected = 1;

                expect(actual).toBe(expected);
            });

            it('SHOULD have it\'s sortBy property set to an empty string.', () => {
                var actual: string,
                    expected: string;

                actual = searchCriteria.sortBy;
                expected = '';

                expect(actual).toBe(expected);
            });

            it('SHOULD have it\'s sortDirection property set to an empty string.', () => {
                var actual: string,
                    expected: string;

                actual = searchCriteria.sortDirection;
                expected = '';

                expect(actual).toBe(expected);
            });

            it('SHOULD not have any selected facets.', () => {
                var actual: number,
                    expected: number;

                actual = Object.keys(searchCriteria.selectedFacets).length;
                expected = 0;

                expect(actual).toBe(expected);
            });
        });
    });
})();
