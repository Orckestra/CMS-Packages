///<reference path='./ICreateVaultTokenOptions.ts' />
///<reference path='./ISetDefaultCustomerPaymentMethodViewModel.ts' />
///<reference path='./IMonerisAddVaultProfileViewModel.ts' />
///<reference path='../CheckoutPayment/ViewModels/IPaymentMethodViewModel.ts' />
///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/ComposerClient.ts' />

module Orckestra.Composer {
    export class MonerisPaymentService {

        /**
         * Adds a token from Moneris to the current payment.
         * @param  {ICreateVaultTokenOptions} request Request to add the Vault Token.
         * @return {Q.Promise<any>}                   Promise of the AJAX request.
         */
        public addCreditCard(request: ICreateVaultTokenOptions): Q.Promise<IMonerisAddVaultProfileViewModel> {
            return ComposerClient.post('/api/vaultprofile/addprofile', request);
        }

        public setDefaultCustomerPaymentMethod(request: ISetDefaultCustomerPaymentMethodViewModel): Q.Promise<IPaymentMethodViewModel> {
            return ComposerClient.put('/api/cart/setdefaultpaymentmethod', request);
        }

        public removePaymentMethod(paymentMethodId: string, paymentProviderName: string): Q.Promise<void> {
            return <Q.Promise<void>>
                ComposerClient.remove('/api/payment/removemethod',
                                        {
                                            PaymentMethodId: paymentMethodId,
                                            PaymentProviderName: paymentProviderName
                                        });
        }

        public removeRecurringCartPaymentMethod(paymentMethodId: string, paymentProviderName: string, cartName: string): Q.Promise<void> {
            return <Q.Promise<void>>
                ComposerClient.remove('/api/payment/recurringcartremovemethod',
                                        {
                                            PaymentMethodId: paymentMethodId,
                                            PaymentProviderName: paymentProviderName,
                                            CartName: cartName
                                        });
        }
    }
}
