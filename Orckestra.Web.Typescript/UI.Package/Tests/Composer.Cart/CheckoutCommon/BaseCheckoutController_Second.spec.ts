///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mocks/MockControllerContext.ts' />
///<reference path='../../../Typescript/IComposerContext.ts' />
///<reference path='../../../Typescript/IComposerConfiguration.ts' />
///<reference path='../../../Typescript/Composer.Cart/CheckoutCommon/BaseCheckoutController.ts' />
///<reference path='./Mocks/SecondMockCheckoutController.ts' />

(() => {

    'use strict';

    var composerContext: Orckestra.Composer.IComposerContext = {
        language: 'en-CA'
    };

    var composerConfiguration: Orckestra.Composer.IComposerConfiguration = {
        controllers: []
    };

    var eventHub: Orckestra.Composer.IEventHub = Orckestra.Composer.EventHub.instance();

    describe('Creating SecondMockCheckoutController', () => {
        var secondMockController: Orckestra.Composer.IBaseCheckoutController,
            controllerName: string;

        beforeEach (() => {
            controllerName = 'SecondMockCheckout';

            secondMockController = new Orckestra.Composer.Mocks.SecondMockCheckoutController(
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
                secondMockController.initialize();
            });

            it('SHOULD have been called registerController', () => {
                expect(secondMockController.viewModelName).toBe(controllerName);
                expect(Orckestra.Composer.CheckoutService.prototype.registerController).toHaveBeenCalledWith(secondMockController);
            });

            describe('WHEN unregistering the controller', () => {

                beforeEach(() => {
                    secondMockController.unregisterController();
                });

                it('SHOULD have been called unregisterController', () => {
                    expect(Orckestra.Composer.CheckoutService.prototype.unregisterController).toHaveBeenCalledWith(controllerName);
                });
            });

            describe('WHEN getting validation promise', () => {
                var promise: Q.Promise<boolean>;

                beforeEach(() => {
                    promise = secondMockController.getValidationPromise();
                });

                it('SHOULD not pass the validation', (done) => {
                    promise
                        .then((validationResult) => {
                            expect(validationResult).toBeFalsy();
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
                    promise = secondMockController.getUpdateModelPromise();
                });

                it('SHOULD return a view model update', (done) => {
                    promise
                        .then((viewModelResult) => {
                            expect(viewModelResult).toEqual({ SecondMockCheckout: 'FirstName: "toto", LastName: "tata"' });
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
                        secondMockController.renderData(context);
                    });
                });

                it('SHOULD not throw an error', () => {
                   expect(() => {
                       func(null);
                   }).not.toThrowError('Method not implemented');
                });
            });

        });
    });

})();
