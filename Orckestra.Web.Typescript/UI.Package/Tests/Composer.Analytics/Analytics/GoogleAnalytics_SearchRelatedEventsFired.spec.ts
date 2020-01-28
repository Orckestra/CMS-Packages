///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mocks/MockControllerContext.ts' />
///<reference path='../../../Typescript/Composer.Analytics/Analytics/GoogleAnalyticsPlugin.ts' />
///<reference path='../../../Typescript/Composer.Analytics/Analytics/AnalyticsPlugin.ts' />

module Orckestra.Composer {
    (() => {

        'use strict';

        function CreateSearchResultSample(pricing: any) {
            let searchResult = {
                ListName : 'Search Results',
                PageNumber : 1,
                MaxItemsPerPage : 1,

                ProductSearchResults: [ {
                            ProductId : '3883909',
                            HasVariants : true,
                            Sku : '87390991',
                            DisplayName : 'POLO RALPH LAUREN Mesh Polo Mini Shirtdress',
                            FullDisplayName : 'POLO RALPH LAUREN Mesh Polo Mini Shirtdress',
                            Brand : 'POLO RALPH LAUREN',
                            BrandId : null,
                            DefinitionName : 'RetailWithVariant',
                            Description : 'Featuring a two-button placket and Ralph Lauren\'s signature embroidered pony',
                            Url : 'https://localhost:44300/en-CA/p-polo-ralph-lauren-mesh-polo-mini-shirtdress/3883909',
                            ImageUrl : 'https://az743273.vo.msecnd.net/images/3883909_0_M.jpg',
                            FallbackImageUrl : 'https://az743273.vo.msecnd.net/images/image_not_found.jpg',
                            Pricing : pricing,
                            IsAvailableToSell : true,
                            CategoryId : 'womens_dresses',
                            JsonContext : '{}'
                        }
                    ]
            };

            return searchResult;
        };

        function CreateExpectedAnalyticsProducts(searchResult: any, price: number) {
            var analyticsProducts : IAnalyticsProduct[] = [ {
                id: searchResult.ProductSearchResults[0].ProductId,
                name: searchResult.ProductSearchResults[0].DisplayName,
                price: price,
                brand: searchResult.ProductSearchResults[0].Brand,
                category: searchResult.ProductSearchResults[0].CategoryId,
                list: searchResult.ListName,
                position: (0 + 1) + (searchResult.MaxItemsPerPage * (searchResult.PageNumber - 1))
                }
            ];

            return analyticsProducts;
        };

        describe('WHEN NO Search Results are returned', () => {

            let analytics : GoogleAnalyticsPlugin;
            let eventHub: IEventHub;

            analytics = new GoogleAnalyticsPlugin();
            analytics.initialize();
            eventHub = EventHub.instance();

            it('SHOULD fire the no results analytics trigger', () => {
                // arrange
                const notFoundKeyword = 'Hello';
                spyOn(analytics, 'noResultsFound');

                // act -- publish
                eventHub.publish('noResultsFound', {
                    data : {
                        Keyword : notFoundKeyword,
                    }}
                );

                // assert -- does it match
                expect(analytics.noResultsFound).toHaveBeenCalledWith(notFoundKeyword);
            });
        });

        describe('WHEN Search Results are returned', () => {

            let analytics : GoogleAnalyticsPlugin;
            let eventHub: IEventHub;
            let dummyProducts: IAnalyticsProduct[];

            beforeEach (() => {
                analytics = new GoogleAnalyticsPlugin();
                analytics.initialize();
                eventHub = EventHub.instance();
            });

            describe('WITH product with regular price', () => {

                var regularPricing = {
                    DisplayPrice : '$165.00',
                    DisplaySpecialPrice : null,
                    HasPriceRange : false,
                    IsOnSale : false,
                    ListPrice : 165,
                    Price : null,
                    PriceListId : 'DEFAULT',
                    JsonContext : '{}'
                };

                var searchResult = CreateSearchResultSample(regularPricing);

                var expectedAnalyticsProducts = CreateExpectedAnalyticsProducts(searchResult, regularPricing.ListPrice);

                it('SHOULD fire the productimpressions trigger', () => {
                    // arrange
                    spyOn(analytics, 'productImpressions');

                    // act -- publish
                    eventHub.publish('searchResultRendered', {data: searchResult});

                    // assert -- does it match
                    expect(analytics.productImpressions).toHaveBeenCalledWith(expectedAnalyticsProducts);
                });
            });

            describe('WITH product with discount price', () => {

                var discountPricing = {
                    DisplayPrice : '$165.00',
                    DisplaySpecialPrice : '$70.00',
                    HasPriceRange : false,
                    IsOnSale : true,
                    ListPrice : 165,
                    Price : 70,
                    PriceListId : 'DEFAULT',
                    JsonContext : '{}'
                };

                var searchResult = CreateSearchResultSample(discountPricing);

                var expectedAnalyticsProducts = CreateExpectedAnalyticsProducts(searchResult, discountPricing.Price);

                it('SHOULD fire the productimpressions trigger', () => {
                    // arrange
                    spyOn(analytics, 'productImpressions');

                    // act -- publish
                    eventHub.publish('searchResultRendered', {data: searchResult});

                    // assert -- does it match
                    expect(analytics.productImpressions).toHaveBeenCalledWith(expectedAnalyticsProducts);
                });
            });

            describe('WITH corrected search terms', () => {
                const keywordEntered = 'shoooes';
                const keywordCorrected = 'shoes';

                const expectedAnalyticsSearchResults : IAnalyticsSearchResults = {
                    keywordEntered : keywordEntered,
                    keywordCorrected : keywordCorrected
                };

                it('SHOULD fire the search term corrected analytics trigger', () => {
                    // arrange
                    spyOn(analytics, 'searchKeywordCorrection');

                    // act -- publish
                    eventHub.publish('searchTermCorrected', {
                        data: {
                            KeywordEntered : keywordEntered,
                            KeywordCorrected : keywordCorrected
                        }}
                    );

                    // assert -- does it match
                    expect(analytics.searchKeywordCorrection).toHaveBeenCalledWith(expectedAnalyticsSearchResults);
                });
            });

            describe('WITH filtering categories', () => {
                const facetKeyEntered = 'CategoryLevel1_Facet';
                const facetValueEntered = 'shoes';
                const pageTypeEntered = 'search';

                const facetKeyExpected = 'category';
                const facetValueExpected = facetValueEntered;
                const pageTypeExpected = pageTypeEntered;

                const parametersExpected = {
                    facetKey: facetKeyExpected,
                    facetValue: facetValueExpected,
                    pageType: pageTypeExpected
                };

                it('SHOULD fire the single facet search analytics trigger', () => {
                    // arrange
                    spyOn(analytics, 'singleFacetChanged');

                    // act -- publish
                    eventHub.publish('singleFacetsChanged', {
                        data: {
                            facetKey: facetKeyEntered,
                            facetValue: facetValueEntered,
                            pageType: pageTypeEntered
                        }}
                    );

                    // assert -- does it match
                    expect(analytics.singleFacetChanged).toHaveBeenCalledWith(parametersExpected);
                });
            });

            describe('WITH filtering any other single facet', () => {
                const facetKeyEntered = 'brand';
                const facetValueEntered = 'Diesel';
                const pageTypeEntered = 'search';

                const facetKeyExpected = facetKeyEntered;
                const facetValueExpected = facetValueEntered;
                const pageTypeExpected = pageTypeEntered;

                const parametersExpected = {
                    facetKey: facetKeyExpected,
                    facetValue: facetValueExpected,
                    pageType: pageTypeExpected
                };

                it('SHOULD fire the single facet search analytics trigger', () => {
                    // arrange
                    spyOn(analytics, 'singleFacetChanged');

                    // act -- publish
                    eventHub.publish('singleFacetsChanged', {
                        data: {
                            facetKey: facetKeyEntered,
                            facetValue: facetValueEntered,
                            pageType: pageTypeEntered
                        }}
                    );

                    // assert -- does it match
                    expect(analytics.singleFacetChanged).toHaveBeenCalledWith(parametersExpected);
                });
            });

            describe('WITH filtering any multi facet having brackets []', () => {
                const facetKeyEntered = 'seasonwear[]';
                const facetValueEntered = 'winter';
                const pageTypeEntered = 'search';

                const facetKeyExpected = 'seasonwear';
                const facetValueExpected = facetValueEntered;
                const pageTypeExpected = pageTypeEntered;

                const parametersExpected = {
                    facetKey: facetKeyExpected,
                    facetValue: facetValueExpected,
                    pageType: pageTypeExpected
                };

                it('SHOULD fire the multi facet search analytics trigger withouth the brackets', () => {
                    // arrange
                    spyOn(analytics, 'multiFacetChanged');

                    // act -- publish
                    eventHub.publish('multiFacetChanged', {
                        data: {
                            facetKey: facetKeyEntered,
                            facetValue: facetValueEntered,
                            pageType: pageTypeEntered
                        }}
                    );

                    // assert -- does it match
                    expect(analytics.multiFacetChanged).toHaveBeenCalledWith(parametersExpected);
                });
            });

            describe('WITH sorting', () => {
                const sortingTypeEntered = 'relevance';
                const pageTypeEntered = 'search';
                const urlEntered = '';

                const facetKeyExpected = sortingTypeEntered;
                const pageTypeExpected = pageTypeEntered;

                it('SHOULD trigger the sorting type', () => {
                    // arrange
                    spyOn(analytics, 'sortingChanged');

                    // act -- publish
                    eventHub.publish('sortingChanged', {
                        data: {
                            sortingType: sortingTypeEntered,
                            pageType: pageTypeEntered,
                            url: urlEntered
                        }}
                    );

                    // assert -- does it match
                    expect(analytics.sortingChanged).toHaveBeenCalledWith(sortingTypeEntered, pageTypeEntered);
                });
            });
        });
    })();
}