///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../CheckoutCommon/BaseCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class CheckoutCompleteController extends Orckestra.Composer.BaseCheckoutController {

        public initialize() {

            super.initialize();
            this.viewModelName = 'CompleteCheckout';
        }

        public renderData(checkoutContext: ICheckoutContext): Q.Promise<void> {

            return Q(this.render('CheckoutComplete', checkoutContext.cartViewModel));
        }
    }
}
