///<reference path='./BaseSpecializedMonerisCanadaPaymentProvider.ts' />
///<reference path='../MonerisPaymentService.ts' />
///<reference path='../IMonerisResponseData.ts' />
///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
///<reference path='../../../ErrorHandling/ErrorHandler.ts' />
///<reference path='../../CheckoutPayment/ViewModels/IActivePaymentViewModel.ts' />

module Orckestra.Composer {
    'use strict';

    export class CreditCardMonerisCanadaPaymentProvider extends BaseSpecializedMonerisCanadaPaymentProvider {

        private _validationDefer: Q.Deferred<any>;

        /**
         * This property has to be public for the specs to work
         */
        public _monerisResponseData: IMonerisResponseData;

        /**
         * This property is only there to inject test data by the specs
         * it has to be public
         */
        public _formData: any;

        constructor(window: Window, paymentService: MonerisPaymentService, eventHub: IEventHub) {
            super(window, paymentService, eventHub);
        }

        /**
         * Register event handlers for dom events
         */
        public registerDomEvents(): void {
            $(this._window).on('message.composer', this.handleMessageResponse.bind(this));
        }

        /**
         * Unregister event handlers for dom events
         */
        public unregisterDomEvents(): void {
            $(this._window).off('message.composer', this.handleMessageResponse);
        }

        /**
         * Method called to get a promise for payment validation.
         * Returns a promise of boolean. The return boolean needs to be false for validation error,
         * or true if valid.
         * @param   {IActivePaymentViewModel}   The current active payment view model
         * @return  {Q.Promise<boolean>}        Promise that will be executed when we validate the payment control.
         */
        public validatePayment(activePaymentVM: IActivePaymentViewModel): Q.Promise<boolean> {
            return Q
                .fcall(() => this.collectAndValidateFormData())
                .then(_ => {
                    this.hideAllMonerisErrors();

                    this._validationDefer = Q.defer();
                    this._validationDefer.promise
                        .then((monerisRes: IMonerisResponseData) => {
                            this._validationDefer = null;

                            if (!monerisRes.dataKey) {
                                throw new Error('Moneris did not return a dataKey.');
                            }

                            this._monerisResponseData = monerisRes;

                            return true;
                        });

                    if (this._monerisResponseData) {
                        this._validationDefer.resolve(this._monerisResponseData);
                    } else {
                        this.getMonerisIFrame().contentWindow.postMessage('', activePaymentVM.CapturePaymentUrl);
                    }

                    return this._validationDefer.promise;
                });
        }

        private validateMonerisIFrame(vm: IActivePaymentViewModel): Q.Promise<boolean> {
            var monerisFrame: HTMLIFrameElement = this.getMonerisIFrame();
            var monerisContentWindow: Window = monerisFrame.contentWindow;
            var promise: Q.Promise<boolean>;
            this.hideAllMonerisErrors();

            this._validationDefer = Q.defer();
            promise = this._validationDefer.promise
                .then((monerisRes: IMonerisResponseData) => {
                    this._validationDefer = null;

                    if (!monerisRes.dataKey) {
                        throw new Error('Moneris did not return a dataKey.');
                    }

                    this._monerisResponseData = monerisRes;

                    return true;
                });

            if (this._monerisResponseData) {
                this._validationDefer.resolve(this._monerisResponseData);
            } else {
                monerisContentWindow.postMessage('', vm.CapturePaymentUrl);
            }

            return this._validationDefer.promise;
        }

        /**
         * Add the temporary token to the vault profile of the user
         * @param   {IActivePaymentViewModel}   The current active payment view model
         * @return  {Q.Promise<any>}            The object is the updated properties of the cart used in CheckoutService.updateCart()
         */
        public addVaultProfileToken(activePaymentVM : IActivePaymentViewModel): Q.Promise<any> {
            if (!activePaymentVM.ShouldCapturePayment) {
                return Q({});
            }

            let formData = this.collectAndValidateFormData();

            let request: ICreateVaultTokenOptions = {
                CardHolderName: formData.cardholder,
                CreatePaymentProfile: formData.createPaymentProfile,
                VaultTokenId: this._monerisResponseData.dataKey,
                PaymentId: activePaymentVM.Id,
                PaymentProviderName: activePaymentVM.ProviderName
            };

            console.log('Adding Moneris payment information.');

            return this._paymentService
                       .addCreditCard(request)
                       .then((result: IMonerisAddVaultProfileViewModel) => {
                            if (!result.Success) {
                                this._monerisResponseData = null;
                                throw new Error(`Moneris: Could not add the credit card to the payment:
                                                (${result.ErrorCode}) ${result.ErrorMessage}`);
                            }

                            activePaymentVM.ShouldCapturePayment = false;

                            return {};
                       });
        }

        private handleMessageResponse(e: JQueryEventObject) {
            let monerisEvent = <MessageEvent> e.originalEvent;
            let msgData = monerisEvent.data;
            let responseData: IMonerisResponseData = JSON.parse(monerisEvent.data);

            if (responseData.errorMessage && !_.isEmpty(responseData.errorMessage)) {
                this.handleMonerisError(monerisEvent, responseData);
            } else {
                this.handleMonerisSuccess(responseData);
            }
        }

        private handleMonerisSuccess(responseData: IMonerisResponseData): void {
            if (!this._validationDefer) {
                throw new Error('Received Moneris success response, but no validation defer was found.');
            }

            this._validationDefer.resolve(responseData);
        }

        private handleMonerisError(monerisEvent: MessageEvent, responseData: IMonerisResponseData): void {
            var errorMsg: string = `${monerisEvent.origin} SENT (${responseData.responseCode}) ${responseData.dataKey}
            - ${responseData.errorMessage}`;

            this.showMonerisErrors(responseData.responseCode);

            if (this._validationDefer) {
                this._validationDefer.reject(new Error(errorMsg));
            } else {
                console.error(errorMsg, responseData);
            }
        }

        private showMonerisErrors(errorCodes: string[]): void {
            var shouldShowGeneral: boolean = false;

            _.each(errorCodes, (code: string) => {
                var errorNode = $(`#monerisError${code}`);

                if (_.isEmpty(errorNode)) {
                    shouldShowGeneral = true;
                }

                errorNode.removeClass('hide');
            });

            if (shouldShowGeneral) {
                $('#monerisErrorGeneral').removeClass('hide');
            }
        }

        private collectAndValidateFormData(): any {
            let form = <ISerializeObjectJqueryPlugin> this.getForm();
            let formData = this._formData || form.serializeObject();

            if (!formData.cardholder) {
                throw new Error('The form does not contain a field named "cardholder". This is required.');
            }

            return formData;
        }

        private getMonerisIFrame() : HTMLIFrameElement {
            let frames: JQuery = $('#monerisFrame');

            if (_.isEmpty(frames)) {
                throw new Error('Cannot find monerisFrame DOM element');
            }

            return <HTMLIFrameElement>frames[0];
        }

        private hideAllMonerisErrors(): void {
            this.getForm()
                .find('.parsley-errors-list>.parsley-required')
                .addClass('hide');

            $('#monerisErrorGeneral').addClass('hide');
        }

        /**
         * Gets the container for the Payment Provider.
         * @return {JQuery} jQuery object.
         */
        private getForm(): JQuery {
            var form = $('#PaymentForm');

            if (!form || _.isEmpty(form)) {
                throw new Error('Could not find the element PaymentForm on this page.');
            }

            return form;
        }
    }
}
