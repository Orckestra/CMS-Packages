/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../Mvc/ComposerClient.ts' />
/// <reference path='./ICartRepository.ts' />

module Orckestra.Composer {
    'use strict';

    export class CartRepository implements ICartRepository {

        public getCart(): Q.Promise<any> {

            return ComposerClient.get('/api/cart/getcart');
        }

        public addLineItem(productId: string, variantId: string, quantity: number,
            recurringOrderFrequencyName?: string,
            recurringOrderProgramName?: string): Q.Promise<any> {

            if (!productId) {
                throw new Error('The product id is required');
            }

            if (quantity <= 0) {
                throw new Error('The quantity must be greater than zero');
            }

            var data = {
                ProductId: productId,
                VariantId: variantId,
                Quantity: quantity,
                RecurringOrderFrequencyName: recurringOrderFrequencyName,
                RecurringOrderProgramName: recurringOrderProgramName
            };

            return ComposerClient.post('/api/cart/lineitem', data);
        }

        public updateLineItem(lineItemId: string, quantity: number,
            recurringOrderFrequencyName?: string,
            recurringOrderProgramName?: string): Q.Promise<any> {

            if (!lineItemId) {
                throw new Error('The line item id is required');
            }

            if (quantity <= 0) {
                throw new Error('The quantity must be greater than zero');
            }

            var data = {
                LineItemId: lineItemId,
                Quantity: quantity,
                RecurringOrderFrequencyName: recurringOrderFrequencyName,
                RecurringOrderProgramName: recurringOrderProgramName
            };

            return ComposerClient.put('/api/cart/lineitem', data);
        }

        public deleteLineItem(lineItemId: string): Q.Promise<any> {

            if (!lineItemId) {
                throw new Error('The line item id is required');
            }

            var data = {
                LineItemId: lineItemId
            };

            return ComposerClient.remove('/api/cart/lineitem', data);
        }

        public updateBillingMethodPostalCode(postalCode: string): Q.Promise<any> {

            if (!postalCode) {
                throw new Error('The postal code is required');
            }

            var data = {
                PostalCode: postalCode
            };

            return ComposerClient.post('/api/cart/billingaddress', data);
        }

        public updateShippingMethodPostalCode(postalCode: string): Q.Promise<any> {

            if (!postalCode) {
                throw new Error('The postal code is required');
            }

            var data = {
                PostalCode: postalCode
            };

            return ComposerClient.post('/api/cart/shippingaddress', data);
        }

        public setCheapestShippingMethod(): Q.Promise<any> {

            var data = {
                UseCheapest: true
            };

            return ComposerClient.post('/api/cart/shippingmethod', data);
        }

        public addCoupon(couponCode: string): Q.Promise<any> {

            if (!couponCode) {
                throw new Error('The coupon code is required');
            }

            var data = {
                CouponCode: couponCode
            };

            return ComposerClient.post('/api/cart/coupon', data);
        }

        public removeCoupon(couponCode: string): Q.Promise<any> {

            if (!couponCode) {
                throw new Error('The coupon code is required');
            }

            var data = {
                CouponCode: couponCode
            };

            return ComposerClient.remove('/api/cart/coupon', data);
        }

        public clean(): Q.Promise<any> {

            var data = { };

            return ComposerClient.remove('/api/cart/clean', data);
        }

        public updateCart(param: any): Q.Promise<IUpdateCartResult> {

            if (!param) {
                throw new Error('The param is required');
            }

            return ComposerClient.post('/api/cart/updateCart', param);
        }

        public completeCheckout(currentStep: number): Q.Promise<ICompleteCheckoutResult> {

            var data = {
                CurrentStep: currentStep
            };

            return ComposerClient.post('/api/cart/completecheckout', data);
        }
    }
}
