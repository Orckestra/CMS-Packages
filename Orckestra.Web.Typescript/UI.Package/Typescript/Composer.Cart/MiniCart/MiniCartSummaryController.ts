///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Events/EventScheduler.ts' />
///<reference path='../../Repositories/CartRepository.ts' />
///<reference path='../../Events/EventHub.ts' />
///<reference path='../../Events/IEventInformation.ts' />
///<reference path='../../Composer.Analytics/Analytics/GoogleAnalyticsPlugin.ts' />
///<reference path='../CartSummary/CartService.ts' />

module Orckestra.Composer {
    'use strict';

    export class MiniCartSummaryController extends Orckestra.Composer.Controller {

        private cartService: CartService = new CartService(new CartRepository(), this.eventHub);
        private cacheProvider: ICacheProvider = CacheProvider.instance();
        private timer: number;

        public initialize() {
            super.initialize();
            this.initializeMiniCartSummary();
            this.registerSubscriptions();
        }

        private registerSubscriptions(): void {
            this.eventHub.subscribe('cartUpdated', (e: IEventInformation) => {
                this.renderMiniCart(e.data);
            });
            this.eventHub.subscribe('lineItemAddedToCart', (e: IEventInformation) => {
                this.displayMiniCart(e);
            });
            this.eventHub.subscribe('languageSwitched', (e: IEventInformation) => {
                this.invalidateCart(e);
            });
        }

        private invalidateCart(e: IEventInformation): void {
            this.cartService.invalidateCache();
            this.initializeMiniCartSummary();
        }

        private displayMiniCart(e: IEventInformation): void {
            let miniCartContainer = $(this.context.container);
            let notificationTime =  parseInt(miniCartContainer.data('notificationTime'), 10);
            let scrollToLineItemKey = e.data.ProductId + '-' + (e.data.VariantId || '');

            //To reset timer
            clearTimeout(this.timer);

            if (notificationTime > 0) {
                miniCartContainer.addClass('displayMiniCart');

                // Scroll to added item
                $('.minicart-summary-products', miniCartContainer).stop().animate({
                    scrollTop: $('[data-lineitem-id="' + scrollToLineItemKey + '"]', miniCartContainer).position().top
                }, 1000);

                this.timer = setTimeout(function(){
                    miniCartContainer.removeClass('displayMiniCart');
                }, notificationTime);
            }
        }

        private onCloseMiniCart(e: IEventInformation): void {
            let miniCartContainer = $(this.context.container);

            miniCartContainer.addClass('hidden');
            setTimeout(function(){
                miniCartContainer.removeClass('hidden');
            }, 250);

            //Hide the display and cancel the display timer to not have a flickering display
            miniCartContainer.removeClass('displayMiniCart');
            clearTimeout(this.timer);
        }

        protected onCheckout(actionContext: IControllerActionContext): void {
            // Set origin of checkout that we will be used in AnalyticsPlugin
            AnalyticsPlugin.setCheckoutOrigin('Mini Cart Checkout');
        }

        protected initializeMiniCartSummary(): void {
            var busy = this.asyncBusy({containerContext: this.context.container });
            this.cartService.getCart().done(cart => {
                if (!_.isEmpty(cart)) {
                    this.renderMiniCart(cart);
                }
                busy.done();
            });
        }

        protected renderMiniCart(cart: any): void {
            this.render('MinicartSummary', cart);
        }
    }
}
