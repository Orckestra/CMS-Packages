/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../Cache/CacheError.ts' />
/// <reference path='../../Cache/ICache.ts' />
/// <reference path='../../Cache/ICachePolicy.ts' />
/// <reference path='../../Mvc/ComposerClient.ts' />
/// <reference path='./IWishListRepository.ts' />

module Orckestra.Composer {

    export class WishListRepository implements Orckestra.Composer.IWishListRepository {


        public getWishList(): Q.Promise<any> {

            return ComposerClient.get('/api/wishlist/getwishlist');

        }

        public getWishListSummary(): Q.Promise<any> {

           return ComposerClient.get('/api/wishlist/getwishlistsummary');
        }

        public addLineItem(productId: string, variantId: string, quantity: number,
            recurringOrderFrequencyName?: string,
            recurringOrderProgramName?: string): Q.Promise<void> {

            if (!productId) {
                throw new Error('The product id is required');
            }

            if (quantity <= 0) {
                throw new Error('The quantity must be greater than zero');
            }

            var data = {
                ProductId: productId,
                VariantId: variantId,
                Quantity: quantity,
                RecurringOrderFrequencyName: recurringOrderFrequencyName,
                RecurringOrderProgramName: recurringOrderProgramName
            };

            return ComposerClient.post('/api/wishlist/lineitem', data);
        }

        public deleteLineItem(lineItemId: string): Q.Promise<void> {

            if (!lineItemId) {
                throw new Error('The line item id is required');
            }

            var data = {
                LineItemId: lineItemId
            };

            return ComposerClient.remove('/api/wishlist/lineitem', data);
        }

    }
}
