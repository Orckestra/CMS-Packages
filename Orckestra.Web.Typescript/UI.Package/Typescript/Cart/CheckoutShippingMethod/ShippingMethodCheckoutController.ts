///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../CheckoutCommon/BaseCheckoutController.ts' />
///<reference path='./ShippingMethodService.ts' />

module Orckestra.Composer {
    export class ShippingMethodCheckoutController extends Orckestra.Composer.BaseCheckoutController {

        protected debounceMethodSelected: Function = _.debounce(this.methodSelectedImpl, 500, { 'leading': true });

        public initialize() {

            this.viewModelName = 'ShippingMethod';

            super.initialize();
        }

        public renderData(checkoutContext: ICheckoutContext): Q.Promise<void> {

            return Q.fcall(() => {
                var selectedShippingMethodName: string = '';

            if (!_.isUndefined(checkoutContext.cartViewModel.ShippingMethod)) {

                selectedShippingMethodName = checkoutContext.cartViewModel.ShippingMethod.Name;
                checkoutContext.shippingMethodsViewModel.ShippingMethods.forEach(shippingMethod => {

                    if (shippingMethod.Name === selectedShippingMethodName) {
                        checkoutContext.shippingMethodsViewModel.SelectedShippingProviderId = shippingMethod.ShippingProviderId;
                    }
                });
            }

            this.render(this.viewModelName, {
                Methods : checkoutContext.shippingMethodsViewModel,
                SelectedMethod : selectedShippingMethodName,
                HasMethods : !_.isEmpty(checkoutContext.shippingMethodsViewModel.ShippingMethods)
            });

            this.eventHub.publish(`${this.viewModelName}Rendered`, null);
            });
        }

        public methodSelected(actionContext: IControllerActionContext) {

            ErrorHandler.instance().removeErrors();
            this.debounceMethodSelected(actionContext);
        }

        private methodSelectedImpl(actionContext: IControllerActionContext) {

            this.eventHub.publish('cartUpdating', null);

            this.updateShippingProviderId(actionContext)
                .then(() => this.checkoutService.updateCart())
                .then(result => {

                    if (result.HasErrors) {
                        throw new Error('The updated cart contains errors');
                    }

                    this.eventHub.publish('cartUpdated', { data: result.Cart });
                })
                .fail((reason) => this.handleError(reason));
        }

        private updateShippingProviderId(actionContext: IControllerActionContext): Q.Promise<void> {

            return Q.fcall(() => {
                var shippingProviderId = actionContext.elementContext.data('shipping-provider-id');
                $('#ShippingProviderId').val(shippingProviderId.toString());
            });
        }

        protected handleError(reason : any) {

            this.eventHub.publish('cartUpdatingFailed', null);

            console.error('The user changed the shipping method, but we were unable to update the cart dynamically', reason);

            ErrorHandler.instance().outputErrorFromCode('ShippingMethodUpdateFailed');
        }
    }
}
