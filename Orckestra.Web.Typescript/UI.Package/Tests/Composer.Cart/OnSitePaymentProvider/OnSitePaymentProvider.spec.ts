///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../../Typescript/Composer.Cart/OnSitePaymentProvider/OnSitePOSPaymentProvider.ts' />
///<reference path='../../../Typescript/Composer.Cart/CheckoutPayment/ViewModels/IActivePaymentViewModel.ts' />

(() => {
    'use strict';

    describe('When calling validation of OnSitePOSPaymentProvider', () => {
        let posProvider: Orckestra.Composer.OnSitePOSPaymentProvider;
        let validationResult: boolean;
        let eventHub: Orckestra.Composer.IEventHub;

        beforeEach(doneCb => {
            eventHub = Orckestra.Composer.EventHub.instance();
            posProvider = new Orckestra.Composer.OnSitePOSPaymentProvider(<Window>{ }, 'POS', eventHub);

            let promise = posProvider.validatePayment(<Orckestra.Composer.IActivePaymentViewModel> { });

            promise.done((val: boolean) => {
                validationResult = val;
                doneCb();

            }, (reason: any) => {
                fail(reason);
            });
        });

        it('should always return truthy promise', () => {
            expect(validationResult).toBe(true);
        });
    });

    describe('When calling submit payment on OnSitePOSPaymentProvider', () => {
        let posProvider: Orckestra.Composer.OnSitePOSPaymentProvider;
        let result: any;
        let eventHub: Orckestra.Composer.IEventHub;

        beforeEach(doneCb => {
            eventHub = Orckestra.Composer.EventHub.instance();
            posProvider = new Orckestra.Composer.OnSitePOSPaymentProvider(<Window>{}, 'POS', eventHub);

            let promise = posProvider.submitPayment(<Orckestra.Composer.IActivePaymentViewModel> {});

            promise.done((val: any) => {
                result = val;
                doneCb();

            }, (reason: any) => {
                fail(reason);
            });
        });

        it('should always return empty object', () => {
            expect(result).toBeTruthy();
            expect(_.isEmpty(result)).toBe(true);
        });
    });
})();
