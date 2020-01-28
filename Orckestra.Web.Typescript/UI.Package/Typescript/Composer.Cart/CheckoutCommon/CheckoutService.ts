///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Composer.MyAccount/Common/MembershipService.ts' />
///<reference path='../../ErrorHandling/ErrorHandler.ts' />
///<reference path='../../Repositories/CartRepository.ts' />
///<reference path='../CheckoutShippingMethod/ShippingMethodService.ts' />
///<reference path='../CartSummary/CartService.ts' />
///<reference path='./IBaseCheckoutController.ts' />
///<reference path='./RegionService.ts' />
///<reference path='./ICheckoutService.ts' />
///<reference path='./ICheckoutContext.ts' />
///<reference path='./IRegisterOptions.ts' />

module Orckestra.Composer {
    'use strict';

    export class CheckoutService implements ICheckoutService {

        private static instance: ICheckoutService;

        public static checkoutStep: number;

        private orderConfirmationCacheKey = 'orderConfirmationCacheKey';
        private orderCacheKey = 'orderCacheKey';

        private window: Window;
        private eventHub: IEventHub;
        private registeredControllers: any = {};
        private allControllersReady: Q.Deferred<boolean>;
        private cacheProvider: ICacheProvider;

        protected cartService: ICartService;
        protected membershipService: IMembershipService;
        protected regionService: IRegionService;
        protected shippingMethodService: ShippingMethodService;

        public static getInstance(): ICheckoutService {

            if (!CheckoutService.instance) {
                CheckoutService.instance = new CheckoutService();
            };

            return CheckoutService.instance;
        }

        public constructor() {

            if (CheckoutService.instance) {
                throw new Error('Instantiation failed: Use CheckoutService.instance() instead of new.');
            }

            this.eventHub = EventHub.instance();
            this.window = window;
            this.allControllersReady = Q.defer<boolean>();
            this.cacheProvider = CacheProvider.instance();

            this.cartService = new CartService(new CartRepository(), this.eventHub);
            this.membershipService = new MembershipService(new MembershipRepository());
            this.regionService = new RegionService();
            this.shippingMethodService = new ShippingMethodService();
            this.registerAllControllersInitialized();

            CheckoutService.instance = this;
        }

        protected registerAllControllersInitialized(): void {

            this.eventHub.subscribe('allControllersInitialized', () => {
                this.initialize();
            });
        }

        private initialize() {

            var authenticatedPromise = this.membershipService.isAuthenticated();
            var getCartPromise = this.getCart();
            var regionsPromise: Q.Promise<any> = this.regionService.getRegions();
            var shippingMethodsPromise: Q.Promise<any> = this.shippingMethodService.getShippingMethods();

            Q.all([authenticatedPromise, getCartPromise, regionsPromise, shippingMethodsPromise])
                .spread((authVm, cartVm, regionsVm, shippingMethodsVm) => {

                    var results: ICheckoutContext = {
                        authenticationViewModel: authVm,
                        cartViewModel: cartVm,
                        regionsViewModel: regionsVm,
                        shippingMethodsViewModel: shippingMethodsVm
                    };

                    return this.renderControllers(results);
                })
                .then(() => {
                    this.allControllersReady.resolve(true);
                })
                .fail((reason: any) => {
                    console.error('Error while initializing CheckoutService.', reason);
                    ErrorHandler.instance().outputErrorFromCode('CheckoutRenderFailed');
                });
        }

        public registerController(controller: IBaseCheckoutController) {

            if (this.allControllersReady.promise.isPending()) {
                this.allControllersReady.resolve(false);
            }

            this.allControllersReady.promise
                .then(allControllersReady => {

                    if (allControllersReady) {
                        throw new Error('Too late to register all controllers are ready.');
                    } else {
                        var controllerName = controller.viewModelName;
                        this.registeredControllers[controllerName] = controller;
                    }
            });
        }

        public unregisterController(controllerName: string) {

            delete this.registeredControllers[controllerName];
        }

        public renderControllers(checkoutContext: ICheckoutContext): Q.Promise<void[]> {

            var controllerInstance: IBaseCheckoutController,
                renderDataPromises: Q.Promise<void>[] = [];

             for (var controllerName in this.registeredControllers) {
                if (this.registeredControllers.hasOwnProperty(controllerName)) {
                    controllerInstance = <IBaseCheckoutController>this.registeredControllers[controllerName];

                    renderDataPromises.push(controllerInstance.renderData(checkoutContext));
                }
            }

            return Q.all(renderDataPromises);
        }

        public updatePostalCode(postalCode: string): Q.Promise<void> {

            return this.cartService.updateBillingMethodPostalCode(postalCode);
        }

        public invalidateCache(): Q.Promise<void> {

            return this.cartService.invalidateCache();
        }

        public getCart(): Q.Promise<any> {

            if (!_.isNumber(CheckoutService.checkoutStep)) {
                throw new Error('CheckoutService.checkoutStep has not been set or is not a number.');
            }

            return this.invalidateCache()
                .then(() => this.cartService.getCart())
                .then(cart => {
                    return this.handleCheckoutSecurity(cart, CheckoutService.checkoutStep);
                })
                .fail(reason => {
                    this.handleError(reason);
                });
        }

