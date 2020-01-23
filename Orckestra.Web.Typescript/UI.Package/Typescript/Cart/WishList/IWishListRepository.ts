/// <reference path='../../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    /**
     * Separates the logic that retrieves the data and maps it to the entity model from the application services that acts on the model.
    */
    export interface IWishListRepository {
        /**
         * Get the lightweight wishList of the current customer.
         */
        getWishListSummary(): Q.Promise<any>;
        /**
         * Get the wishList of the current customer.
         */
        getWishList(): Q.Promise<any>;
        /**
         * Add a line item to the wishList of the current customer.
         */
        addLineItem(productId: string, variantId: string, quantity: number,
            recurringOrderFrequencyName?: string,
            recurringOrderProgramName?: string): Q.Promise<void>;
        /**
         * Delete a line item from the wishList of the current customer.
         */
        deleteLineItem(lineItemId: string): Q.Promise<void>;

    }
}
