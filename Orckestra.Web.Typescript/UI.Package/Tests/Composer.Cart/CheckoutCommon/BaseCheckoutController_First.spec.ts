///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mocks/MockControllerContext.ts' />
///<reference path='../../../Typescript/IComposerContext.ts' />
///<reference path='../../../Typescript/IComposerConfiguration.ts' />
///<reference path='../../../Typescript/Composer.Cart/CheckoutCommon/BaseCheckoutController.ts' />
///<reference path='./Mocks/FirstMockCheckoutController.ts' />

(() => {

    'use strict';

    var composerContext: Orckestra.Composer.IComposerContext = {
        language: 'en-CA'
    };

    var composerConfiguration: Orckestra.Composer.IComposerConfiguration = {
        controllers: []
    };

    var eventHub: Orckestra.Composer.IEventHub = Orckestra.Composer.EventHub.instance();

    describe('Creating FirstMockCheckoutController', () => {
        var firstMockController: Orckestra.Composer.IBaseCheckoutController,
            controllerName: string;

        beforeEach (() => {
            controllerName = 'FirstMockCheckout';

            firstMockController = new Orckestra.Composer.Mocks.FirstMockCheckoutController(
                Orckestra.Composer.Mocks.MockControllerContext,
                eventHub,
                composerContext,
                composerConfiguration);

            spyOn(Orckestra.Composer.CheckoutService.prototype, 'registerController')
                .calls.reset();

            spyOn(Orckestra.Composer.CheckoutService.prototype, 'unregisterController')
                .calls.reset();
        });

        afterEach (() => {
           //
        });

        describe('WHEN initializing the controller', () => {

            beforeEach(() => {
                firstMockController.initialize();
            });

            it('SHOULD have been called registerController', () => {
                expect(firstMockController.viewModelName).toBe(controllerName);
                expect(Orckestra.Composer.CheckoutService.prototype.registerController).toHaveBeenCalledWith(firstMockController);
            });

            describe('WHEN unregistering the controller', () => {

                beforeEach(() => {
                    firstMockController.unregisterController();
                });

                it('SHOULD have been called unregisterController', () => {
                    expect(Orckestra.Composer.CheckoutService.prototype.unregisterController).toHaveBeenCalledWith(controllerName);
                });
            });

            describe('WHEN getting validation promise', () => {
                var promise: Q.Promise<boolean>;

                beforeEach(() => {
                    promise = firstMockController.getValidationPromise();
                });

                it('SHOULD pass the validation', (done) => {
                    promise
                        .then((validationResult) => {
                            expect(validationResult).toBeTruthy();
                            done();
                        })
                        .fail((reason) => {
                            fail(reason);
                            done();
                        });
                });
            });

            describe('WHEN getting update view model promise', () => {
                var promise: Q.Promise<any>;

                beforeEach(() => {
                    promise = firstMockController.getUpdateModelPromise();
                });

                it('SHOULD return an empty view model update', (done) => {
                    promise
                        .then((viewModelResult) => {
                            expect(viewModelResult).toEqual({ FirstMockCheckout: '{}' });
                            done();
                        })
                        .fail((reason) => {
                            fail(reason);
                            done();
                        });
                });
            });

            describe('WHEN rendering data', () => {
                var func: Function;

                beforeEach(() => {
                    func = ((context) => {
                        firstMockController.renderData(context);
                    });
                });

                it('SHOULD throw an error', () => {
                   expect(() => {
                       func(null);
                   }).toThrowError('Method not implemented');
                });
            });

        });
    });

})();
