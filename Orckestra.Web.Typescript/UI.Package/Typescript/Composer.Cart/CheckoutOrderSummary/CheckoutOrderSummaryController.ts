///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../CheckoutCommon/BaseCheckoutController.ts' />
///<reference path='../RecurringOrder/Services/RecurringOrderService.ts' />
///<reference path='../RecurringOrder/Services/IRecurringOrderService.ts' />
///<reference path='../RecurringOrder/Repositories/RecurringOrderRepository.ts' />

module Orckestra.Composer {
    'use strict';

    export class CheckoutOrderSummaryController extends Orckestra.Composer.BaseCheckoutController {

        protected recurringOrderService: IRecurringOrderService = new RecurringOrderService(new RecurringOrderRepository(), this.eventHub);

        public initialize() {

            this.viewModelName = 'CheckoutOrderSummary';

            super.initialize();

            this.registerSubscriptions();

            CheckoutService.checkoutStep = this.context.viewModel.CurrentStep;
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
                e.data.LoadCheckoutOrderSummary = true;
                this.render(this.viewModelName, e.data);
            });
        }

        public renderData(checkoutContext: ICheckoutContext): Q.Promise<void> {

            this.handleRecurringOrderCheckoutSecurity(checkoutContext);

            return Q.fcall(() => {
                checkoutContext.cartViewModel.LoadCheckoutOrderSummary = true;
                this.render(this.viewModelName, checkoutContext.cartViewModel);
            });
        }

        private reRender(): void {

            this.renderLoading();

            this.checkoutService.getCart()
                .then(cartVm => {
                    cartVm.LoadCheckoutOrderSummary = true;
                    this.render(this.viewModelName, cartVm);
                })
                .fail((reason: any) => this.onRenderDataFailed(reason));
        }

        private renderLoading(): void {

            return this.render(this.viewModelName, { LoadCheckoutOrderSummary: true, GettingCart: true });
        }

        /**
         *  Update the cart.
         *  If errors are returned, it stay in the same page.
         *  If there is no errors it moves to the next step.
         */
        public nextStep(actionContext: IControllerActionContext) {

            var busy: UIBusyHandle = this.asyncBusy({elementContext: actionContext.elementContext});

            ErrorHandler.instance().removeErrors();

            this.checkoutService.updateCart()
                .then(result => {

                    if (result.HasErrors === false) {
                        window.location.href = result.NextStepUrl;
                    } else {
                        console.error('Error while updating the cart');
                        busy.done();
                    }
                })
                .fail(reason => {
                    console.error('Error on checkout submit.', reason);
                    ErrorHandler.instance().outputErrorFromCode('CheckoutNextStepFailed');
                    busy.done();
                });
        }

        /**
         * If user has recurring items and is not logged in, redirect to checkout login.
         */
        public handleRecurringOrderCheckoutSecurity(checkoutContext: ICheckoutContext): any {

            var isAuthenticated = checkoutContext.authenticationViewModel.IsAuthenticated;
            var containsRecurring = checkoutContext.cartViewModel.HasRecurringLineitems;

            var anonymousCartSignInUrlPromise = this.recurringOrderService.getAnonymousCartSignInUrl();

            Q.all([anonymousCartSignInUrlPromise])
                .spread((anonymousCartSignInUrl) => {
                    if (containsRecurring && !isAuthenticated) {
                        window.location.href = anonymousCartSignInUrl;
                    }
                });
        }
    }
}
