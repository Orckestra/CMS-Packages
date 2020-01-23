/// <reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    /**
     * Separates the logic that retrieves the data and maps it to the entity model from the application services that acts on the model.
    */
    export interface ICartRepository {

        /**
         * Get the cart of the current customer.
         */
        getCart(): Q.Promise<any>;

        /**
         * Add a line item to the cart of the current customer.
         */
        addLineItem(productId: string, variantId: string, quantity: number,
            recurringOrderFrequencyName?: string,
            recurringOrderProgramName?: string): Q.Promise<any>;

        /**
         * Update the quantity of a line item in the cart of the current customer.
         */
        updateLineItem(lineItemId: string, quantity: number,
            recurringOrderFrequencyName?: string,
            recurringOrderProgramName?: string): Q.Promise<any>;

        /**
         * Delete a line item from the cart of the current customer.
         */
        deleteLineItem(lineItemId: string): Q.Promise<any>;

        /**
         * Update the postal code of the billing method in cart of the current customer.
         */
        updateBillingMethodPostalCode(postalCode: string): Q.Promise<any>;

        /**
         * Update the postal code of the shipping method in cart of the current customer.
         */
        updateShippingMethodPostalCode(postalCode: string): Q.Promise<any>;

        /**
         * Set the cheapest shipping method in cart of the current customer.
         */
        setCheapestShippingMethod(): Q.Promise<any>;

        /**
         * Add a coupon to the cart of the current customer.
         */
        addCoupon(couponCode: string): Q.Promise<any>;

        /**
         * Remove a coupon from the cart of the current customer.
         */
        removeCoupon(couponCode: string): Q.Promise<any>;

        /**
         * Cleans the cart of invalid line items.
         */
        clean(): Q.Promise<any>;

        /**
         * Update the cart of the current customer.
         */
        updateCart(param: any): Q.Promise<IUpdateCartResult>;

        /**
         * Complete the checkout, thereby clearing every item in the cart of the current customer.
         */
        completeCheckout(currentStep: number): Q.Promise<ICompleteCheckoutResult>;
    }

    export interface IUpdateCartResult {

        HasErrors: boolean;

        NextStepUrl: string;

        Cart: any;
    }

    export interface ICompleteCheckoutResult {

        OrderNumber: string;

        CustomerEmail: string;

        NextStepUrl: string;
    }
}
