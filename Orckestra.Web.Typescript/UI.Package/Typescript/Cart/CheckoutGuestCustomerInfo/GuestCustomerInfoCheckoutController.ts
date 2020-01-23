///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../CheckoutCommon/BaseCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class GuestCustomerInfoCheckoutController extends Orckestra.Composer.BaseCheckoutController {

        public initialize() {

            this.viewModelName = 'GuestCustomerInfo';

            super.initialize();
        }

        public renderData(checkoutContext: ICheckoutContext): Q.Promise<void> {

            return Q.fcall(() => {
                if (checkoutContext.authenticationViewModel.IsAuthenticated) {
                    this.renderAuthenticated(checkoutContext);
                } else {
                    this.renderUnauthenticated(checkoutContext);
                }
            });
        }

        protected renderAuthenticated(checkoutContext: ICheckoutContext) {

            this.unregisterController();
            this.render(this.viewModelName, checkoutContext.authenticationViewModel);
        }

        protected renderUnauthenticated(checkoutContext: ICheckoutContext) {

            this.registerSubscriptions();
            this.render(this.viewModelName, checkoutContext.cartViewModel);
            this.eventHub.publish(`${this.viewModelName}Rendered`, null);
        }
    }
}
