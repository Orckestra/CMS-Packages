///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../../Typescript/Mvc/ComposerClient.ts' />
///<reference path='../../../Typescript/Events/EventHub.ts' />
///<reference path='../../Mocks/MockControllerContext.ts' />
///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../../Typescript/Mvc/ComposerClient.ts' />
///<reference path='../../../Typescript/Events/EventHub.ts' />
///<reference path='../../Mocks/MockControllerContext.ts' />
///<reference path='../../../Typescript/Composer.Product/ProductSpecification/ProductSpecificationsService.ts' />


module Orckestra.Composer {
    'use strict';

    (() => {

        describe('WHEN getting the product specifications', () => {

            var cut: ProductSpecificationsService;

            beforeEach(() => {
                cut = new ProductSpecificationsService();
            });

            it('SHOULD throw an error when the product id is undefined', () => {
                expect(() => cut.getProductSpecifications(undefined, 'variant1')).toThrowError(Error);
            });

            it('SHOULD throw an error when the product id is null', () => {
                expect(() => cut.getProductSpecifications(null, 'variant1')).toThrowError(Error);
            });

            it('SHOULD throw an error when the product id is empty', () => {
                expect(() => cut.getProductSpecifications('', 'variant1')).toThrowError(Error);
            });
        });

        describe('WHEN getting the product specifications for a variant', () => {

            var mock: SinonMock, cut: ProductSpecificationsService;

            beforeEach(done => {

                mock = sinon.mock(ComposerClient);
                mock.expects('post').once().returns(Q.fcall(() => jasmine.any));

                cut = new ProductSpecificationsService();
                cut.getProductSpecifications('product1', 'variant1')
                   .done(() => done());
            });

            afterEach(() => {
                mock.restore();
            });

            it('SHOULD send request to composer once', () => {
                mock.verify();
            });
        });

        describe('WHEN getting the product specifications for the same variant twice', () => {

            var mock: SinonMock, cut: ProductSpecificationsService;

            beforeEach(done => {

                mock = sinon.mock(ComposerClient);
                mock.expects('post').once().returns(Q.fcall(() => jasmine.any));

                cut = new ProductSpecificationsService();
                cut.getProductSpecifications('product1', 'variant1')
                   .then(() => cut.getProductSpecifications('product1', 'variant1'))
                   .done(() => done());
            });

            afterEach(() => {
                mock.restore();
            });

            it('SHOULD send request to composer once', () => {
                mock.verify();
            });
        });

        describe('WHEN getting the product specifications for two different variants', () => {

            var mock: SinonMock, cut: ProductSpecificationsService;

            beforeEach(done => {

                mock = sinon.mock(ComposerClient);
                mock.expects('post').twice().returns(Q.fcall(() => jasmine.any));

                cut = new ProductSpecificationsService();
                cut.getProductSpecifications('product1', 'variant1')
                   .then(() => cut.getProductSpecifications('product1', 'variant2'))
                   .done(() => done());
            });

            afterEach(() => {
                mock.restore();
            });

            it('SHOULD send request to composer twice', () => {
                mock.verify();
            });
        });

        describe('WHEN getting the product specifications for the same variant thrice', () => {

            var mock: SinonMock, cut: ProductSpecificationsService;

            beforeEach(done => {

                mock = sinon.mock(ComposerClient);
                mock.expects('post').once().returns(Q.fcall(() => jasmine.any));

                cut = new ProductSpecificationsService();
                cut.getProductSpecifications('product1', 'variant1')
                   .then(() => cut.getProductSpecifications('product1', 'variant1'))
                   .then(() => cut.getProductSpecifications('product1', 'variant1'))
                   .done(() => done());
            });

            afterEach(() => {
                mock.restore();
            });

            it('SHOULD send request to composer once', () => {
                mock.verify();
            });
        });

        describe('WHEN getting the product specifications for three different variants', () => {

            var mock: SinonMock, cut: ProductSpecificationsService;

            beforeEach(done => {

                mock = sinon.mock(ComposerClient);
                mock.expects('post').thrice().returns(Q.fcall(() => jasmine.any));

                cut = new ProductSpecificationsService();
                cut.getProductSpecifications('product1', 'variant1')
                   .then(() => cut.getProductSpecifications('product1', 'variant2'))
                   .then(() => cut.getProductSpecifications('product1', 'variant3'))
                   .done(() => done());
            });

            afterEach(() => {
                mock.restore();
            });

            it('SHOULD send request to composer thrice', () => {
                mock.verify();
            });
        });

    })();
}
