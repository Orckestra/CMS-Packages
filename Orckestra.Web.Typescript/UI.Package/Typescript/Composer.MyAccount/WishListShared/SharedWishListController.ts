///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='../../Composer.Cart/WishList/WishListRepository.ts' />
///<reference path='../../Composer.Cart/WishList/Services/WishListService.ts' />
///<reference path='../../Composer.Cart/CartSummary/CartService.ts' />
///<reference path='../../Repositories/CartRepository.ts' />
///<reference path='../WishList/WishListController.ts' />

module Orckestra.Composer {

    export class SharedWishListController extends Orckestra.Composer.WishListController {


        public initialize() {

            super.initialize();
        }

        protected getListNameForAnalytics(): string {
            return 'Shared Wish List';
        }

    }
}
