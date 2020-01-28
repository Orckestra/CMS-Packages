///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../CheckoutCommon/BaseCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class CompleteCheckoutOrderSummaryController extends Orckestra.Composer.BaseCheckoutController {

        public initialize() {
            super.initialize();

            this.viewModelName = 'CompleteCheckoutOrderSummary';

            CheckoutService.checkoutStep = this.context.viewModel.CurrentStep;

            this.registerSubscriptions();
        }

        public renderData(checkoutContext: ICheckoutContext): Q.Promise<void> {

            return Q(this.render(this.viewModelName, checkoutContext.cartViewModel));
        }

        /**
        *  complete Checkout
        *  If errors are returned, it stays in the same page.
        *  If there is no errors it moves to the next step.
        */
        public nextStep(actionContext: IControllerActionContext) {

            var busy: UIBusyHandle = this.asyncBusy();

            this.checkoutService.completeCheckout()
                .then((result: ICompleteCheckoutResult) => {
                    if (_.isEmpty(result.OrderNumber)) {
                        throw {
                            message: 'We could not complete the order because the order number is empty',
                            data: result
                        };
                    }

                    this.eventHub.publish('checkoutCompleted', { data: result });
                    this.checkoutService.setOrderToCache(result);

                    return result;
                })
                .then((result: ICompleteCheckoutResult) => {

                    var orderConfirmationviewModel = {
                        OrderNumber: result.OrderNumber,
                        CustomerEmail: result.CustomerEmail
                    };
                    this.checkoutService.setOrderConfirmationToCache(orderConfirmationviewModel);

                    if (result.NextStepUrl) {
                        window.location.href = result.NextStepUrl;
                    }
                })
                .fail(reason => {
                    console.error('An error occurred while completing the checkout.', reason);
                    ErrorHandler.instance().outputErrorFromCode('CompleteCheckoutFailed');
                    busy.done();
                });
        }

        protected registerSubscriptions() {

            super.registerSubscriptions();

            var handle: number;

            this.eventHub.subscribe('cartUpdating', () => {
                clearTimeout(handle);
                handle = setTimeout(() => this.renderLoading(), 300);
            });

            this.eventHub.subscribe('cartUpdatingFailed', () => {
                clearTimeout(handle);
                this.reRender();
            });

            this.eventHub.subscribe('cartUpdated', e => {
                clearTimeout(handle);
                e.data.GettingCart = false;
                this.render(this.viewModelName, e.data);
            });
        }

        private renderLoading(): void {
            const loadingViewModel = { GettingCart: true };

            return this.render(this.viewModelName, loadingViewModel);
        }

        private reRender(): void {
            this.renderLoading();

            this.checkoutService
                .getCart()
                .then(cartVm => this.render(this.viewModelName, cartVm))
                .fail((reason: any) => this.onRenderDataFailed(reason));
        }
    }
}
