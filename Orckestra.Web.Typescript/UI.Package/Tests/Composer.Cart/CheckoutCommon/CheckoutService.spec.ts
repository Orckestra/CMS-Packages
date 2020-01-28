///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mocks/MockControllerContext.ts' />
///<reference path='../../../Typescript/Repositories/CartRepository.ts' />
///<reference path='../../../Typescript/Mvc/ComposerClient.ts' />
///<reference path='../../../Typescript/Events/EventHub.ts' />
///<reference path='../../../Typescript/Composer.Cart/CheckoutCommon/BaseCheckoutController.ts' />

(() => {

    'use strict';

    describe('Getting an instance of CheckoutService', () => {
        var checkoutService: Orckestra.Composer.ICheckoutService,
            checkoutServiceSingleton: Orckestra.Composer.ICheckoutService;

        beforeEach(() => {
            checkoutServiceSingleton = Orckestra.Composer.CheckoutService.getInstance();
        });

        afterEach(() => {
            //
        });

        describe('WHEN calling the constructor', () => {
            it('SHOULD throw an error', () => {
                expect(() => checkoutService = new Orckestra.Composer.CheckoutService())
                    .toThrowError('Instantiation failed: Use CheckoutService.instance() instead of new.');
            });
        });

        describe('WHEN calling getCart when not setting CheckoutStep', () => {
            it('SHOULD throw an error', () => {
                expect(() => {
                        checkoutServiceSingleton.getCart();
                    })
                    .toThrowError('CheckoutService.checkoutStep has not been set or is not a number.');
            });
        });

        describe('WHEN calling updateCart when not setting CheckoutStep', () => {
            it('SHOULD throw an error', () => {
                expect(() => {
                        checkoutServiceSingleton.updateCart();
                    })
                    .toThrowError('CheckoutService.checkoutStep has not been set or is not a number.');
            });
        });

        describe('WHEN calling completeCheckout when not setting CheckoutStep', () => {
            it('SHOULD throw an error', () => {
                expect(() => {
                        checkoutServiceSingleton.completeCheckout();
                    })
                    .toThrowError('CheckoutService.checkoutStep has not been set or is not a number.');
            });
        });

        describe('Setting static checkoutStep to first step', () => {

            beforeEach(() => {
                Orckestra.Composer.CheckoutService.checkoutStep = 1;

                spyOn(Orckestra.Composer.CartService.prototype, 'getCart').and.callFake(() => {
                    return Q(fakeCart());
                })
                .calls.reset();

                 spyOn(Orckestra.Composer.CartService.prototype, 'updateCart').and.callFake(() => {
                    return fakeCartUpdateCart();
                })
                .calls.reset();

                spyOn(Orckestra.Composer.CartService.prototype, 'completeCheckout').and.callFake(() => {
                    return fakeCompleteCheckoutCart();
                })
                .calls.reset();
            });

            afterEach(() => {
                //
            });

            describe('WHEN calling checkoutService.getCart', () => {
                it('SHOULD cartService.getCart have been called', (done) => {

                    checkoutServiceSingleton.getCart()
                        .then(() => {
                            expect(Orckestra.Composer.CartService.prototype.getCart).toHaveBeenCalled();
                            done();
                        })
                        .fail((reason) => {
                            fail('An error occur in getCart test: ' + reason);
                            done();
                        });
                });
            });

            describe('WHEN calling checkoutService.updateCart', () => {
                it('SHOULD cartService.updateCart have been called', (done) => {

                checkoutServiceSingleton.updateCart()
                    .then(() => {
                        expect(Orckestra.Composer.CartService.prototype.updateCart).toHaveBeenCalled();
                        done();
                    })
                    .fail((reason) => {
                        fail('An error occur in updateCart test: ' + reason);
                        done();
                    });
                });
            });

            describe('WHEN calling checkoutService.completeCheckout', () => {
                it('SHOULD cartService.updateCart and cartService.completeCheckout have been called', (done) => {

                checkoutServiceSingleton.completeCheckout()
                    .then(() => {
                        expect(Orckestra.Composer.CartService.prototype.updateCart).not.toHaveBeenCalled();
                        expect(Orckestra.Composer.CartService.prototype.completeCheckout).toHaveBeenCalled();
                        done();
                    })
                    .fail((reason) => {
                        fail('An error occur in completeCheckout test: ' + reason);
                        done();
                    });
                });
            });

        });
    });


    function fakeCart() : any {

        return {
            OrderSummary : {
                CheckoutRedirectAction : {
                    LastCheckoutStep : Orckestra.Composer.CheckoutService.checkoutStep,
                    RedirectUrl : jasmine.any(String)
                }
            }
        };
    }

    function fakeCartUpdateCart() : Q.Promise<any> {

        return Q.fcall<Orckestra.Composer.IUpdateCartResult>(() => {
            return {
                HasErrors: false,
                NextStepUrl: '',
                Cart: fakeCart()
            };
        });
    }

    function fakeCompleteCheckoutCart() : Q.Promise<any> {

        return Q.fcall<Orckestra.Composer.ICompleteCheckoutResult>(() => {
            return {
                OrderNumber: '',
                CustomerEmail: '',
                NextStepUrl: ''
            };
        });
    }

})();
