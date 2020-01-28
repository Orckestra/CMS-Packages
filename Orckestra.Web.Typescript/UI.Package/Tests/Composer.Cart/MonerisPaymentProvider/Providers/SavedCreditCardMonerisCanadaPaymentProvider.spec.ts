///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../../Typescript/Composer.Cart/MonerisPaymentProvider/MonerisCanadaPaymentProvider.ts' />
///<reference path='../../../../Typescript/Composer.Cart/MonerisPaymentProvider/Providers/SavedCreditCardMonerisCanadaPaymentProvider.ts' />
///<reference path='../../../../Typescript/Composer.Cart/MonerisPaymentProvider/ICreateVaultTokenOptions.ts' />
///<reference path='../../../../Typescript/Composer.Cart/MonerisPaymentProvider/IMonerisResponseData.ts' />
///<reference path='../../../../Typescript/Composer.Cart/MonerisPaymentProvider/MonerisPaymentService.ts' />
///<reference path='../../../../Typescript/Composer.Cart/CheckoutPayment/ViewModels/IActivePaymentViewModel.ts' />

(() => {
    //Used to test constructor's logic and dispose logic.
    describe('When SavedCreditCardMonerisCanadaPaymentProvider is built or disposed', () => {
        let onSpy: SinonSpy;
        let offSpy: SinonSpy;
        let provider: Orckestra.Composer.SavedCreditCardMonerisCanadaPaymentProvider;
        let paymentService: Orckestra.Composer.MonerisPaymentService;
        let eventHub: Orckestra.Composer.IEventHub;

        beforeEach(() => {
            let jQueryStub: SinonStub;

            onSpy = sinon.spy();
            offSpy = sinon.spy();
            eventHub = Orckestra.Composer.EventHub.instance();

            jQueryStub = sinon.stub().withArgs(jasmine.any(Object)).returns({
                on: onSpy,
                off: offSpy
            });
            $ = <any>jQueryStub;

            paymentService = new Orckestra.Composer.MonerisPaymentService();

            provider = new Orckestra.Composer.SavedCreditCardMonerisCanadaPaymentProvider(
                <Window>{ },
                paymentService,
                eventHub);
            provider.registerDomEvents();
            provider.unregisterDomEvents();
        });

        afterEach(() => {
            $ = jQuery;
        });

        it('should hook on composer event', () => {
            expect(onSpy.called).toBeTruthy();
        });

        it('should unhook on composer event.', () => {
            expect(offSpy.called).toBeTruthy();
        });
    });

    //Used to validate the validatePayment method.
    describe('WHEN SavedCreditCardMonerisCanadaPaymentProvider.validatePayment is invoked', () => {
        let windowStub: Window;
        let provider: Orckestra.Composer.SavedCreditCardMonerisCanadaPaymentProvider;
        let promiseResponseValue: boolean;
        let eventHub: Orckestra.Composer.IEventHub;

        beforeEach(done => {
            windowStub = <Window> {
                document: <Document>{ }
            };

            $ = <any>sinon.stub().withArgs(jasmine.any(Object)).returns({
                on: () => { /* do nothing */ },
                off: () => { /* do nothing */ }
            });

            eventHub = Orckestra.Composer.EventHub.instance();

            let paymentService = new Orckestra.Composer.MonerisPaymentService();

            provider = new Orckestra.Composer.SavedCreditCardMonerisCanadaPaymentProvider(
                windowStub, paymentService, eventHub);

            provider.registerDomEvents();

            provider
                .validatePayment(<Orckestra.Composer.IActivePaymentViewModel>{ })
                .done(value => {
                    promiseResponseValue = value;
                    done();
                });
        });

        afterEach(() => {
            $ = jQuery;
            provider.unregisterDomEvents();
        });

        it('SHOULD noop and return promise with true as value', () => {
            expect(promiseResponseValue).toBe(true);
        });
    });

    describe('WHEN SavedCreditCardMonerisCanadaPaymentProvider.addVaultProfileToken is invoked', () => {
        let windowStub: Window;
        let provider: Orckestra.Composer.SavedCreditCardMonerisCanadaPaymentProvider;
        let promiseResponseValue: boolean;
        let eventHub: Orckestra.Composer.IEventHub;

        beforeEach(done => {
            windowStub = <Window> {
                document: <Document>{ }
            };

            $ = <any>sinon.stub().withArgs(jasmine.any(Object)).returns({
                on: sinon.spy(),
                off: sinon.spy()
            });

            eventHub = Orckestra.Composer.EventHub.instance();

            let paymentService = new Orckestra.Composer.MonerisPaymentService();

            provider = new Orckestra.Composer.SavedCreditCardMonerisCanadaPaymentProvider(
                    windowStub, paymentService, eventHub);

            provider.registerDomEvents();

            provider
                .addVaultProfileToken(<Orckestra.Composer.IActivePaymentViewModel>{ })
                .done(value => {
                    promiseResponseValue = value;
                    done();
                });
        });

        afterEach(() => {
            $ = jQuery;
            provider.unregisterDomEvents();
        });

        it('SHOULD noop and return promise with an empty object as value', () => {
            expect(_.isEqual(promiseResponseValue, {})).toBe(true);
        });
    });
})();
