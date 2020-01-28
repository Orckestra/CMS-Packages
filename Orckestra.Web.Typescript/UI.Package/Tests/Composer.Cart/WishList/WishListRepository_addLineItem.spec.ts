///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mocks/MockControllerContext.ts' />
///<reference path='../../../Typescript/Mvc/ComposerClient.ts' />
///<reference path='../../../Typescript/Events/EventHub.ts' />
///<reference path='../../Mocks/MockControllerContext.ts' />
///<reference path='../../../Typescript/Composer.Cart/WishList/WishListRepository.ts' />


module Orckestra.Composer {
    'use strict';

    (() => {

        describe('WHEN add Wish List item', () => {

            var cut: IWishListRepository;

            beforeEach(() => {
                cut = new WishListRepository();
            });

            it('SHOULD throw an error when the productId is undefined', () => {
                expect(() => cut.addLineItem(undefined, 'variantId', 1)).toThrowError(Error);
            });

            it('SHOULD throw an error when the productId is empty', () => {
                expect(() => cut.addLineItem('', 'variantId', 1)).toThrowError(Error);
            });

            it('SHOULD throw an error when the quantity is 0', () => {
                expect(() => cut.addLineItem('productId', 'variantId', 0)).toThrowError(Error);
            });

            it('SHOULD throw an error when the quantity is less than 0', () => {
                expect(() => cut.addLineItem('productId', 'variantId', -1)).toThrowError(Error);
            });

        });
    })();
}
