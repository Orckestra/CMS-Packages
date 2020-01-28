///<reference path='./ICreateVaultTokenOptions.ts' />
///<reference path='./IMonerisResponseData.ts' />
///<reference path='./MonerisPaymentService.ts' />
///<reference path='./Providers/BaseSpecializedMonerisCanadaPaymentProvider.ts' />
///<reference path='./Providers/CreditCardMonerisCanadaPaymentProvider.ts' />
///<reference path='./Providers/SavedCreditCardMonerisCanadaPaymentProvider.ts' />
///<reference path='../CheckoutPayment/Providers/BaseCheckoutPaymentProvider.ts' />
///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Events/IEventHub.ts' />
///<reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />

module Orckestra.Composer {
    'use strict';

    export class MonerisCanadaPaymentProvider extends BaseCheckoutPaymentProvider {
        private _validationDefer: Q.Deferred<any>;
        private _monerisPaymentService: MonerisPaymentService;
        private _providers: BaseMonerisCanadaPaymentProviderCollection;

        constructor(window: Window, providerName: string, eventHub: IEventHub) {
            super(window, eventHub, 'MonerisCanadaPaymentProvider', providerName);

            this._monerisPaymentService = new MonerisPaymentService();

            this.registerSpecializedProviders();
            this.registerDomEvents();
        }

        get providers(): BaseMonerisCanadaPaymentProviderCollection {
            return this._providers;
        }

        /**
         * Method called to get a promise for payment validation.
         * Returns a promise of boolean. The return boolean needs to be false for validation error,
         * or true if valid.
         * @return {Q.Promise<boolean>} Promise that will be executed when we validate the payment control.
         */
        public validatePayment(activePaymentVM: IActivePaymentViewModel): Q.Promise<boolean> {
            return this.getProvider(activePaymentVM.PaymentMethodType).validatePayment(activePaymentVM);
        }

        public submitPayment(activePaymentVM : IActivePaymentViewModel): Q.Promise<any> {
            return this.getProvider(activePaymentVM.PaymentMethodType)
                       .addVaultProfileToken(activePaymentVM);
        }

        public dispose(): void {
            this.unregisterDomEvents();
        }

        protected setDefaultCustomerPaymentMethod(activePaymentVM: IActivePaymentViewModel): Q.Promise<IPaymentMethodViewModel> {
            return this._monerisPaymentService
                       .setDefaultCustomerPaymentMethod({
                           PaymentMethodId: activePaymentVM.Id,
                           PaymentProviderName: activePaymentVM.ProviderName
                       });
        }

        protected registerSpecializedProviders(): void {
            this._providers = {
                'SavedCreditCard': new SavedCreditCardMonerisCanadaPaymentProvider(
                        this.window,
                        this._monerisPaymentService,
                        this._eventHub),

                'CreditCard': new CreditCardMonerisCanadaPaymentProvider(
                        this.window,
                        this._monerisPaymentService,
                        this._eventHub)
            };
        }

        protected registerDomEvents(): void {
            _.forEach(this.providers, p => p.registerDomEvents());
        }

        protected unregisterDomEvents(): void {
            _.forEach(this.providers, p => p.unregisterDomEvents());
        }


        protected getProvider(providerName: string): BaseSpecializedMonerisCanadaPaymentProvider {
            if (!this.providers[providerName]) {
                throw new Error('Provider not found');
            }

            return this.providers[providerName];
        }
    }
}
