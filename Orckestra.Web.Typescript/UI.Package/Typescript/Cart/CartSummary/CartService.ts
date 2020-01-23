///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Cache/CacheProvider.ts' />
///<reference path='../../Cache/CacheError.ts' />
///<reference path='../../Repositories/CartRepository.ts' />
///<reference path='../../Events/EventHub.ts' />
///<reference path='../../Utils/Utils.ts' />
///<reference path='./ICartService.ts' />

module Orckestra.Composer {
    'use strict';

    export class CartService implements ICartService {

        protected static GettingFreshCart: Q.Promise<any>;

        protected cacheKey: string;
        protected cachePolicy: ICachePolicy = { slidingExpiration: 300 }; // 5min
        protected cacheProvider: ICacheProvider;
        protected cartRepository: ICartRepository;
        protected eventHub: IEventHub;

        constructor(cartRepository: ICartRepository, eventHub: IEventHub) {

            if (!cartRepository) {
                throw new Error('Error: cartRepository is required');
            }
            if (!eventHub) {
                throw new Error('Error: eventHub is required');
            }
            this.cacheKey = `CartViewModel|${Utils.getWebsiteId()}`;
            this.cacheProvider = CacheProvider.instance();
            this.cartRepository = cartRepository;
            this.eventHub = eventHub;
        }

        public getCart(): Q.Promise<any> {

            return this.getCacheCart()
                .fail(reason => {

                    if (this.canHandle(reason)) {
                        return this.getFreshCart();
                    }

                    console.error('An error occured while getting the cart from cache.', reason);
                    throw reason;
                });
        }

        protected canHandle(reason: any): boolean {

            return reason === CacheError.Expired || reason === CacheError.NotFound;
        }

        public getFreshCart(): Q.Promise<any> {

            if (!CartService.GettingFreshCart) {
                CartService.GettingFreshCart = this.cartRepository.getCart()
                    .then(cart => this.setCartToCache(cart));
            }

            // to avoid getting a fresh cart multiple times within a page session
            return CartService.GettingFreshCart
                .fail((reason) => {
                    console.error('An error occured while getting a fresh cart.', reason);
                    throw reason;
                });
        }

        public addLineItem(productId: string, price: string, variantId?: string,
             quantity: number = 1,
             recurringOrderFrequencyName?: string,
             recurringOrderProgramName?: string): Q.Promise<any> {

            var data = {
                ProductId: productId,
                VariantId: variantId,
                Quantity: quantity,
                Price: price
            };

            this.eventHub.publish('cartUpdating', { data: data });

            return this.cartRepository.addLineItem(productId, variantId, quantity, recurringOrderFrequencyName, recurringOrderProgramName)
                .then(cart => this.setCartToCache(cart))
                .then(cart => {
                    let addedToCartData = {
                        Cart: cart,
                        ProductId: productId,
                        VariantId: variantId
                    };

                    this.eventHub.publish('cartUpdated', { data: cart });
                    this.eventHub.publish('lineItemAddedToCart', { data: addedToCartData });
                });
        }

        public updateLineItem(lineItemId: string, quantity: number, productId: string,
            recurringOrderFrequencyName?: string,
            recurringOrderProgramName?: string): Q.Promise<any> {

            var data = {
                LineItemId: lineItemId,
                Quantity: quantity,
                ProductId: productId
            };

            this.eventHub.publish('cartUpdating', { data: data });

            return this.cartRepository.updateLineItem(lineItemId, quantity, recurringOrderFrequencyName, recurringOrderProgramName)
                .then(cart => this.setCartToCache(cart))
                .then(cart => this.eventHub.publish('cartUpdated', { data: cart }));
        }

        public deleteLineItem(lineItemId: string, productId: string): Q.Promise<any> {

            var data = {
                LineItemId: lineItemId,
                ProductId: productId
            };

            this.eventHub.publish('cartUpdating', { data: data });

            return this.cartRepository.deleteLineItem(lineItemId)
                .then(cart => this.setCartToCache(cart))
                .then(cart => this.eventHub.publish('cartUpdated', { data: cart }));
        }

        public updateBillingMethodPostalCode(postalCode: string): Q.Promise<any> {

            return this.cartRepository.updateBillingMethodPostalCode(postalCode)
                .then(cart => this.setCartToCache(cart));
        }

        public updateShippingMethodPostalCode(postalCode: string): Q.Promise<any> {

            return this.cartRepository.updateShippingMethodPostalCode(postalCode)
                .then(cart => this.setCartToCache(cart));
        }

        public setCheapestShippingMethod(): Q.Promise<any> {

            return this.cartRepository.setCheapestShippingMethod()
                .then(cart => this.setCartToCache(cart));
        }

        public addCoupon(couponCode: string): Q.Promise<any> {

            return this.cartRepository.addCoupon(couponCode)
                .then(cart => this.setCartToCache(cart));
        }

        public removeCoupon(couponCode: string): Q.Promise<any> {

            return this.cartRepository.removeCoupon(couponCode)
                .then(cart => this.setCartToCache(cart));
        }

        public clean(): Q.Promise<any> {

            return this.cartRepository.clean()
                .then(cart => this.setCartToCache(cart));
        }

        public updateCart(param: any): Q.Promise<IUpdateCartResult> {

            return this.cartRepository.updateCart(param)
                 .then((result: IUpdateCartResult) => this.setCartToCache(result.Cart).then(() => result));
        }

        public completeCheckout(currentStep: number): Q.Promise<ICompleteCheckoutResult> {

            return this.cartRepository.completeCheckout(currentStep)
                 .then((result: ICompleteCheckoutResult) => this.setCartToCache(null).then(() => result));
        }

        public invalidateCache(): Q.Promise<void> {

            return this.cacheProvider.defaultCache.clear(this.cacheKey);
        }

        protected getCacheCart(): Q.Promise<any> {

            return this.cacheProvider.defaultCache.get<any>(this.cacheKey);
        }

        protected setCartToCache(cart: any): Q.Promise<any> {

            return this.cacheProvider.defaultCache.set(this.cacheKey, cart, this.cachePolicy);
        }
    }
}
