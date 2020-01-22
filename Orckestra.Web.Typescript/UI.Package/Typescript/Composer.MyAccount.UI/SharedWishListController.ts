///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Composer.UI/Controller.ts' />
///<reference path='../Composer.UI/IControllerActionContext.ts' />
///<reference path='../Composer.Cart.UI/WishListRepository.ts' />
///<reference path='../Composer.Cart.UI/WishListService.ts' />
///<reference path='../Composer.Cart.UI/CartService.ts' />
///<reference path='../Composer.UI/CartRepository.ts' />
///<reference path='./WishListController.ts' />

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
