///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../../Typescript/Mvc/ComposerClient.ts' />
///<reference path='../../../Typescript/Events/EventHub.ts' />
///<reference path='../../Mocks/MockControllerContext.ts' />
///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../../Typescript/Mvc/ComposerClient.ts' />
///<reference path='../../../Typescript/Events/EventHub.ts' />
///<reference path='../../Mocks/MockControllerContext.ts' />
///<reference path='../../../Typescript/Composer.Product/Product/InventoryService.ts' />


module Orckestra.Composer {
    'use strict';

    (() => {

        describe('WHEN determining whether a product is available for sell', () => {

            var cut: InventoryService;

            beforeEach(() => {
                cut = new InventoryService();
            });

            it('SHOULD throw an error when the sku is undefined', () => {
                expect(() => cut.isAvailableToSell(undefined)).toThrowError(Error);
            });

            it('SHOULD throw an error when the sku is null', () => {
                expect(() => cut.isAvailableToSell(null)).toThrowError(Error);
            });

            it('SHOULD throw an error when the sku is empty', () => {
                expect(() => cut.isAvailableToSell('')).toThrowError(Error);
            });
        });

        describe('WHEN determining whether a product is available for sell', () => {

            var mock: SinonMock, cut: InventoryService;

            beforeEach(done => {

                mock = sinon.mock(ComposerClient);
                mock.expects('post').once().returns(Q.fcall(() => []));

                cut = new InventoryService();
                cut.isAvailableToSell('any sku')
                   .done(() => done());
            });

            afterEach(() => {
                mock.restore();
            });

            it('SHOULD send request to composer once', () => {
                mock.verify();
            });
        });

        describe('WHEN determining whether a product is available for sell twice', () => {

            var mock: SinonMock, cut: InventoryService;

            beforeEach(done => {

                mock = sinon.mock(ComposerClient);
                mock.expects('post').once().returns(Q.fcall(() => []));

                cut = new InventoryService();
                cut.isAvailableToSell('any sku')
                   .then(() => cut.isAvailableToSell('any sku'))
                   .done(() => done());
            });

            afterEach(() => {
                mock.restore();
            });

            it('SHOULD send request to composer once', () => {
                mock.verify();
            });
        });

        describe('WHEN determining whether two products are available for sell', () => {

            var mock: SinonMock, cut: InventoryService;

            beforeEach(done => {

                mock = sinon.mock(ComposerClient);
                mock.expects('post').twice().returns(Q.fcall(() => []));

                cut = new InventoryService();
                cut.isAvailableToSell('sku1')
                   .then(() => cut.isAvailableToSell('sku2'))
                   .done(() => done());
            });

            afterEach(() => {
                mock.restore();
            });

            it('SHOULD send request to composer twice', () => {
                mock.verify();
            });
        });

    })();
}
