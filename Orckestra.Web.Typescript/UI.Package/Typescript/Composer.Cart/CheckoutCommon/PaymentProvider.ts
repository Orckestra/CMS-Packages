///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/ComposerClient.ts' />
///<reference path='../../Events/IEventHub.ts' />
///<reference path='./IUpdatePaymentOptions.ts' />
///<reference path='./IGetPaymentMethodsOptions.ts' />

module Orckestra.Composer {
    'use strict';

    export class PaymentProvider {
        protected window: Window;
        protected eventHub: IEventHub;

        protected _currentPaymentMethod: IUpdatePaymentOptions;

        public getCurrentPaymentMethod(): IUpdatePaymentOptions {
            return this._currentPaymentMethod;
        }

        constructor(window: Window, eventHub: IEventHub) {
            this.window = window;
            this.eventHub = eventHub;
        }

        /**
        * Return a Promise which returns an array of Payment Methods.
        */
        public getPaymentMethods(getPaymentMethodOptions: IGetPaymentMethodsOptions): Q.Promise<any> {
            return ComposerClient.post('/api/payment/paymentmethods', getPaymentMethodOptions);
        }

        public updatePaymentMethod(request: IUpdatePaymentOptions): Q.Promise<any> {
            return ComposerClient.put('/api/payment/paymentmethod', request)
                .then((payload) => {
                    this._currentPaymentMethod = request;

                    return payload;
                });
        }
    }
}
