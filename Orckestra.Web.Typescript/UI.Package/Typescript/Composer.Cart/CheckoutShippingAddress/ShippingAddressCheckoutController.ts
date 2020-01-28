///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../CheckoutCommon/BaseCheckoutController.ts' />
///<reference path='../CartSummary/CartService.ts' />
///<reference path='./ShippingAddressCheckoutService.ts' />

module Orckestra.Composer {
    export class ShippingAddressCheckoutController extends Orckestra.Composer.BaseCheckoutController {

        protected shippingAddressCheckoutService: ShippingAddressCheckoutService =
            new ShippingAddressCheckoutService(new CartService(new CartRepository(), this.eventHub), this.eventHub);

        public initialize() {

            this.viewModelName = 'ShippingAddress';

            super.initialize();
        }

        public renderData(checkoutContext: ICheckoutContext): Q.Promise<void> {

            return Q.fcall(() => {
                if (checkoutContext.authenticationViewModel.IsAuthenticated) {
                    this.unregisterController();
                    this.render(this.viewModelName, checkoutContext.authenticationViewModel);
                } else {
                    this.registerSubscriptions();
                    this.render(this.viewModelName, checkoutContext.cartViewModel);
                    this.render('AddressRegionPicker', {
                        Regions: checkoutContext.regionsViewModel,
                        SelectedRegion : this.getRegionCode(checkoutContext.cartViewModel)
                    });

                    this.eventHub.publish(`${this.viewModelName}Rendered`, checkoutContext.cartViewModel);
                }
            });
        }

        private getRegionCode(cart: any): string {

            if (cart.ShippingAddress === undefined || cart.ShippingAddress.RegionCode === undefined) {
                return '';
            }

            return cart.ShippingAddress.RegionCode;
        }

        public changePostalCode(actionContext: IControllerActionContext) {

            var context: JQuery = actionContext.elementContext;
            context.val(context.val().toUpperCase());

            if (!this.isValidForUpdate()) {
                return;
            }

            var postalCode: string = context.val();
            this.shippingAddressCheckoutService.setCheapestShippingMethodUsing(postalCode).done();
        }
    }
}
