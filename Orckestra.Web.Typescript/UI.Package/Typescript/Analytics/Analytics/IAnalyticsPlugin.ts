///<reference path='./IAnalyticsProduct.ts' />
///<reference path='./IAnalyticsSearchResults.ts' />

module Orckestra.Composer {
    'use strict';
    export interface IAnalyticsPlugin {
        productImpressions(products: IAnalyticsProduct[]): void;
        productClick(product: IAnalyticsProduct, listName: string): void;
        productDetailImpressions(products: IAnalyticsProduct[], listName: string): void;
        addToCart(product: IAnalyticsProduct, listName: string): void;
        removeFromCart(product: IAnalyticsProduct, listName: string): void;
        checkout(step: number, transaction: IAnalyticsTransaction, products: IAnalyticsProduct[]): void;
        checkoutOption(step: number): void;
        searchKeywordCorrection(searchResults: IAnalyticsSearchResults): void;
        noResultsFound(keywordNotFound: string): void;
        couponsUsed(order: IAnalyticsCoupon): void;
        purchase(order: IAnalyticsOrder, transaction: IAnalyticsTransaction, products: IAnalyticsProduct[]): void;
        sendEvent(eventName: string, category: string, action: string, label?: string, value?: number): void;
        userLoggedIn(type: string, source: string): void;
        userCreated(): void;
        recoverPassword(): void;
    }
}