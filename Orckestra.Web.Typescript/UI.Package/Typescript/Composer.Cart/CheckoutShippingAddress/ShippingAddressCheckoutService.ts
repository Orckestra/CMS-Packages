///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Events/IEventHub.ts' />
///<reference path='../CartSummary/CartService.ts' />

module Orckestra.Composer {
    'use strict';

    export class ShippingAddressCheckoutService {

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

        public setCheapestShippingMethodUsing(postalCode: string): Q.Promise<void> {

            this.eventHub.publish('cartUpdating', null);

            return this.cartService.updateShippingMethodPostalCode(postalCode)
                       .then(() => this.cartService.setCheapestShippingMethod())
                       .then(() => this.cartService.getCart())
                       .then(cart => {
                           this.eventHub.publish('cartUpdated', { data: cart });
                           this.eventHub.publish('postalCodeChanged', { data: cart });
                       })
                       .fail(reason => this.handleError(reason));
        }

        private handleError(reason : any) {

            console.error('The user changed the postal code, ' +
                'but we were unable to update the cart dynamically and estimate shipping fees', reason);

            this.eventHub.publish('cartUpdatingFailed', null);
        }
    }
}
