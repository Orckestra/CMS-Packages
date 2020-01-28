///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='../../Repositories/CartRepository.ts' />
///<reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
///<reference path='./CouponService.ts' />

module Orckestra.Composer {
    /**
     * Controller for the Coupons section.
     */
    export class CouponController extends Orckestra.Composer.Controller {

        private couponService: CouponService = new CouponService(new CartService(new CartRepository(), this.eventHub), this.eventHub);
        private isFirstLoad: boolean = true;

        public initialize() {

            super.initialize();
            this.registerSubscriptions();
        }

        /**
         * Registers events on the eventHub.
         */
        private registerSubscriptions() {
            this.eventHub.subscribe('cartUpdated', (eventInfo: IEventInformation) => {
                var cart = eventInfo.data;

                this.render(this.isFirstLoad || _.isEmpty(cart) || cart.IsCartEmpty ? 'CouponsSummary' : 'Coupons', cart);
                this.isFirstLoad = false;
            });

            this.eventHub.subscribe('couponUpdated', (e: IEventInformation) => {
                this.onCouponUpdated(e.data);
            });
        }

        /**
         * Event triggered when adding a coupon.
         * @param {IControllerActionContext} actionContext - Event context.
         */
        public applyCoupon(actionContext: IControllerActionContext) {
            actionContext.event.preventDefault();

            var formData = (<ISerializeObjectJqueryPlugin>actionContext.elementContext).serializeObject();

            if (_.isEmpty(formData.couponCode)) {
                console.log('The coupon code may not be null');
                return;
            }

            var busy = this.asyncBusy({elementContext: actionContext.elementContext});

            this.couponService.addCoupon(formData.couponCode)
                .done(() => busy.done());
        }

        /**
         * Event triggered when removing a coupon.
         * @param {IControllerActionContext} actionContext - Event context.
         */
        public removeCoupon(actionContext: IControllerActionContext) {
            actionContext.event.preventDefault();

            var busy = this.asyncBusy({elementContext: actionContext.elementContext});

            var couponCode = actionContext.elementContext.data('couponcode');

            if (!couponCode || 0 === couponCode.length) {
                console.log('The coupon code may not be null');
                return;
            }

            this.couponService.removeCoupon(couponCode.toString())
                .fin(() => busy.done())
                .done();
        }

        /**
         * Event handler for coupon updated. Fires even if request fails.
         * @param {any} viewModel - ViewModel received by the request. May be undefined if the request failed.
         */
        private onCouponUpdated(viewModel: any) {
            var formElement = this.getCouponForm();
            var hasErrorMessage = (!viewModel) || _.some(viewModel.Coupons.Messages, m => { return (<any>m).Level === 'danger'; });

            if (!hasErrorMessage) {
                formElement.trigger('reset');
            }
        }

        /**
         * Gets the coupon form handled by this Controller.
         * @return {JQuery} JQuery element matching the first form.
         */
        private getCouponForm() : JQuery {
            var forms = $(this.context.container).find('form:first');
            return forms;
        }
    }
}
