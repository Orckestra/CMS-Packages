/// <reference path='../../../../Typings/tsd.d.ts' />
/// <reference path='../../../Events/IEventHub.ts' />
/// <reference path='./BaseCheckoutPaymentProvider.ts' />

module Orckestra.Composer {
    'use strict';

    export class CheckoutPaymentProviderFactory {
        private _eventHub: IEventHub;
        private _window: Window;

        public constructor(window: Window, eventHub: IEventHub) {
            this._eventHub = eventHub;
            this._window = window;
        }

        /**
         * Determines if the Checkout Payment Provider exists or not.
         * @param  {string}  providerType Type of the provider.
         * @return {boolean}              True if the provider exists in the Orckestra.Composer namespace.
         */
        public hasProvider(providerType: string) : boolean {
            if (Orckestra.Composer[providerType]) {
                 return true;
            }

            return false;
        }

        /**
         * Gets an instance of a Checkout Payment provider. If the provider does not exists, throws an
         * error.
         * @param  {string}                   providerType                  Type of the provider.
         * @param  {string}                   providerName                  Name of the provider.
         * @return {ICheckoutPaymentProvider}                               Instance of the provider.
         */
        public getInstance(providerType: string, providerName: string)
            : BaseCheckoutPaymentProvider {

            if (this.hasProvider(providerType)) {
                var Clazz = Orckestra.Composer[providerType];
                var instance = new Clazz(this._window, providerName, this._eventHub);
                return instance;
            }

            throw new Error('Unable to find a class named "' + providerType + '".');
        }
    }
}