        public updateCart(): Q.Promise<IUpdateCartResult> {

            this.allControllersReady.promise
            .then(allControllersReady => {
                if (!allControllersReady) {
                    throw new Error('All registered controllers are not ready.');
                }
            });

            if (!_.isNumber(CheckoutService.checkoutStep)) {
                throw new Error('CheckoutService.checkoutStep has not been set or is not a number.');
            }

            var emptyVm = {
                CurrentStep: CheckoutService.checkoutStep,
                UpdatedCart: {}
            };

            return this.buildCartUpdateViewModel(emptyVm)
                .then(vm => this.cartService.updateCart(vm));
        }

        public completeCheckout(): Q.Promise<ICompleteCheckoutResult> {

            if (!_.isNumber(CheckoutService.checkoutStep)) {
                throw new Error('CheckoutService.checkoutStep has not been set or is not a number.');
            }

            var emptyVm = {
                CurrentStep: CheckoutService.checkoutStep,
                UpdatedCart: {}
            };

            return this.buildCartUpdateViewModel(emptyVm)
                .then(vm => {

                    if (_.isEmpty(vm.UpdatedCart)) {
                        console.log('No modification required to the cart.');
                        return vm;
                    }

                    return this.cartService.updateCart(vm);
                })
                .then(result => {

                    if (result && result.HasErrors) {
                        throw new Error('Error while updating the cart. Complete Checkout will not complete.');
                    }

                    console.log('Publishing the cart!');

                    return this.cartService.completeCheckout(CheckoutService.checkoutStep);
                });
        }

        private buildCartUpdateViewModel(vm: any): Q.Promise<any> {

            var validationPromise: Q.Promise<any>;
            var viewModelUpdatePromise: Q.Promise<any>;

            validationPromise = Q(vm).then(vm => {
                return this.getCartValidation(vm);
            });

            viewModelUpdatePromise = validationPromise.then(vm => {
                return this.getCartUpdateViewModel(vm);
            });

            return viewModelUpdatePromise;
        }

        private getCartValidation(vm : any): Q.Promise<any> {

            var validationPromise = this.collectValidationPromises();

            var promise = validationPromise.then((results : Array<boolean>) => {
                console.log('Aggregrating all validation results');
                var hasFailedValidation = _.any(results, r => !r);

                if (hasFailedValidation) {
                    throw new Error('There were validation errors.');
                }

                return vm;
            });

            return promise;
        }

        private getCartUpdateViewModel(vm: any): Q.Promise<any> {

            var updateModelPromise = this.collectUpdateModelPromises();

            var promise = updateModelPromise.then((updates: Array<any>) => {
                console.log('Aggregating all ViewModel updates.');

                _.each(updates, update => {

                    if (update) {
                       var keys = _.keys(update);
                        _.each(keys, key => {
                            vm.UpdatedCart[key] = update[key];
                        });
                    }
                });

                return vm;
            });

            return promise;
        }

        private collectValidationPromises(): Q.Promise<any> {

            var promises: Q.Promise<any>[] = [];
            var controllerInstance: IBaseCheckoutController;
            var controllerOptions: IRegisterControlOptions;

            for (var controllerName in this.registeredControllers) {

                if (this.registeredControllers.hasOwnProperty(controllerName)) {
                    controllerInstance = <IBaseCheckoutController>this.registeredControllers[controllerName];
                    promises.push(controllerInstance.getValidationPromise());
                }
            }

            return Q.all(promises);
        }

        private collectUpdateModelPromises(): Q.Promise<any> {

            var promises: Q.Promise<any>[] = [];
            var controllerInstance: IBaseCheckoutController;
            var controllerOptions: IRegisterControlOptions;

            for (var controllerName in this.registeredControllers) {

                if (this.registeredControllers.hasOwnProperty(controllerName)) {
                    controllerInstance = <IBaseCheckoutController>this.registeredControllers[controllerName];
                    promises.push(controllerInstance.getUpdateModelPromise());
                }
            }

            return Q.all(promises);
        }

        private handleCheckoutSecurity(cart: any, targetedStep: number): any {

            var lastStep: number = cart.OrderSummary.CheckoutRedirectAction.LastCheckoutStep;
            var redirectUrl: string = cart.OrderSummary.CheckoutRedirectAction.RedirectUrl;

            if (targetedStep > lastStep) {
                this.window.location.href = redirectUrl;
            }

            return cart;
        }

        private handleError(reason: any): void {

            console.error('Unable to retrieve the cart for the checkout', reason);

            throw reason;
        }

        public setOrderConfirmationToCache(orderConfirmationviewModel : any) : void {

            this.cacheProvider.defaultCache.set(this.orderConfirmationCacheKey, orderConfirmationviewModel).done();
        }

        public getOrderConfirmationFromCache(): Q.Promise<any> {

            return this.cacheProvider.defaultCache.get<any>(this.orderConfirmationCacheKey);
        }

        public clearOrderConfirmationFromCache(): void {

             this.cacheProvider.defaultCache.clear(this.orderConfirmationCacheKey).done();
        }

        public setOrderToCache(orderConfirmationviewModel : any) : void {

            this.cacheProvider.defaultCache.set(this.orderCacheKey, orderConfirmationviewModel).done();
        }
    }
}
