///<reference path='../MonerisPaymentService.ts' />
///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../CheckoutPayment/ViewModels/IActivePaymentViewModel.ts' />

module Orckestra.Composer {
    'use strict';

    export type BaseMonerisCanadaPaymentProviderCollection = { [type: string] : BaseSpecializedMonerisCanadaPaymentProvider };

    export class BaseSpecializedMonerisCanadaPaymentProvider {

        /**
         * Injected property for specs
         */
        protected _window: Window;

        protected _paymentService: MonerisPaymentService;
        protected _eventHub: IEventHub;

        constructor(window: Window, paymentService: MonerisPaymentService, eventHub: IEventHub) {
            this._window = window;
            this._paymentService = paymentService;
            this._eventHub = eventHub;
        }


        /**
         * Register event handlers for dom events
         */
        public registerDomEvents(): void {
            // do nothing
        }

        /**
         * Unregister event handlers for dom events
         */
        public unregisterDomEvents(): void {
            // do nothing
        }

        /**
         * Method called to get a promise for payment validation.
         * Returns a promise of boolean. The return boolean needs to be false for validation error,
         * or true if valid.
         * @param   {IActivePaymentViewModel}   The current active payment view model
         * @return  {Q.Promise<boolean>}        Promise that will be executed when we validate the payment control.
         */
        public validatePayment(activePaymentVM: IActivePaymentViewModel): Q.Promise<boolean> {
            return Q(true);
        }

        /**
         * Add the temporary token to the vault profile of the user
         * @param   {IActivePaymentViewModel}   The current active payment view model
         * @return  {Q.Promise<any>}            The object is the updated properties of the cart used in CheckoutService.updateCart()
         */
        public addVaultProfileToken(activePaymentVM : IActivePaymentViewModel): Q.Promise<any> {
            return Q({});
        }
    }
}
