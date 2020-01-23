/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../Mvc/ComposerClient.ts' />
/// <reference path='../../Events/EventHub.ts' />
/// <reference path='./AnalyticsPlugin.ts' />
/// <reference path='./IAnalyticsProduct.ts' />

declare var dataLayer: any;
declare var ga: any;

module Orckestra.Composer {
    'use strict';

    export class GoogleAnalyticsPlugin extends AnalyticsPlugin {
        public initialize() {
            super.initialize();

            if (!window['dataLayer']) {
                console.warn('The dataLayer variable does not exists. Have you included the gtm.js script ?');
                window['dataLayer'] = [];
            }

        }

        public userLoggedIn(type: string, source: string) {
            dataLayer.push({
                event: 'accountLogin',
                loginType: type,
                loginSource: source
            });
        }

        public userCreated() {
            dataLayer.push({
                event: 'accountCreated'
            });
        }

        public recoverPassword() {
            dataLayer.push({
                event: 'passRecovery'
            });
        }

        public singleFacetChanged(searchFilters: IAnalyticsSearchFilters) {
            dataLayer.push({
                event: 'filterRefinement',
                filterName: searchFilters.facetKey,
                filterValue: searchFilters.facetValue,
                sectionName: searchFilters.pageType
            });
        }

        public multiFacetChanged(searchFilters: IAnalyticsSearchFilters) {
            dataLayer.push({
                event: 'filterRefinement',
                filterName: searchFilters.facetKey,
                filterValue: searchFilters.facetValue,
                sectionName: searchFilters.pageType
            });
        }

        public sortingChanged(sortingType: string, pageType: string) {
            dataLayer.push({
                event: 'sortingOption',
                sortingType: sortingType,
                sectionName: pageType
            });
        }

        public productImpressions(products: IAnalyticsProduct[]) {
            dataLayer.push({
                event: 'productImpressions',
                ecommerce: {
                    impressions: products
                }
            });
        }

        public productClick(products: IAnalyticsProduct[], listName: string) {
            dataLayer.push({
                event: 'productClick',
                ecommerce: {
                    click: {
                        actionField: {
                            list: listName
                        },
                        products: products
                    }
                }
            });
        }

        public productDetailImpressions(products: IAnalyticsProduct[], listName: string) {
            dataLayer.push({
                event: 'productDetailImpressions',
                ecommerce: {
                    detail: {
                        actionField: {
                            list: listName
                        },    // 'detail' actions have an optional list property.
                        products: products
                    }
                }
            });
        }

        public addToCart(product: IAnalyticsProduct, listName: string) {
            dataLayer.push({
                event: 'addToCart',
                ecommerce: {
                    add: {
                        actionField: { list: listName },
                        products: [product]
                    }
                }
            });
        }

        public addToWishList(product: IAnalyticsProduct) {
            dataLayer.push({
                'event': 'addToWishList',
                'productName': product.name,
                'productPrice': product.price
            });
        }

        public couponsUsed(coupon: IAnalyticsCoupon) {
            dataLayer.push({
                'event': 'checkoutComplete',
                'couponCode': coupon.code,
                'discountAmount': coupon.discountAmount
            });
        }

        public shareWishList(data: any) {
            dataLayer.push({
                'event': 'shareMyWishList'
            });
        }

        public removeFromCart(product: IAnalyticsProduct, listName: string) {
            dataLayer.push({
                event: 'removeFromCart',
                ecommerce: {
                    remove: {
                        actionField: { list: listName },
                        products: [product]
                    }
                }
            });
        }

        public checkout(step: number, transaction: IAnalyticsTransaction, products: IAnalyticsProduct[]) {
            dataLayer.push({
                event: 'checkout',
                transaction: transaction,
                ecommerce: {
                    checkout: {
                        actionField: {
                            step: this.getStepNumber(step)
                        },
                        products: products
                    }
                }
            });
        }

        public checkoutOption(step: number) {
            dataLayer.push({
                event: 'checkoutOption',
                ecommerce: {
                    checkout_option: {
                        actionField: {
                            step: this.getStepNumber(step)
                        }
                    }
                }
            });
        }

        public purchase(order: IAnalyticsOrder, transaction: IAnalyticsTransaction, products: IAnalyticsProduct[]) {
            dataLayer.push({
                event: 'purchase',
                transaction: transaction,
                ecommerce: {
                    purchase: {
                        actionField: order,
                        products: products
                    }
                }
            });
        }

        public noResultsFound(keywordNotFound: string) {
            dataLayer.push({
                'event': 'noResults',
                'keywordEntered': keywordNotFound
            });
        }

        public searchKeywordCorrection(searchResults: IAnalyticsSearchResults) {
            dataLayer.push({
                'event': 'keywordCorrection',
                'keywordCorrected': searchResults.keywordCorrected,
                'keywordEntered': searchResults.keywordEntered
            });
        }

        public sendEvent(eventName: string, category: string, action: string, label?: string, value?: number) {
            dataLayer.push({
                event: eventName,
                gaEventCategory: category,
                gaEventAction: action,
                gaEventLabel: label,
                gaEventValue: value
            });
        }

        private getStepNumber(step) {
            return step + 1;
        }
    }
}