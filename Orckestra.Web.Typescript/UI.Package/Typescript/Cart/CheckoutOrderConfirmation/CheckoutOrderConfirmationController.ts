///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../CheckoutCommon/BaseCheckoutController.ts' />

module Orckestra.Composer {
    'use strict';

export class CheckoutOrderConfirmationController extends Orckestra.Composer.Controller {

        private cacheProvider: ICacheProvider;
        private orderConfirmationCacheKey = 'orderConfirmationCacheKey';
        private orderCacheKey = 'orderCacheKey';

        public initialize() {

            super.initialize();
            this.cacheProvider = CacheProvider.instance();

            this.cacheProvider.defaultCache.get<any>(this.orderCacheKey)
                .then((result: ICompleteCheckoutResult) => {

                    this.eventHub.publish('CheckoutConfirmation', { data: result });
                    this.cacheProvider.defaultCache.clear(this.orderCacheKey).done();
                })
                .fail((reason: any) => {

                    console.error('Unable to retrieve order number from cache, attempt to redirect.');
                });

            this.cacheProvider.defaultCache.get<any>(this.orderConfirmationCacheKey)
                .then((result: ICompleteCheckoutResult) => {

                    var orderConfirmationviewModel = {
                          OrderNumber: result.OrderNumber,
                          CustomerEmail: result.CustomerEmail
                    };

                    if (orderConfirmationviewModel !== undefined) {

                        this.render('CheckoutOrderConfirmation', orderConfirmationviewModel);

                        this.eventHub.publish('checkoutStepRendered', {

                            data: { StepNumber: this.context.viewModel.CurrentStep }
                        });

                    } else {
                        console.error('Order was placed but it is not possible to retrieve order number from cache.');
                    }
                })
                .fail((reason: any) => {

                    console.error('Unable to retrieve order number from cache, attempt to redirect.');

                    var redirectUrl: string = this.context.viewModel.RedirectUrl;

                    if (redirectUrl) {

                        window.location.href = redirectUrl;
                    } else {

                        console.error('Redirect url was not detected.');
                    }
                });
        }
    }
}
