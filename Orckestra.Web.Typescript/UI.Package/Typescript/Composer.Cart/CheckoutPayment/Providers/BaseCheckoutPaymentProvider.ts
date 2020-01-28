///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../System/IDisposable.ts' />
///<reference path='../ViewModels/IActivePaymentViewModel.ts' />

module Orckestra.Composer {
    'use strict';

    export class BaseCheckoutPaymentProvider implements Orckestra.Composer.IDisposable {
        protected _providerType: string;
        protected _providerName: string;
        protected _eventHub: IEventHub;

        /**
         * This property is injected for unit test purposes
         */
        private _window: Window;

        constructor(window: Window, eventHub: IEventHub, providerType: string, providerName: string) {
            this._providerType = providerType;
            this._providerName = providerName;
            this._window = window;
            this._eventHub = eventHub;
        }

        /**
         * Obtains the underlying type of the Payment Provider.
         * @return {string} Type of the Payment Provider.
         */
        public get providerType(): string {
            return this._providerType;
        }

        /**
         * Obtains the name of the Payment Provider.
         * @return {string} Name of the Payment provider.
         */
        public get providerName(): string {
            return this._providerName;
        }

        protected get window(): Window {
            return this._window;
        }

        /**
         * Method called to get a promise for payment validation.
         * Returns a promise of boolean. The return boolean needs to be false for validation error,
         * or true if valid.
         * @return {Q.Promise<boolean>} Promise that will be executed when we validate the payment control.
         */
        public validatePayment(activePaymentVM: IActivePaymentViewModel): Q.Promise<boolean> {
            throw new Error('This Payment Provider does not implement the "validatePayment" method.');
        }

        /**
         * Method called to get a promise when payment will submit.
         * @return {Q.Promise<any>} Promise that will be executed when to cart is about the be updated.
         */
        public submitPayment(activePaymentVM: IActivePaymentViewModel): Q.Promise<any> {
            throw new Error('This Payment Provider does not implement the "submitPayment" method.');
        }

        /**
         * Gets the container for the Payment Provider.
         * @return {JQuery} jQuery object.
         */
        protected getForm(): JQuery {
            var form = $('#PaymentForm');

            if (!form || _.isEmpty(form)) {
                throw new Error('Could not find the element PaymentForm on this page.');
            }

            return form;
        }

        public dispose(): void {
            //Nothing to do here.
        }
    }
}
