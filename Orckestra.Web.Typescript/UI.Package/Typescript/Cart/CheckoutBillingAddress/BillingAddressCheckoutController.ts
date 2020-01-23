///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../CheckoutCommon/BaseCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class BillingAddressCheckoutController extends Orckestra.Composer.BaseCheckoutController {

        public initialize() {

            this.viewModelName = 'BillingAddress';

            super.initialize();
        }

         public changeUseShippingAddress() {

            this.setBillingAddressFormVisibility();
            this.setBillingAddressFormValidation();
        }

        public renderData(checkoutContext: ICheckoutContext): Q.Promise<void> {

            return Q.fcall(() => {
                if (checkoutContext.authenticationViewModel.IsAuthenticated) {
                    this.renderAuthenticated(checkoutContext);
                } else {
                    this.renderUnauthenticated(checkoutContext);
                };
            });
        }

        protected renderAuthenticated(checkoutContext: ICheckoutContext) {

            this.unregisterController();
            this.render(this.viewModelName, checkoutContext.authenticationViewModel);
        }

        protected renderUnauthenticated(checkoutContext: ICheckoutContext) {

            this.registerSubscriptions();
            this.render(this.viewModelName, checkoutContext.cartViewModel);
            this.render('AddressRegionPicker', {
                Regions: checkoutContext.regionsViewModel,
                SelectedRegion : this.getRegionCode(checkoutContext.cartViewModel)
            });
            this.eventHub.publish(`${this.viewModelName}Rendered`, checkoutContext.cartViewModel);
        }

        protected registerSubscriptions() {

            this.eventHub.subscribe(`${this.viewModelName}Rendered`, () => this.onRendered());
        }

        protected renderDataFailed(reason: any): void {
            console.error('Failed to render Billing Address control', reason);
            this.context.container.find('.js-loading').hide();

            //TODO: Error handling
        }

        private getRegionCode(cart: any): string {

            if (cart.Payment === undefined ||
                cart.Payment.BillingAddress === undefined ||
                cart.Payment.BillingAddress.RegionCode === undefined ||
                cart.Payment.BillingAddress.UseShippingAddress) {

                return '';
            }

            return cart.Payment.BillingAddress.RegionCode;
        }

        private onRendered() {

            var useShippingAddress: Boolean = this.useShippingAddress();
            this.eventHub.subscribe('postalCodeChanged', e => this.onPostalCodeChanged(useShippingAddress, e.data));

            this.formInstances = this.registerFormsForValidation(this.getVisibleForms());
        }

        private useShippingAddress() : Boolean {

            return $(this.context.container).find('input[name=UseShippingAddress]:checked').val() === 'true';
        }

        private onPostalCodeChanged(useShippingAddress: Boolean, cart: any) {

            if (!useShippingAddress) {
                return;
            }

            var postalCode: string = cart.ShippingAddress.PostalCode;
            this.checkoutService.updatePostalCode(postalCode).done();
        }

        private getVisibleForms(): JQuery {
            var visibleForms = $('form', this.context.container).not('form:has(.hide)');
            return visibleForms;
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

            return formInstance.$element.is('form#BillingAddress');
        }

        private enableBillingAddressFormValidation() {

            this.formInstances = this.formInstances.concat(this.registerFormsForValidation($('form#BillingAddress')));
        }
    }
}
