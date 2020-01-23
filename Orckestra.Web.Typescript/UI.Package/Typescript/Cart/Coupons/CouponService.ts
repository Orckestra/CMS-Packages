///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Events/EventHub.ts' />
///<reference path='../CartSummary/CartService.ts' />

module Orckestra.Composer {
    'use strict';

    export class CouponService {

        private cartService: ICartService;
        private eventHub: IEventHub;

        constructor(cartService: ICartService, eventHub: IEventHub) {

            if (!cartService) {
                throw new Error('Error: cartService is required');
            }
            if (!eventHub) {
                throw new Error('Error: eventHub is required');
            }

            this.cartService = cartService;
            this.eventHub = eventHub;
        }

        /**
        * Adds a coupon using the Composer API.
        * @param couponCode Code of the Coupon to add through the API.
        */
        public addCoupon(couponCode: string): Q.Promise<void> {

            var data = {
                CouponCode: couponCode
            };

            this.eventHub.publish('couponUpdating', { data: data });

            return this.cartService.addCoupon(couponCode)
                       .then(() => this.cartService.getCart())
                       .then(cart => {

                           this.eventHub.publish('cartUpdated', { data: cart });
                           this.publishCouponUpdatedEvent(cart, true);

                        }, reason => {
                          console.error('Error while adding coupon', reason);
                          this.publishCouponUpdatedEvent(undefined, false);
                        });
        }

        /**
         * Removes a coupon using the Composer API
         * @param {String} couponCode Code of the coupon to remove.
         */
        public removeCoupon(couponCode: string): Q.Promise<void> {

            var data = {
                CouponCode: couponCode
            };

            return this.cartService.removeCoupon(couponCode)
                       .then(() => this.cartService.getCart())
                       .then(cart => {

                           this.eventHub.publish('cartUpdated', { data: cart });
                           this.publishCouponUpdatedEvent(cart, true);

                       }, reason => this.publishCouponUpdatedEvent(undefined, false));
        }

        private publishCouponUpdatedEvent(result: any, isSuccess: boolean) {
            this.eventHub.publish('couponUpdated', { data: result });
        }
    }
}
