///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../Mvc/ComposerClient.ts' />
///<reference path='../../../Cache/CacheProvider.ts' />
///<reference path='../../../Cache/CacheError.ts' />
///<reference path='../IWishListRepository.ts' />
///<reference path='./IWishListService.ts' />


module Orckestra.Composer {
    'use strict';

    export class WishListService implements IWishListService {

        private static GettingFreshWishListSummary: Q.Promise<any>;

        private wishListRepository: IWishListRepository;
        private cacheKey: string = 'WishListSummaryViewModel';
        private cachePolicy: ICachePolicy = { slidingExpiration: 300 }; // 5min
        private cacheProvider: ICacheProvider;
        private eventHub: IEventHub;

        constructor(wishListRepository: IWishListRepository, eventHub: IEventHub) {

            if (!wishListRepository) {
                throw new Error('Error: wishListRepository is required');
            }
            if (!eventHub) {
                throw new Error('Error: eventHub is required');
            }

            this.wishListRepository = wishListRepository;
            this.cacheProvider = CacheProvider.instance();
            this.eventHub = eventHub;
        }

        public getWishListSummary(): Q.Promise<any> {

            return this.getCacheWishListSummary()
                .fail(reason => {

                    if (this.canHandle(reason)) {
                        return this.getFreshWishListSummary();
                    }

                    console.error('An error occured while getting the wishList from cache.', reason);
                    throw reason;
                });
        }

        public getFreshWishListSummary(): Q.Promise<any> {
            if (!WishListService.GettingFreshWishListSummary) {
                WishListService.GettingFreshWishListSummary =
                    this.wishListRepository.getWishListSummary().then(wishList => this.setWishListToCache(wishList));
            }

            // to avoid getting a fresh wishlist multiple times within a page session
            return WishListService.GettingFreshWishListSummary
                .fail((reason) => {
                    console.error('An error occured while getting a fresh wish list.', reason);
                    throw reason;
                });
        }

        public getSignInUrl(): Q.Promise<any> {
            return this.getWishListSummary()
                .then(wishList => {
                    return wishList.SignInUrl;
                });
        }

        public getLineItem(productId: string, variantId?: string): Q.Promise<any> {

            return this.getWishListSummary().then(wishList => {
                if (wishList && wishList.Items) {
                    return wishList.Items.filter(it => it.ProductId === productId && it.VariantId === variantId)[0];
                }
                return null;
            });

        }

        public addLineItem(productId: string, variantId?: string, quantity: number = 1,
            recurringOrderFrequencyName?: string,
            recurringOrderProgramName?: string): Q.Promise<any> {

            var data = {
                ProductId: productId,
                VariantId: variantId,
                Quantity: quantity
            };

            this.eventHub.publish('wishListUpdating', { data: data });

            return this.wishListRepository.addLineItem(productId, variantId, quantity, recurringOrderFrequencyName, recurringOrderProgramName)
                .then(wishList => this.setWishListToCache(wishList))
                .then(wishList => {
                    this.eventHub.publish('wishListUpdated', { data: wishList });
                    return wishList;
                })
                .fail(reason => {
                    this.clearCache();
                    throw reason;
                });
        }

        public removeLineItem(lineItemId: string): Q.Promise<any> {
            var data = {
                LineItemId: lineItemId
            };

            this.eventHub.publish('wishListUpdating', { data: data });

            return this.wishListRepository.deleteLineItem(lineItemId)
                .then(wishList => this.setWishListToCache(wishList))
                .then(wishList => {
                    this.eventHub.publish('wishListUpdated', { data: wishList });
                    return wishList;
                })
                .fail(reason => {
                    this.clearCache();
                    throw reason;
                });
        }

        public clearCache() {
            return this.cacheProvider.defaultCache.clear(this.cacheKey);
        }

        protected getCacheWishListSummary(): Q.Promise<any> {

            return this.cacheProvider.defaultCache.get<any>(this.cacheKey);
        }

        protected setWishListToCache(wishList: any): Q.Promise<any> {

            return this.cacheProvider.defaultCache.set(this.cacheKey, wishList, this.cachePolicy);
        }

        private canHandle(reason: any): boolean {

            return reason === CacheError.Expired || reason === CacheError.NotFound;
        }
    }
}
