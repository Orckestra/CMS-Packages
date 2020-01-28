///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mocks/MockControllerContext.ts' />
///<reference path='../../../Typescript/Mvc/ComposerClient.ts' />
///<reference path='../../../Typescript/Events/EventHub.ts' />
///<reference path='../../Mocks/MockControllerContext.ts' />
///<reference path='../../../Typescript/Composer.Cart/WishList/WishListRepository.ts' />


module Orckestra.Composer {
    'use strict';

    (() => {

        describe('WHEN delete Wish List item', () => {

            var cut: IWishListRepository;

            beforeEach(() => {
                cut = new WishListRepository();
            });

            it('SHOULD throw an error when the lineItemId is undefined', () => {
                expect(() => cut.deleteLineItem(undefined)).toThrowError(Error);
            });

            it('SHOULD throw an error when the lineItemId is empty', () => {
                expect(() => cut.deleteLineItem('')).toThrowError(Error);
            });
        });
    })();
}
