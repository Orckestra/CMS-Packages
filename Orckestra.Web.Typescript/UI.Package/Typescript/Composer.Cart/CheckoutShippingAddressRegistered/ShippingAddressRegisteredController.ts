///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Repositories/CustomerRepository.ts' />
///<reference path='../../Composer.MyAccount/Common/CustomerService.ts' />
///<reference path='../../Composer.MyAccount/Common/MyAccountEvents.ts' />
///<reference path='../../Composer.MyAccount/Common/MyAccountStatus.ts' />
///<reference path='../CheckoutCommon/BaseCheckoutController.ts' />
///<reference path='./ShippingAddressRegisteredService.ts' />
///<reference path='../../UI/UIModal.ts' />

module Orckestra.Composer {
    'use strict';

    export class ShippingAddressRegisteredController extends Orckestra.Composer.BaseCheckoutController {

        private uiModal: UIModal;

        protected debounceChangeShippingMethod: Function = _.debounce(this.changeShippingAddressImpl, 500, { 'leading': true });

        protected deleteModalElementSelector: string;

        protected customerService: ICustomerService = new CustomerService(new CustomerRepository());
        protected shippingAddressRegisteredService: ShippingAddressRegisteredService =
            new ShippingAddressRegisteredService(this.customerService);

        public initialize() {
            super.initialize();

            this.viewModelName = 'ShippingAddressRegistered';

            this.deleteModalElementSelector = '#confirmationModal';

            this.uiModal = new UIModal(window, this.deleteModalElementSelector, this.deleteAddress, this, $(this.context.container));
        }

        protected registerSubscriptions() {

            this.eventHub.subscribe(`${this.viewModelName}Rendered`, e => this.onRendered(e));
            this.eventHub.subscribe(MyAccountEvents[MyAccountEvents.AddressDeleted], e => this.onAddressDeleted(e));
        }

        public renderData(checkoutContext: ICheckoutContext): Q.Promise<void> {

            if (checkoutContext.authenticationViewModel.IsAuthenticated) {

                this.registerSubscriptions();
                this.render(this.viewModelName, {IsAuthenticated: true, GettingCart: true});

                return this.shippingAddressRegisteredService.getShippingAddresses(checkoutContext.cartViewModel)
                    .then((shippingAdressesVm) => {
                        this.render(this.viewModelName, checkoutContext.cartViewModel);
                        this.render('RegisteredAddresses', shippingAdressesVm);
                    })
                    .fail(reason => this.handleError(reason))
                    .fin(() => this.eventHub.publish(`${this.viewModelName}Rendered`, checkoutContext.cartViewModel));
            } else {
                this.unregisterController();
                this.render(this.viewModelName, checkoutContext.authenticationViewModel);
            }
        }

        private onRendered(e: IEventInformation) {

            this.formInstances = this.registerFormsForValidation($('#RegisteredShippingAddress', this.context.container));

            var selectedShippingAddressId: string = $(this.context.container).find('input[name=ShippingAddressId]:checked').val();

            if (!selectedShippingAddressId) {
                return;
            }

            this.checkoutService.getCart()
                .done(cart => {
                    if (selectedShippingAddressId !== cart.ShippingAddress.AddressBookId) {
                        this.debounceChangeShippingMethod();
                    }
                });
        }

        private onAddressDeleted(e: IEventInformation) {

            var addressId = e.data;
            var $addressListItem = $(this.context.container).find('[data-address-id=' + addressId + ']');

            $addressListItem.remove();
        }

        public changeShippingAddress(actionContext: IControllerActionContext) {

            this.debounceChangeShippingMethod();
        }

        private changeShippingAddressImpl() {

            this.eventHub.publish('cartUpdating', null);

            this.checkoutService.updateCart()
                .done(result => {

                    if (result.HasErrors) {
                        throw new Error('The updated cart contains errors');
                    }

                    this.eventHub.publish('cartUpdated', { data: result.Cart });

                }, reason => this.handleError(reason));
        }

        public deleteAddressConfirm(actionContext: IControllerActionContext) {
            this.uiModal.openModal(actionContext.event);
        }

        private deleteAddress(event: JQueryEventObject): Q.Promise<void> {

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

        private renderFailedForm(status: string) {
            //TODO
        }

        private handleError(reason : any) {

            this.eventHub.publish('cartUpdatingFailed', null);

            console.error('The user changed the shipping address, but we were unable to estimate shipping dynamically', reason);
        }
    }
}
