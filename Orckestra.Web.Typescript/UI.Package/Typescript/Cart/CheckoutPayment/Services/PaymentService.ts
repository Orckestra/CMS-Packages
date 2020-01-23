///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../Mvc/ComposerClient.ts' />
///<reference path='../../../Events/EventHub.ts' />
///<reference path='../Providers/BaseCheckoutPaymentProvider.ts' />
///<reference path='../Repositories/IPaymentRepository.ts' />
///<reference path='./IPaymentService.ts' />

module Orckestra.Composer {
    'use strict';

    export class PaymentService implements IPaymentService {
        private eventHub: IEventHub;
        private paymentRepository: IPaymentRepository;

        constructor(eventHub : IEventHub, paymentRepository: IPaymentRepository) {
            this.eventHub = eventHub;
            this.paymentRepository = paymentRepository;
        }

        /**
         * Return a list of acceptable payment providers details with labels
         * @param  {providers: Array<string>}      Array of provider names.
         * @return {Array<IPaymentMethodViewModel} List of payment provider details
         */
        public getPaymentMethods(providers: Array<string>) : Q.Promise<IPaymentViewModel> {
            return this.paymentRepository.getPaymentMethods(providers);
        }

        /**
         * Return the active payment for the active cart
         * @return {IActivePaymentViewModel} Active payment for the active cart.
         */
        public getActivePayment() : Q.Promise<IActivePaymentViewModel> {
            return this.paymentRepository.getActivePayment();
        }

        public removePaymentMethod(paymentMethodId: string, paymentProviderName: string): Q.Promise<void> {
            return this.paymentRepository.removePaymentMethod(paymentMethodId, paymentProviderName);
        }

        public setPaymentMethod(request : any) : Q.Promise<IPaymentViewModel> {
            return this.paymentRepository.setPaymentMethod(request);
        }
    }
}
