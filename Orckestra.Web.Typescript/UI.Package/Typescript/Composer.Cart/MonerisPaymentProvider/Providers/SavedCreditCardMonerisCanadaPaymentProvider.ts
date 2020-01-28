///<reference path='./BaseSpecializedMonerisCanadaPaymentProvider.ts' />
///<reference path='../MonerisPaymentService.ts' />
///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../ErrorHandling/ErrorHandler.ts' />
///<reference path='../../../UI/UIModal.ts' />
///<reference path='../../CheckoutPayment/ViewModels/IActivePaymentViewModel.ts' />


module Orckestra.Composer {
    'use strict';

    export class SavedCreditCardMonerisCanadaPaymentProvider extends BaseSpecializedMonerisCanadaPaymentProvider {

        private _deleteModalElementSelector: string = '#confirmationModal';
        private _busyHandler: UIBusyHandle;
        private _uiModal: UIModal;

        constructor(window: Window, paymentService: MonerisPaymentService, eventHub: IEventHub) {
            super(window, paymentService, eventHub);

            this._uiModal = new UIModal(window, this._deleteModalElementSelector, this.deleteCart, this);
        }

        /**
         * Register event handlers for dom events
         */
        public registerDomEvents(): void {
            $(this._window.document).on('click', '.moneris--deletecard', this._uiModal.openModal);
        }

        /**
         * Unregister event handlers for dom events
         */
        public unregisterDomEvents(): void {
            $(this._window.document).off('click', '.moneris--deletecard', this._uiModal.openModal);
        }

        /**
         * Method called to get a promise for payment validation.
         * Returns a promise of boolean. The return boolean needs to be false for validation error,
         * or true if valid.
         * @param   {IActivePaymentViewModel}   The current active payment view model
         * @return  {Q.Promise<boolean>}        Promise that will be executed when we validate the payment control.
         */
        public validatePayment(activePaymentVM: IActivePaymentViewModel): Q.Promise<boolean> {
            // Considering the credit card was already added we do not need to run additional validations
            return Q(true);
        }

        /**
         * Add the temporary token to the vault profile of the user
         * @param   {IActivePaymentViewModel}   The current active payment view model
         * @return  {Q.Promise<any>}            The object is the updated properties of the cart used in CheckoutService.updateCart()
         */
        public addVaultProfileToken(activePaymentVM : IActivePaymentViewModel): Q.Promise<any> {
            // no need to add the payment method to the vault
            return Q({});
        }

        protected deleteCart(event: JQueryEventObject) : Q.Promise<void> {
            let element = $(event.target);
            let paymentMethodId: string = element.data('payment-id');
            let paymentProviderName: string = element.data('payment-provider');

            //TODO : To replace with async busy from controller when change inheritance with controller.
            this._busyHandler = new UIBusyHandle($(document), $(document), 0);


            // TODO: publish valid event
            return this._paymentService
                       .removePaymentMethod(paymentMethodId, paymentProviderName)
                       .then(() => this._eventHub.publish('paymentMethodsUpdated', null))
                       .fail(reason => ErrorHandler.instance().outputError(reason))
                       .fin(() => {
                           this.releaseBusyHandler();
                       });
        }

        protected releaseBusyHandler(): void {
            if (this._busyHandler) {
                this._busyHandler.done();
                this._busyHandler = null;
            }
        }
    }
}
