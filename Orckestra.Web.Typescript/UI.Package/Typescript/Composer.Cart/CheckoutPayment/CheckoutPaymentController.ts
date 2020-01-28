///<reference path='./Services/IPaymentService.ts' />
///<reference path='./Services/PaymentService.ts' />
///<reference path='./Repositories/PaymentRepository.ts' />
///<reference path='./Providers/BaseCheckoutPaymentProvider.ts' />
///<reference path='./Providers/CheckoutPaymentProviderFactory.ts' />
///<reference path='./ViewModels/IPaymentViewModel.ts' />
///<reference path='./ViewModels/IActivePaymentViewModel.ts' />
///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../ErrorHandling/ErrorHandler.ts' />
///<reference path='../CheckoutCommon/CheckoutService.ts' />
///<reference path='../CheckoutCommon/BaseCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

    export class CheckoutPaymentController extends Orckestra.Composer.BaseCheckoutController {

        protected paymentProviders: Array<BaseCheckoutPaymentProvider>;
        protected activePaymentProvider: BaseCheckoutPaymentProvider;

        /**
         * this prevents changing payment method more than once per click
         * this happens when "new credit card" is selected since we need to
         * listen for both click and change events to handle both when we
         * change to new credit card or when new CC is already selected and
         * we want to come back to the new CC screen
         */
        protected debounceChangePaymentMethod = _.debounce(this.executeChangePaymentMethod, 300);

        /* Public to enable mocking for UT */
        public paymentService: IPaymentService;
        protected viewModel: IPaymentViewModel;

        protected _window: Window;
        protected _busyHandler: UIBusyHandle;

        public initialize() {
            super.initialize();

            this._window = window;
            this.viewModelName = 'CheckoutPayment';
            this.paymentService = new PaymentService(this.eventHub, new PaymentRepository());

            this.paymentProviders = this.getPaymentProviders();
            this.eventHub.subscribe('paymentMethodsUpdated', _ => this.renderData());
            this.registerSubscriptions();
        }

        public selectCreditCardPaymentMethod() {
            if (this.viewModel.CreditCardPaymentMethod) {
                // If a saved credit card is available select the first one
                if (this.viewModel.SavedCreditCards.length > 0) {
                    this.payWithSavedCreditCard();
                } else {
                    this.payWithCreditCard();
                }
            }
        }

        //TODO is it used ?
        public payWithSavedCreditCard() {
            let provider = _.first(this.viewModel.SavedCreditCards);
            this.changePaymentMethodInternal(provider.Id, provider.PaymentProviderName);
        }

        //TODO is it used ?
        public payWithCreditCard() {
            let provider = this.viewModel.CreditCardPaymentMethod;
            this.changePaymentMethodInternal(provider.Id, provider.PaymentProviderName);
        }

        public changePaymentMethodInternal(activeMethodId: string, paymentProviderName: string, paymentType?: string) {
            this.viewModel.IsProviderLoading = true;
            this._busyHandler = super.asyncBusy({ elementContext: $(document), containerContext: $(document) });

            this.debounceChangePaymentMethod({ activeMethodId, paymentProviderName, paymentType });
        }

        public changePaymentMethod(actionContext : IControllerActionContext) {
            var activeMethodId : string = actionContext.elementContext.data('payment-id');
            var paymentProviderName: string = actionContext.elementContext.data('payment-provider');
            var paymentType: string = actionContext.elementContext.data('payment-type');

            this.changePaymentMethodInternal(activeMethodId, paymentProviderName, paymentType);
        }

        protected executeChangePaymentMethod(args: { activeMethodId: string, paymentProviderName: string, paymentType: string }) {
            let { activeMethodId, paymentProviderName, paymentType } = args;

            this.setPaymentMethod(this.viewModel.PaymentId, activeMethodId, paymentProviderName, paymentType)
                .then(_ => this.checkoutService.getCart())
                .then(cart => this.eventHub.publish('cartUpdated', { data: cart }))
                .fin(() => {
                    this.viewModel.IsProviderLoading = false;
                    this.releaseBusyHandler();
                    this.renderView();
                    this.formInstances = this.registerFormsForValidation(this.context.container.find('form'));
                });
        }

        public renderData(): Q.Promise<void> {
            return this.paymentService
                .getPaymentMethods(_.map(this.paymentProviders, p => p.providerName))
                .then((vm: IPaymentViewModel) => {
                    if (!vm) {
                        throw new Error('No viewModel received');
                    }

                    if (_.isEmpty(vm.PaymentMethods)) {
                        throw new Error('No payment method was found.');
                    }

                    this.viewModel = vm;

                    if (this.viewModel.ActivePaymentViewModel) {
                        this.activePaymentProvider = this.findPaymentProviderByType(vm.ActivePaymentViewModel.ProviderType);

                        return this.viewModel;
                    } else {
                        let defaultPaymentMethod = this.findDefaultPaymentMethod(vm.PaymentMethods);

                        // only set the payment method if we found an appropriate default payment method
                        if (!!defaultPaymentMethod) {
                            // If we don't already have an active payment selected
                            // we found a defaultPaymentMethod (either the default or the only one)
                            // there is always going to be at least one (default provider + saved payment methods)
                            return this.setPaymentMethod(vm.PaymentId,
                            defaultPaymentMethod.Id,
                            defaultPaymentMethod.PaymentProviderName,
                            defaultPaymentMethod.PaymentType);
                        }
                    }
                })
                .then((vm: IPaymentViewModel) => {
                    this.renderView();

                    this.formInstances = this.registerFormsForValidation(this.context.container.find('form'));
                })
                .fail((reason) => {
                    this.onInitializePaymentMethodFailed(reason);
                });
        }

        public getValidationPromise() : Q.Promise<boolean> {
            if (!this.activePaymentProvider || !this.viewModel.ActivePaymentViewModel) {
                return Q(false);
            }

            if (!this.isValidForUpdate()) {
                return Q(false);
            }

            return this.activePaymentProvider
                       .validatePayment(this.viewModel.ActivePaymentViewModel);
        }

        public getUpdateModelPromise(): Q.Promise<any> {
            return Q.fcall(() => {
                console.log('Committing payment information.');

                return this.activePaymentProvider.submitPayment(this.viewModel.ActivePaymentViewModel);
            });
        }

        public dispose() {
            super.dispose();
            _.each(this.paymentProviders, p => p.dispose());
        }

        protected findDefaultPaymentMethod(paymentMethods: Array<IPaymentMethodViewModel>): IPaymentMethodViewModel {
            let defaultPaymentMethod = _.find(paymentMethods, (paymentMethod) => paymentMethod.Default && paymentMethod.IsValid);

            if (!!defaultPaymentMethod) {
                return defaultPaymentMethod;
            } else {
                return _.find(paymentMethods, (paymentMethod) => paymentMethod.IsValid);
            }

            //return null;
        }

        protected onInitializePaymentMethodFailed(reason: any) {
            super.removeLoading();
            console.error('Error while retrieving payment methods', reason);

            ErrorHandler.instance().outputErrorFromCode('CheckoutRenderFailed');
        }

        protected renderView(): void {

            this.render(this.viewModelName, this.viewModel);
        }

        protected setPaymentMethod(
            paymentId: string,
            activeMethodId: string,
            paymentProviderName: string,
            paymentType: string): Q.Promise<any> {
            return this.paymentService.setPaymentMethod({
                PaymentId: paymentId,
                PaymentProviderName: paymentProviderName,
                PaymentMethodId: activeMethodId,
                PaymentType: paymentType,
                Providers: _.map(this.paymentProviders, p => p.providerName)
            })
            .then((paymentViewModel: IPaymentViewModel) => {
                this.activePaymentProvider = this.findPaymentProviderByType(paymentViewModel.ActivePaymentViewModel.ProviderType);

                this.viewModel = paymentViewModel;

                ErrorHandler.instance().removeErrors();

                return paymentViewModel;
            })
            .fail(reason => {
                this.onChangePaymentMethodFailed(reason);
            });
        }

        protected onChangePaymentMethodFailed(reason: any) {
            if (this.viewModel) {
                this.viewModel.ActivePaymentViewModel = null;
                this.viewModel.IsLoading = false;
            }

            console.error('Error while changing the payment method.', reason);
            ErrorHandler.instance().outputErrorFromCode('PaymentMethodChangeFailed');
        }

        protected findPaymentProviderByType(providerType: string) : BaseCheckoutPaymentProvider {
            var provider : BaseCheckoutPaymentProvider = _.find(this.paymentProviders, provider => {
                return provider.providerType === providerType;
            });

            if (!provider) {
                throw new Error('Unable to resove any payment provider named "' + providerType + '"');
            }

            return provider;
        }

        protected getPaymentProviders() : Array<BaseCheckoutPaymentProvider> {
            if (_.isEmpty(this.context.viewModel.PaymentProviders)) {
                console.error('No payment provider was found in the Context.');
            }

            var factory = new CheckoutPaymentProviderFactory(this._window, this.eventHub);
            var providers : Array<BaseCheckoutPaymentProvider> = [];

            _.each(this.context.viewModel.PaymentProviders, (vm: any) => {
                var provider = factory.getInstance(vm.ProviderType, vm.ProviderName);
                providers.push(provider);
            });

            return providers;
        }

        protected releaseBusyHandler(): void {
            if (this._busyHandler) {
                this._busyHandler.done();
                this._busyHandler = null;
            }
        }
    }
}
