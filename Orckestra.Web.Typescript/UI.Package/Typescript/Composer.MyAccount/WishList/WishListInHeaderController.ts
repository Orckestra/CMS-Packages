///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Events/EventScheduler.ts' />
///<reference path='../../Composer.Cart/WishList/WishListRepository.ts' />
///<reference path='../../Events/EventHub.ts' />
///<reference path='../../Events/IEventInformation.ts' />
///<reference path='../Common/MyAccountEvents.ts' />
///<reference path='../../Composer.Cart/WishList/Services/WishListService.ts' />

module Orckestra.Composer {
    'use strict';

    export class WishListInHeaderController extends Orckestra.Composer.Controller {

        private _wishListService: IWishListService = new WishListService(new WishListRepository(), this.eventHub);

        public initialize() {

            super.initialize();

            this.initializeWishListQuantity();
            this.registerSubscriptions();
        }

        private initializeWishListQuantity(): void {

            if (!this.context.viewModel.IsAuthenticated) {
                this._wishListService.clearCache();
            }

            this._wishListService.getWishListSummary()
                .done(wishList => {

                    if (!_.isEmpty(wishList)) {
                        this.renderWishList(wishList);
                    }
                });
        }

        protected registerSubscriptions(): void {

            var loggedInScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedIn]);
            var loggedOutScheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedOut]);

            this.eventHub.subscribe('wishListUpdated', (e: IEventInformation) => this.onWishListUpdated(e));

            loggedOutScheduler.subscribe((e: IEventInformation) => this.onRefreshUser(e));
            loggedInScheduler.subscribe((e: IEventInformation) => this.onRefreshUser(e));
        }

        protected onWishListUpdated(e: IEventInformation): void {

            var wishList = e.data;
            this.renderWishList(wishList);
        }

        protected onRefreshUser(e: IEventInformation): Q.Promise<any> {

            return this._wishListService.clearCache();
        }

        protected renderWishList(wishList: any): void {

            this.render('WishListQuantity', wishList);
        }

        protected onError(reason: any): void {
            console.error(`An error occured while rendering the wishList with the WishListInHeader.`, reason);
        }
    }
}
