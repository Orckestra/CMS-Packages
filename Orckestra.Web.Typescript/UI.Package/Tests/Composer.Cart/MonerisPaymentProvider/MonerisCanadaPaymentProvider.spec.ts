///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../../Typescript/Composer.Cart/MonerisPaymentProvider/MonerisCanadaPaymentProvider.ts' />
///<reference path='../../../Typescript/Composer.Cart/MonerisPaymentProvider/Providers/BaseSpecializedMonerisCanadaPaymentProvider.ts' />
///<reference path='../../../Typescript/Composer.Cart/MonerisPaymentProvider/ICreateVaultTokenOptions.ts' />
///<reference path='../../../Typescript/Composer.Cart/MonerisPaymentProvider/IMonerisResponseData.ts' />
///<reference path='../../../Typescript/Composer.Cart/CheckoutPayment/ViewModels/IActivePaymentViewModel.ts' />

(() => {
    // Validate that we instanciate 2 providers
    describe('WHEN MonerisCanadaPaymentProvider is instanciated', () => {
        let providerFacade: Orckestra.Composer.MonerisCanadaPaymentProvider;
        let windowStub: Window;
        let onSpy: SinonSpy;
        let offSpy: SinonSpy;
        let eventHub: Orckestra.Composer.IEventHub;

        beforeEach(() => {
            windowStub = <Window>{};
            onSpy = sinon.spy();
            offSpy = sinon.spy();
            eventHub = Orckestra.Composer.EventHub.instance();

            // called on constructor
            spyOn(Orckestra.Composer.CreditCardMonerisCanadaPaymentProvider.prototype, 'registerDomEvents');
            spyOn(Orckestra.Composer.SavedCreditCardMonerisCanadaPaymentProvider.prototype, 'registerDomEvents');

            // called with dispose
            spyOn(Orckestra.Composer.CreditCardMonerisCanadaPaymentProvider.prototype, 'unregisterDomEvents');
            spyOn(Orckestra.Composer.SavedCreditCardMonerisCanadaPaymentProvider.prototype, 'unregisterDomEvents');

            // stub global jquery object
            $ = <any>sinon.stub().withArgs(jasmine.any(Object)).returns({
                on: onSpy,
                off: offSpy
            });

            providerFacade = new Orckestra.Composer.MonerisCanadaPaymentProvider(
                windowStub,
                'MonerisCanadaPaymentProvider',
                eventHub);

            providerFacade.dispose();
        });

        afterEach(() => {
            $ = jQuery;
        });

        it('SHOULD supports both new and saved credit cards', () => {
            expect(providerFacade.providers['SavedCreditCard']).toBeTruthy();
            expect(providerFacade.providers['CreditCard']).toBeTruthy();
        });

        it('SHOULD register registerDomEvents on all providers with constructor', () => {
            expect(Orckestra.Composer.CreditCardMonerisCanadaPaymentProvider.prototype.registerDomEvents).toHaveBeenCalled();
            expect(Orckestra.Composer.SavedCreditCardMonerisCanadaPaymentProvider.prototype.registerDomEvents).toHaveBeenCalled();
        });

        it('SHOULD register unregisterDomEvents on all providers on dispose', () => {
            expect(Orckestra.Composer.CreditCardMonerisCanadaPaymentProvider.prototype.unregisterDomEvents).toHaveBeenCalled();
            expect(Orckestra.Composer.SavedCreditCardMonerisCanadaPaymentProvider.prototype.unregisterDomEvents).toHaveBeenCalled();
        });
    });
})();
