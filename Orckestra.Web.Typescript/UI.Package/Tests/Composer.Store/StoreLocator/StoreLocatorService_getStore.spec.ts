/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../Mocks/MockControllerContext.ts' />
/// <reference path='../../Mocks/MockJqueryEventObject.ts' />
/// <reference path='../../../Typescript/Composer.Store/StoreLocator/Services/StoreLocatorService.ts' />

module Orckestra.Composer {
    'use strict';
    (() => {

        describe('WHEN getting the store', () => {

            var cut: IStoreLocatorService;

            beforeEach(() => {
                cut = new StoreLocatorService();
            });

            it('SHOULD throw an error when the store number is undefined', () => {
                expect(() => cut.getStore(undefined)).toThrowError(Error);
            });

            it('SHOULD throw an error when the store number is null', () => {
                expect(() => cut.getStore(null)).toThrowError(Error);
            });

            it('SHOULD throw an error when the store number is empty', () => {
                expect(() => cut.getStore('')).toThrowError(Error);
            });
        });

        describe('WHEN getting the store for the same store number twice', () => {

            var mock: SinonMock, cut: IStoreLocatorService;

            beforeEach(done => {

                mock = sinon.mock(ComposerClient);
                mock.expects('post').once().returns(Q.fcall(() => jasmine.any));

                cut = new StoreLocatorService();
                cut.getStore('0001')
                    .then(() => cut.getStore('0001'))
                    .done(() => done());
            });

            afterEach(() => {
                mock.restore();
            });

            it('SHOULD send request to composer once', () => {
                mock.verify();
            });
        });

        describe('WHEN getting the store for two different store numbers', () => {

            var mock: SinonMock, cut: IStoreLocatorService;

            beforeEach(done => {

                mock = sinon.mock(ComposerClient);
                mock.expects('post').twice().returns(Q.fcall(() => jasmine.any));

                cut = new StoreLocatorService();
                cut.getStore('0001')
                    .then(() => cut.getStore('0002'))
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