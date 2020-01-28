///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../ViewModels/IPaymentMethodViewModel.ts' />

module Orckestra.Composer {
    export interface IPaymentRepository {

        /**
         * Return a list of saved payment methods for a specified array of payment provider names
         * @param  {providers: Array<string>}       Array of provider names.
         * @return {Array<IPaymentMethodViewModel>} Instance of the provider.
         */
        getPaymentMethods(providers: Array<string>) : Q.Promise<IPaymentViewModel>;

        /**
         * Return the active payment for the active cart
         * @return {IActivePaymentViewModel} Active payment for the active cart.
         */
        getActivePayment() : Q.Promise<IActivePaymentViewModel>;

        removePaymentMethod(paymentMethodId: string, paymentProviderName: string): Q.Promise<void>;

        setPaymentMethod(request : any) : Q.Promise<IPaymentViewModel>;
    }
}
