///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Events/EventScheduler.ts' />
///<reference path='../../Repositories/CartRepository.ts' />
///<reference path='../../Events/EventHub.ts' />
///<reference path='../../Events/IEventInformation.ts' />
///<reference path='../CartSummary/CartService.ts' />
///<reference path='../../Composer.MyAccount/Common/MyAccountEvents.ts' />

module Orckestra.Composer {
    'use strict';

    export class MiniCartController extends Orckestra.Composer.Controller {

        private cartService: CartService = new CartService(new CartRepository(), this.eventHub);

        public initialize() {

            super.initialize();

            this.initializeMiniCartQuantity();
            this.registerSubscriptions();
        }

        private initializeMiniCartQuantity(): void {

            this.cartService.getCart()
                .done(cart => {

                    if (!_.isEmpty(cart)) {
                        this.renderCart(cart);
                    }
                });
        }

        protected registerSubscriptions(): void {

            var loggedInScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedIn]);
            var loggedOutScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedOut]);

            this.eventHub.subscribe('cartUpdated', (e: IEventInformation) => this.onCartUpdated(e));

            loggedOutScheduler.subscribe((e: IEventInformation) => this.onRefreshUser(e));
            loggedInScheduler.subscribe((e: IEventInformation) => this.onRefreshUser(e));
        }

        protected onCartUpdated(e: IEventInformation): void {

            var cart = e.data;
            this.renderCart(cart);
        }

        protected onRefreshUser(e: IEventInformation): Q.Promise<any> {

            return this.cartService.invalidateCache();
        }

        protected renderCart(cart: any): void {

            var viewModel: any = (_.isEmpty(cart) || cart.TotalQuantity === 0) ? {} : cart;
            this.render('MinicartQuantity', viewModel);
        }

        protected onError(reason: any): void {
            console.error(`An error occured while rendering the cart with the MiniCartController.`, reason);
        }
    }
}
