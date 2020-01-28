///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mocks/MockControllerContext.ts' />
///<reference path='../../../Typescript/Composer.Analytics/Analytics/GoogleAnalyticsPlugin.ts' />
///<reference path='../../../Typescript/Composer.Analytics/Analytics/AnalyticsPlugin.ts' />

module Orckestra.Composer {
    (() => {

        'use strict';

        function CreateOrderWithCouponResultSample() {
            let order = {
                BillingCurrency: 'CAD',
                    Coupons: [{
                        CouponCode: '30DOFF',
                        BillingCurrency: 'CAD',
                        Amount: '50.56',
                        PromotionName: 'Summer Sale'
                    }]
                };

            return order;
        };

        function CreateOrderResultSample() {
            let order = {
                    OrderNumber: '123456',
                    Affiliation: 'Montreal store',
                    Revenu: '123.45',
                    Tax: '12.12',
                    Shipping: '2.56',
                    ShippingOptions: 'Pickup',
                    BillingCurrency: 'CAD'
            };

            return order;
        };

        function CreateExpectedAnalyticsCoupon(data: any) {

            var coupons: IAnalyticsCoupon[] = [];

            _.each(data.Coupons, (coupon: any) => {
                var analyticsCoupon: IAnalyticsCoupon = {
                    code: coupon.CouponCode,
                    discountAmount: coupon.Amount,
                    currencyCode: coupon.BillingCurrency,
                    promotionName: coupon.PromotionName
                };

                coupons.push(analyticsCoupon);
            });

            return coupons;
        };

        function CreateExpectedAnalyticsOrder(data: any) {
            var analyticsOrder : IAnalyticsOrder = {
                id: data.OrderNumber,
                affiliation: data.Affiliation,
                revenue: data.Revenu,
                tax: data.Tax,
                shipping: data.Shipping,
                coupon: '',
                currencyCode: data.BillingCurrency
                };

            return analyticsOrder;
        };

        function CreateExpectedAnalyticsTransaction(data: any, checkoutOrigin: string) {
            var analyticsTransaction : IAnalyticsTransaction = {
                checkoutOrigin: checkoutOrigin,
                shippingType: data.ShippingOptions
            };

            return analyticsTransaction;
        };

        describe('WHEN order is created', () => {

            var checkoutOrigin = 'checkoutOrigin';
            Orckestra.Composer.AnalyticsPlugin.setCheckoutOrigin(checkoutOrigin);

            let analytics : GoogleAnalyticsPlugin;
            let eventHub: IEventHub;

            beforeEach (() => {
                analytics = new GoogleAnalyticsPlugin();
                analytics.initialize();
                eventHub = EventHub.instance();
            });

            describe('WITH coupon code used', () => {

                var order = CreateOrderWithCouponResultSample();

                var expectedAnalyticsCoupon = CreateExpectedAnalyticsCoupon(order);

                it('SHOULD fire the purchase trigger', () => {
                    // arrange
                    spyOn(analytics, 'couponsUsed');

                    // act -- publish
                    eventHub.publish('CheckoutConfirmation', {data: order});

                    // assert -- does it match
                    _.each(expectedAnalyticsCoupon, c => {
                        expect(analytics.couponsUsed).toHaveBeenCalledWith(c);
                    });
                });
            });

            describe('WITH pickup shipping option selected', () => {

                var order = CreateOrderResultSample();

                var expectedAnalyticsOrder = CreateExpectedAnalyticsOrder(order);
                var expectedAnalyticsTransaction = CreateExpectedAnalyticsTransaction(order, checkoutOrigin);

                it('SHOULD fire the purchase trigger', () => {
                    // arrange
                    spyOn(analytics, 'purchase');

                    // act -- publish
                    eventHub.publish('CheckoutConfirmation', {data: order});

                    // assert -- does it match
                    const emptyProducts : IAnalyticsProduct[] = [];
                    expect(analytics.purchase).toHaveBeenCalledWith(expectedAnalyticsOrder, expectedAnalyticsTransaction, emptyProducts);
                });
            });

            describe('WITH checkout origin specified', () => {

                var order = CreateOrderResultSample();
                var checkoutOrigin = 'checkoutOrigin';

                var expectedAnalyticsOrder = CreateExpectedAnalyticsOrder(order);
                var expectedAnalyticsTransaction = CreateExpectedAnalyticsTransaction(order, checkoutOrigin);

                it('SHOULD fire the purchase trigger', () => {
                    // arrange
                    spyOn(analytics, 'purchase');

                    // act -- publish
                    eventHub.publish('CheckoutConfirmation', {data: order});

                    // assert -- does it match
                    const emptyProducts : IAnalyticsProduct[] = [];
                    expect(analytics.purchase).toHaveBeenCalledWith(expectedAnalyticsOrder, expectedAnalyticsTransaction, emptyProducts);
                });
            });
        });
    })();
}