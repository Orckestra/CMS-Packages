///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../../Typescript/Composer.Cart/MonerisPaymentProvider/MonerisCanadaPaymentProvider.ts' />
///<reference path='../../../../Typescript/Composer.Cart/MonerisPaymentProvider/Providers/CreditCardMonerisCanadaPaymentProvider.ts' />
///<reference path='../../../../Typescript/Composer.Cart/MonerisPaymentProvider/ICreateVaultTokenOptions.ts' />
///<reference path='../../../../Typescript/Composer.Cart/MonerisPaymentProvider/IMonerisResponseData.ts' />
///<reference path='../../../../Typescript/Composer.Cart/MonerisPaymentProvider/MonerisPaymentService.ts' />
///<reference path='../../../../Typescript/Composer.Cart/CheckoutPayment/ViewModels/IActivePaymentViewModel.ts' />

(() => {
    //Used to test constructor's logic and dispose logic.
    describe('When CreditCardMonerisCanadaPaymentProvider is built or disposed', () => {
        let onSpy: SinonSpy;
        let offSpy: SinonSpy;
        let provider: Orckestra.Composer.CreditCardMonerisCanadaPaymentProvider;
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

            provider = new Orckestra.Composer.CreditCardMonerisCanadaPaymentProvider(
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
    describe('WHEN CreditCardMonerisCanadaPaymentProvider.validatePayment is invoked', () => {
        let windowStub: Window;
        let documentStub: Document;
        let postMessageSpy: SinonSpy;
        let showErrorSpy: SinonSpy;
        let hideErrorSpy: SinonSpy;
        let jQueryStub: any;
        let eventData: Orckestra.Composer.IMonerisResponseData;
        let cardholderName: string;
        let sut: Orckestra.Composer.CreditCardMonerisCanadaPaymentProvider;
        let paymentService: Orckestra.Composer.MonerisPaymentService;
        let eventHub: Orckestra.Composer.IEventHub = Orckestra.Composer.EventHub.instance();

        beforeEach(() => {
            let monerisFrameRegex = new RegExp('^#monerisFrame$');
            let paymentFormRegex = new RegExp('^#PaymentForm$');
            let messageResponseHandler: Function;
            let frameWindow: Window;
            let iFrameFake: HTMLIFrameElement;
            let formFake: HTMLFormElement;

            windowStub = <Window> {
                document: <Document>{ }
            };
            postMessageSpy = sinon.spy();
            showErrorSpy = sinon.spy();
            hideErrorSpy = sinon.spy();

            frameWindow = <Window> {
                postMessage: function(content: any, captureUrl: string) {
                    postMessageSpy.call(this, content, captureUrl);
                    messageResponseHandler({
                        originalEvent: <MessageEvent> {
                            data: JSON.stringify(eventData)
                        }
                    });
                }
            };

            iFrameFake = <HTMLIFrameElement> {
                contentWindow: frameWindow
            };

            jQueryStub = (selector: any): any => {
                if (selector === windowStub) {
                    return {
                        on: function(eventName: string, handler: Function) {
                            messageResponseHandler = handler;
                        }
                    };
                }

                if (selector === windowStub.document) {
                    return {
                        on: function (eventName: string, selector: string, handler: Function) {
                            messageResponseHandler = handler;
                        }
                    };
                }

                if (_.isString(selector) && monerisFrameRegex.test(selector)) {
                    return [iFrameFake];
                }

                if (_.isString(selector) && paymentFormRegex.test(selector)) {
                    return {
                        serializeObject: () => {
                            return {
                                cardholder: cardholderName
                            };
                        },
                        find: jQueryStub
                    };
                }

                return <JQuery> {
                    addClass: function(className: string) { hideErrorSpy.call(this, className); },
                    removeClass: function(className: string) { showErrorSpy.call(this, className); }
                };
            };
            $ = jQueryStub;

            paymentService = new Orckestra.Composer.MonerisPaymentService();
            sut = new Orckestra.Composer.CreditCardMonerisCanadaPaymentProvider(windowStub, paymentService, eventHub);
            sut.registerDomEvents();
        });

        afterEach(() => {
            $ = jQuery;
        });

        describe('USING a success message', () => {
            var promiseResponse: boolean;

            beforeEach((done) => {
                var promise: Q.Promise<boolean>;

                cardholderName = chance.name();
                eventData = {
                    dataKey: chance.string(),
                    errorMessage: undefined,
                    responseCode: []
                };

                promise = sut.validatePayment({
                    CapturePaymentUrl: chance.url(),
                    Id: chance.guid(),
                    PaymentStatus: 'PendingVerification',
                    ProviderType: 'MonerisCanadaPaymentProvider',
                    ShouldCapturePayment: true,
                    PaymentMethodType: 'CreditCard',
                    ProviderName: 'Moneris',
                    CanSavePaymentMethod: false
                });

                promise.done(val => {
                    promiseResponse = val;
                    done();
                }, reason => {
                    fail(reason);
                    done();
                });
            });

            it('SHOULD return a fulfilled promise with a truthy value', () => {
                expect(promiseResponse).toBeTruthy();
            });

            it('SHOULD call the postMessage on the iFrame', () => {
                expect(postMessageSpy.calledOnce).toBeTruthy();
            });

            it('SHOULD hide any error message', () => {
                expect(hideErrorSpy.called).toBeTruthy();
            });
        });

        describe('USING a failure message', () => {
            var failureResponse: any;

            beforeEach((done) => {
                var promise: Q.Promise<boolean>;

                cardholderName = chance.name();
                eventData = {
                    dataKey: undefined,
                    responseCode: [
                        chance.integer().toString(),
                        chance.integer().toString()
                    ],
                    errorMessage: chance.sentence()
                };

                promise = sut.validatePayment({
                    CapturePaymentUrl: chance.url(),
                    Id: chance.guid(),
                    PaymentStatus: 'PendingVerification',
                    ProviderType: 'MonerisCanadaPaymentProvider',
                    ShouldCapturePayment: true,
                    PaymentMethodType: 'CreditCard',
                    ProviderName: 'Moneris',
                    CanSavePaymentMethod: false
                });

                promise.done(() => {
                    fail('Should not have fulfilled promise');
                    done();
                }, (reason: any) => {
                    failureResponse = reason;
                    done();
                });
            });

            it('SHOULD return a rejected promise with a reason', () => {
                expect(failureResponse).toBeTruthy();
            });

            it('SHOULD call the postMessage on the iFrame', () => {
                expect(postMessageSpy.calledOnce).toBeTruthy();
            });

            it('SHOULD hide errors while validating', () => {
                expect(hideErrorSpy.called).toBeTruthy();
            });

            it('SHOULD show error messages', () => {
                expect(showErrorSpy.called).toBeTruthy();
            });
        });

        describe('USING an invalid cardholder', () => {
            var failureReason: any;

            beforeEach((done) => {
                var promise: Q.Promise<boolean>;

                cardholderName = '';
                eventData = {
                    dataKey: chance.string(),
                    errorMessage: 'invalid',
                    responseCode: []
                };

                promise = sut.validatePayment({
                    CapturePaymentUrl: chance.url(),
                    Id: chance.guid(),
                    PaymentStatus: 'PendingVerification',
                    ProviderType: 'MonerisCanadaPaymentProvider',
                    ShouldCapturePayment: true,
                    PaymentMethodType: 'CreditCard',
                    ProviderName: 'Moneris',
                    CanSavePaymentMethod: false
                });

                promise.done(() => {
                    fail('Promise should not have been fulfilled.');
                    done();
                }, (reason: any) => {
                    failureReason = reason;
                    done();
                });
            });

            it('SHOULD return a rejected promise with a reason', () => {
                expect(failureReason).toBeTruthy();
            });

            it('SHOULD not call the postMessage on the iFrame', () => {
                expect(postMessageSpy.notCalled).toBeTruthy();
            });
        });

        it('SHOULD be correctly mocked', () => {
            var win = jQueryStub(windowStub);
            expect(win.on).toBeTruthy();

            var frameArray = jQueryStub('#monerisFrame');
            expect(_.isArray(jQueryStub('#monerisFrame'))).toBeTruthy();
            expect(jQueryStub('#monerisFrame')[0].contentWindow).toBeTruthy();
            expect(true).toBeTruthy();
        });
    });

    describe('WHEN CreditCardMonerisCanadaPaymentProvider.addVaultProfileToken is invoked', () => {
        let composerClientStub: SinonStub;
        let isSuccess: boolean;
        let paymentProvider: Orckestra.Composer.CreditCardMonerisCanadaPaymentProvider;
        let paymentService: Orckestra.Composer.MonerisPaymentService;
        let eventHub: Orckestra.Composer.IEventHub = Orckestra.Composer.EventHub.instance();

        beforeEach(() => {
            let jQueryStub: SinonStub;
            let windowStub: Window = <Window> {};

            composerClientStub = sinon.stub(Orckestra.Composer.ComposerClient, 'post', (url: string, data: any) => {
                return Q({
                    Success: isSuccess
                });
            });

            jQueryStub = sinon.stub().withArgs(jasmine.any(Object)).returns({
                on: (eventName: string, handler: Function): JQuery => { return <any>jQueryStub; },
                off: (eventName: string, handler: Function): JQuery => { return <any>jQueryStub; }
            });
            $ = <any>jQueryStub;

            paymentService = new Orckestra.Composer.MonerisPaymentService();

            paymentProvider = new Orckestra.Composer.CreditCardMonerisCanadaPaymentProvider(
                windowStub, paymentService, eventHub);

            paymentProvider._formData = { cardholder: chance.name() };
            paymentProvider._monerisResponseData = <any>{ dataKey: chance.string() };
            paymentProvider.registerDomEvents();
        });

        afterEach(() => {
            $ = jQuery;
            composerClientStub.restore();
        });

        describe('USING ShouldCapturePayment=true and successful AJAX call', () => {
            let value: any;
            let vm: Orckestra.Composer.IActivePaymentViewModel;

            beforeEach((done) => {
                let promise: Q.Promise<any>;

                isSuccess = true;
                vm = {
                    CapturePaymentUrl: chance.url(),
                    Id: chance.guid(),
                    PaymentStatus: 'PendingVerification',
                    ProviderType: 'MonerisCanadaPaymentProvider',
                    ShouldCapturePayment: true,
                    PaymentMethodType: 'CreditCard',
                    ProviderName: 'Moneris',
                    CanSavePaymentMethod: false
                };

                promise = paymentProvider.addVaultProfileToken(vm);

                promise.done((val: any) => {
                    value = val;
                    done();
                }, (reason: any) => {
                    fail(reason);
                    done();
                });
            });

            it('SHOULD return a fulfilled promise', () => {
                expect(value).toBeTruthy();
            });

            it('SHOULD return an empty object', () => {
                expect(_.isEmpty(value)).toBeTruthy();
            });

            it('SHOULD have called the ComposerClient', () => {
                expect(composerClientStub.calledOnce).toBeTruthy();
            });

            it('SHOULD have set ShouldCapturePayment to falsy value', () => {
                expect(vm.ShouldCapturePayment).toBeFalsy();
            });
        });

        describe('USING ShouldCapturePayment=false', () => {
            var value: any;
            var vm: Orckestra.Composer.IActivePaymentViewModel;

            beforeEach((done) => {
                var promise: Q.Promise<any>;

                isSuccess = true;
                vm = {
                    CapturePaymentUrl: chance.url(),
                    Id: chance.guid(),
                    PaymentStatus: 'PendingVerification',
                    ProviderType: 'MonerisCanadaPaymentProvider',
                    ShouldCapturePayment: false,
                    PaymentMethodType: 'CreditCard',
                    ProviderName: 'Moneris',
                    CanSavePaymentMethod: false
                };

                promise = paymentProvider.addVaultProfileToken(vm);

                promise.done((val: any) => {
                    value = val;
                    done();
                }, (reason: any) => {
                    fail(reason);
                    done();
                });
            });

            it('SHOULD return a fulfilled promise', () => {
                expect(value).toBeTruthy();
            });

            it('SHOULD return an empty object', () => {
                expect(_.isEmpty(value)).toBeTruthy();
            });

            it('SHOULD not have called the ComposerClient', () => {
                expect(composerClientStub.notCalled).toBeTruthy();
            });
        });

        describe('USING ShouldCapturePayment=true and failed AJAX call', () => {
            let failureReason: any;
            let vm: Orckestra.Composer.IActivePaymentViewModel;

            beforeEach((done) => {
                let promise: Q.Promise<any>;

                isSuccess = false;
                vm = {
                    CapturePaymentUrl: chance.url(),
                    Id: chance.guid(),
                    PaymentStatus: 'PendingVerification',
                    ProviderType: 'MonerisCanadaPaymentProvider',
                    ShouldCapturePayment: true,
                    PaymentMethodType: 'CreditCard',
                    ProviderName: 'Moneris',
                    CanSavePaymentMethod: false
                };

                promise = paymentProvider.addVaultProfileToken(vm);
                promise.done((val: any) => {
                    fail('Promise should have been rejected');
                    done();
                }, (reason: any) => {
                    failureReason = reason;
                    done();
                });
            });

            it('SHOULD have returned a rejected promise with a reason', () => {
                expect(failureReason).toBeTruthy();
            });

            it('SHOULD have called the ComposerClient', () => {
                expect(composerClientStub.calledOnce).toBeTruthy();
            });
        });

        describe('USING two consecutives calls', () => {
            let values: Array<any>;
            let vm: Orckestra.Composer.IActivePaymentViewModel;

            beforeEach((done) => {
                let promise: Q.Promise<any>;

                values = [];
                isSuccess = true;
                vm = {
                    CapturePaymentUrl: chance.url(),
                    Id: chance.guid(),
                    PaymentStatus: 'PendingVerification',
                    ProviderType: 'MonerisCanadaPaymentProvider',
                    ShouldCapturePayment: true,
                    PaymentMethodType: 'CreditCard',
                    ProviderName: 'Moneris',
                    CanSavePaymentMethod: false
                };

                promise = paymentProvider.addVaultProfileToken(vm);
                promise = promise.then((val: any) => {
                    values.push(val);
                    return paymentProvider.addVaultProfileToken(vm).then((val2: any) => {
                        values.push(val2);
                        return values;
                    });
                });

                promise.done((result: Array<any>) => {
                    done();
                }, (reason: any) => {
                    fail(reason);
                    done();
                });
            });

            it('SHOULD call ComposerClient only once', () => {
                expect(composerClientStub.calledOnce).toBeTruthy();
            });

            it('SHOULD return empty objects', () => {
                var areAllEmpty = _.all(values, v => _.isEmpty(v));

                expect(areAllEmpty).toBeTruthy();
            });
        });
    });
})();
