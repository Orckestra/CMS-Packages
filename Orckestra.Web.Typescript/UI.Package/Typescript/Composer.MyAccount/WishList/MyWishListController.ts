///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='./WishListController.ts' />

module Orckestra.Composer {

    export class MyWishListController extends Orckestra.Composer.WishListController {

        public initialize() {

            super.initialize();
            this.registerSubscriptions();
        }

        protected registerSubscriptions(): void {
            this.eventHub.subscribe('wishListUpdated', (e: IEventInformation) => this.onWishListUpdated(e));
        }


        public copyShareUrl(actionContext: IControllerActionContext) {
            var shareUrl = $('#txtShareUrl');
            var succeed;

            shareUrl.focus().select();

            try {
                succeed = document.execCommand('copy');
            } catch (e) {
                succeed = false;
            }

            this.eventHub.publish('wishListCopyingShareUrl', {
                data: {}
            });

            return succeed;
        }

        public deleteLineItem(actionContext: IControllerActionContext) {
            var context: JQuery = actionContext.elementContext;
            var lineItemId: string = <any>context.data('lineitemid');
            var container = context.closest('.wishlist-tile');

            container.addClass('is-loading');
            this._wishListService.removeLineItem(lineItemId)
                .then(wishList => {
                    container.parent().remove();
                })
                .fin(() => {
                    container.removeClass('is-loading');
                });
        }

        protected onWishListUpdated(e: IEventInformation): void {
            this.renderWishListQuantity(e.data);

            if (e.data.TotalQuantity === 0) {
                this.context.window.location.reload();
            }
        }

        protected renderWishListQuantity(wishList): void {
            this.render('WishListQuantity', wishList);
        }

        protected getListNameForAnalytics(): string {
            return 'My Wish List';
        }
    }
}
