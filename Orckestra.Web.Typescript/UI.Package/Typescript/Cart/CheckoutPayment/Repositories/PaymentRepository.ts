///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../Mvc/ComposerClient.ts' />
///<reference path="./IPaymentRepository.ts" />
///<reference path="../ViewModels/IPaymentViewModel.ts" />
///<reference path="../ViewModels/IActivePaymentViewModel.ts" />
///<reference path="../ViewModels/IPaymentProfileListViewModel.ts" />

module Orckestra.Composer {
    export class PaymentRepository implements IPaymentRepository {

        /**
         * Return a list of saved payment methods for a specified array of payment provider names
         * @param  {providers: Array<string>}       Array of provider names.
         * @return {Array<IPaymentMethodViewModel>} Instance of the provider.
         */
        public getPaymentMethods(providers: Array<string>): Q.Promise<IPaymentViewModel> {
            return <Q.Promise<IPaymentViewModel>>
                ComposerClient.post('/api/payment/paymentmethods', { Providers : providers});
        }

        /**
         * Return the active payment for the active cart
         * @return {IActivePaymentViewModel} Active payment for the active cart.
         */
        public getActivePayment() : Q.Promise<IActivePaymentViewModel> {
            return <Q.Promise<IActivePaymentViewModel>>
                ComposerClient.get('/api/payment/activepayment');
        }

        public removePaymentMethod(paymentMethodId: string, paymentProviderName: string): Q.Promise<void> {
            return <Q.Promise<void>>
                ComposerClient.remove('/api/payment/removemethod',
                                        {
                                            PaymentMethodId: paymentMethodId,
                                            PaymentProviderName: paymentProviderName
                                        });
        }

        public setPaymentMethod(request : any) : Q.Promise<IPaymentViewModel> {
            return <Q.Promise<IPaymentViewModel>>
                ComposerClient.put('/api/payment/paymentMethod', request);
        }
    }
}
