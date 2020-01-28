///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Composer.MyAccount/Common/CustomerService.ts' />
///<reference path='../../Composer.MyAccount/Common/MyAccountEvents.ts' />
///<reference path='../../Composer.MyAccount/Common/MyAccountStatus.ts' />
///<reference path='../CheckoutCommon/BaseCheckoutController.ts' />
///<reference path='./BillingAddressRegisteredCheckoutService.ts' />
///<reference path='../../UI/UIModal.ts' />

module Orckestra.Composer {
    'use strict';

    export class BillingAddressRegisteredCheckoutController extends Orckestra.Composer.BaseCheckoutController {

        protected debounceChangeBillingMethod: Function = _.debounce(this.changeBillingAddressImpl, 500, { 'leading': true });
        protected modalElementSelector: string = '#confirmationModalBilling';
        private uiModal: UIModal;

        protected customerService: ICustomerService = new CustomerService(new CustomerRepository());
        protected billingAddressRegisteredCheckoutService: BillingAddressRegisteredCheckoutService =
            new BillingAddressRegisteredCheckoutService(this.customerService);

        public initialize() {
            super.initialize();

            this.viewModelName = 'BillingAddressRegistered';

            this.uiModal = new UIModal(window, this.modalElementSelector, this.deleteAddress, this, $(this.context.container));
        }

        protected registerSubscriptions() {

            this.eventHub.subscribe(`${this.viewModelName}Rendered`, e => this.onRendered(e));
            this.eventHub.subscribe(MyAccountEvents[MyAccountEvents.AddressDeleted], e => this.onAddressDeleted(e));
        }

        public renderData(checkoutContext: ICheckoutContext): Q.Promise<void> {

            if (checkoutContext.authenticationViewModel.IsAuthenticated) {
                return this.renderAuthenticated(checkoutContext);
            } else {
                this.renderUnauthenticated(checkoutContext);
            }
        }

        protected renderUnauthenticated(checkoutContext: ICheckoutContext) {

            this.unregisterController();
            this.render(this.viewModelName, checkoutContext.authenticationViewModel);
        }

        protected renderAuthenticated(checkoutContext: ICheckoutContext) : Q.Promise<any> {

        this.registerSubscriptions();
        this.render(this.viewModelName, {IsAuthenticated: true});

        return this.billingAddressRegisteredCheckoutService.getBillingAddresses(checkoutContext.cartViewModel)
            .then((billingAdressesVm) => {
                this.render(this.viewModelName, checkoutContext.cartViewModel);
                this.render('BillingRegisteredAddresses', billingAdressesVm);
            })
            .fail(reason => this.handleError(reason))
            .fin(() => this.eventHub.publish(`${this.viewModelName}Rendered`, checkoutContext.cartViewModel));
        }

        protected onRendered(e: IEventInformation) {

            this.formInstances = this.registerFormsForValidation($('#RegisteredBillingAddress', this.context.container));

            var selectedBillingAddressId: string = $(this.context.container).find('input[name=BillingAddressId]:checked').val();

            if (!selectedBillingAddressId) {
                return;
            }

            this.checkoutService.getCart()
                .then(cart => {
                    if (selectedBillingAddressId !== cart.Payment.BillingAddress.AddressBookId) {
                        this.debounceChangeBillingMethod();
                    }
                })
                .fail(reason => this.handleError(reason));
        }

        protected setSelectedBillingAddress() {

            var selectedBillingAddressId: string = $(this.context.container).find('input[name=BillingAddressId]:checked').val();

            if (!selectedBillingAddressId) {
                return;
            }

            this.checkoutService.getCart()
                .done(cart => {
                    if (selectedBillingAddressId !== cart.Payment.BillingAddress.AddressBookId) {
                        this.debounceChangeBillingMethod();
                    }
                });
        }

        public changeBillingAddress(actionContext: IControllerActionContext) {

            this.debounceChangeBillingMethod();
        }

        protected changeBillingAddressImpl() {

            this.checkoutService.updateCart()
                .done(result => {

                    if (result.HasErrors) {
                        throw new Error('The updated cart contains errors');
                    }

                }, reason => this.handleError(reason));
        }

        /**
         * Requires the element in action context to have a data-address-id.
         */
        protected deleteAddress(event: JQueryEventObject): Q.Promise<void> {

            let element = $(event.target);
            var $addressListItem = element.closest('[data-address-id]');
            var addressId = $addressListItem.data('address-id');

            var busy = this.asyncBusy({elementContext: element, containerContext: $addressListItem });

            return this.customerService.deleteAddress(addressId, '')
                .then(result => {
                    this.eventHub.publish(MyAccountEvents[MyAccountEvents.AddressDeleted], { data: addressId });
                })
                .fail(() => this.renderFailedForm(MyAccountStatus[MyAccountStatus.AjaxFailed]))
                .fin(() => busy.done());
        }

         public deleteAddressConfirm(actionContext: IControllerActionContext) {
             this.uiModal.openModal(actionContext.event);
        }

        private onAddressDeleted(e: IEventInformation) {
            var addressId = e.data;
            var $addressListItem = $(this.context.container).find('[data-address-id=' + addressId + ']');

            $addressListItem.remove();
        }

        private useShippingAddress() : Boolean {
            var useShippingAddress = $(this.context.container).find('input[name=UseShippingAddress]:checked').val() === 'true';
            return useShippingAddress;
        }

        private getVisibleForms(): JQuery {
            var visibleForms = $('form', this.context.container).not('form:has(.hide)');
            return visibleForms;
        }

        public changeUseShippingAddress() {

            this.setBillingAddressFormVisibility();
            this.setBillingAddressFormValidation();
            this.setSelectedBillingAddress();
        }

        private setBillingAddressFormVisibility() {

            var useShippingAddress: Boolean = this.useShippingAddress();
            if (useShippingAddress) {
                $('#BillingAddressContent').addClass('hide');
            } else {
                $('#BillingAddressContent').removeClass('hide');
            }
        }

        private setBillingAddressFormValidation() {

            var useShippingAddress: Boolean = this.useShippingAddress();
            var isValidationEnabled: Boolean = this.isBillingAddressFormValidationEnabled();

            if (useShippingAddress) {
                if (isValidationEnabled) {
                    this.disableBillingAddressFormValidation();
                }
            } else {
                if (!isValidationEnabled) {
                    this.enableBillingAddressFormValidation();
                }
            }
        }

        private isBillingAddressFormValidationEnabled() : Boolean {

            return _.some(this.formInstances, (formInstance : any) => {
                return this.isBillingAddressFormInstance(formInstance);
            });
        }

        private disableBillingAddressFormValidation() {

            var formInstance = _.find(this.formInstances, (formInstance : any) => {
                return this.isBillingAddressFormInstance(formInstance);
            });

            formInstance.destroy();

            _.remove(this.formInstances, (formInstance : any) => {
                return this.isBillingAddressFormInstance(formInstance);
            });
        }

        private isBillingAddressFormInstance(formInstance : any) : boolean {
            var isBillingAddressFormInstance = formInstance.$element.is('form#BillingAddressRegistered');
            return isBillingAddressFormInstance;
        }

        private enableBillingAddressFormValidation() {

            this.formInstances = this.formInstances.concat(this.registerFormsForValidation($('form#BillingAddressRegistered')));
        }

        private renderFailedForm(status: string) {
            //TODO
        }

        protected handleError(reason : any) {

            this.eventHub.publish('cartUpdatingFailed', null);

            console.error('The user changed the billing address, but an error occured when updating the preferred billing address', reason);
        }
    }
}
