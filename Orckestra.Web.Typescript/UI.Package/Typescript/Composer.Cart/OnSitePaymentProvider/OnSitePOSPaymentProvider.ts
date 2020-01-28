///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Events/IEventHub.ts' />
///<reference path='../CheckoutPayment/ViewModels/IActivePaymentViewModel.ts' />
///<reference path='../CheckoutPayment/Providers/BaseCheckoutPaymentProvider.ts' />

module Orckestra.Composer {
    'use strict';

    export class OnSitePOSPaymentProvider extends BaseCheckoutPaymentProvider {

        constructor(window: Window, providerName: string, eventHub: IEventHub) {
            super(window, eventHub, 'OnSitePOSPaymentProvider', providerName);
        }

        /**
         * Method called to get a promise for payment validation.
         * Returns a promise of boolean. The return boolean needs to be false for validation error,
         * or true if valid.
         * @return {Q.Promise<boolean>} Promise that will be executed when we validate the payment control.
         */
        public validatePayment(activeVM: IActivePaymentViewModel): Q.Promise<boolean> {
            return Q(true);
        }

        /**
         * Method called to get a promise when payment will submit.
         * @return {Q.Promise<any>} Promise that will be executed when to cart is about the be updated.
         */
        public submitPayment(activeVM: IActivePaymentViewModel): Q.Promise<any> {
            return Q({});
        }
    }
}
