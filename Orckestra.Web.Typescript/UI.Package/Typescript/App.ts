///<reference path='../Typings/tsd.d.ts' />
///<reference path='./Bootstrap.ts' />
///<reference path='./JQueryPlugins/IPopOverJqueryPlugin.ts' />
///<reference path='./IComposerConfiguration.ts' />
///<reference path='./Controller/SearchBoxController.ts' />
///<reference path='./Controller/PageNotFoundAnalyticsController.ts' />
///<reference path='./Controller/LanguageSwitchController.ts' />
///<reference path='./Product/ProductSearch/SortBySearchController.ts' />
///<reference path='./Product/ProductSearch/FacetSearchController.ts' />
///<reference path='./Product/ProductSearch/SearchResultsController.ts' />
///<reference path='./Product/ProductSearch/SearchSummaryController.ts' />
///<reference path='./Product/ProductSearch/QuickViewController.ts' />
///<reference path='./Product/ProductSearch/SelectedFacetSearchController.ts' />
///<reference path='./Cart/AddToCartNotification/AddToCartNotificationController.ts' />
///<reference path='./Cart/CartSummary/FullCartController.ts' />
///<reference path='./Cart/MiniCart/MiniCartController.ts' />
///<reference path='./Cart/MiniCart/MiniCartSummaryController.ts' />
///<reference path='./Cart/Coupons/CouponController.ts' />
///<reference path='./Product/ProductDetail/ProductDetailController.ts' />
///<reference path='./Product/RelatedProducts/RelatedProductsController.ts' />
///<reference path='./Product/ProductSpecification/ProductSpecificationsController.ts' />
///<reference path='./Product/ProductDetail/ProductZoomController.ts' />
///<reference path='./Cart/OrderSummary/OrderSummaryController.ts' />
///<reference path='./Cart/CheckoutGuestCustomerInfo/GuestCustomerInfoCheckoutController.ts' />
///<reference path='./Cart/CheckoutShippingAddress/ShippingAddressCheckoutController.ts' />
///<reference path='./Cart/CheckoutShippingMethod/ShippingMethodCheckoutController.ts' />
///<reference path='./Cart/CheckoutBillingAddress/BillingAddressCheckoutController.ts' />
///<reference path='./Cart/CheckoutBillingAddressRegistered/BillingAddressRegisteredCheckoutController.ts' />
///<reference path='./Cart/CheckoutOrderConfirmation/CheckoutOrderConfirmationController.ts' />
///<reference path='./Cart/CheckoutComplete/CheckoutCompleteController.ts' />
///<reference path='./MyAccount/AddressList/AddressListController.ts' />
///<reference path='./MyAccount/ChangePassword/ChangePasswordController.ts' />
///<reference path='./MyAccount/CreateAccount/CreateAccountController.ts' />
///<reference path='./MyAccount/EditAddress/EditAddressController.ts' />
///<reference path='./MyAccount/ForgotPassword/ForgotPasswordController.ts' />
///<reference path='./MyAccount/AccountHeader/AccountHeaderController.ts' />
///<reference path='./MyAccount/UpdateAccount/UpdateAccountController.ts' />
///<reference path='./MyAccount/NewPassword/NewPasswordController.ts' />
///<reference path='./MyAccount/ReturningCustomer/ReturningCustomerController.ts' />
///<reference path='./MyAccount/WishList/MyWishListController.ts' />
///<reference path='./MyAccount/WishListShared/SharedWishListController.ts' />
///<reference path='./MyAccount/WishList/WishListInHeaderController.ts' />
///<reference path='./Cart/CheckoutShippingAddressRegistered/ShippingAddressRegisteredController.ts' />
///<reference path='./Cart/CheckoutOrderSummary/CheckoutOrderSummaryController.ts' />
///<reference path='./Cart/CheckoutOrderSummary/CompleteCheckoutOrderSummaryController.ts' />
///<reference path='./Cart/CheckoutPayment/CheckoutPaymentController.ts' />
///<reference path='./Cart/CheckoutCommon/CheckoutNavigationController.ts' />
///<reference path='./MyAccount/SignInHeader/SignInHeaderController.ts' />
///<reference path='./MyAccount/MyAccount/MyAccountController.ts' />
///<reference path='./Cart/OrderHistory/CurrentOrdersController.ts' />
///<reference path='./Cart/OrderHistory/PastOrdersController.ts' />
///<reference path='./Cart/OrderDetails/OrderDetailsController.ts' />
///<reference path='./Cart/FindMyOrder/FindMyOrderController.ts' />
///<reference path='./ErrorHandling/ErrorController.ts' />

///<reference path='./Store/StoreLocator/StoreLocatorController.ts' />
///<reference path='./Store/StoreLocator/StoreDetailsController.ts' />
///<reference path='./Store/StoreDirectory/StoresDirectoryController.ts' />
///<reference path='./Store/StoreInventory/StoreInventoryController.ts' />
///<reference path='./MyAccount/RecurringSchedule/MyRecurringScheduleController.ts' />
///<reference path='./MyAccount/RecurringSchedule/MyRecurringScheduleDetailsController.ts' />
///<reference path='./MyAccount/RecurringCart/MyRecurringCartsController.ts' />
///<reference path='./MyAccount/RecurringCart/MyRecurringCartDetailsController.ts' />

(() => {
    'use strict';

    // This file is currently used for the composer team so that we can deploy and hook into
    // our client-side code, but the starter site that ships, will ship with an App.ts
    // that will look like this.

    $(document).ready(() => {
        let composerConfiguration: Orckestra.Composer.IComposerConfiguration = {
            plugins: [
                'AntiIFrameClickJacking',
                'ComposerValidationLocalization',
                'HelpBubbles',
                'StickyAffix',
                'SlickCarousel',
                'FocusElement',
                'GoogleAnalytics'
            ],

            controllers: [
                { name: 'General.ErrorController', controller: Orckestra.Composer.ErrorController },
                { name: 'General.SearchBox', controller: Orckestra.Composer.SearchBoxController },
                { name: 'General.LanguageSwitch', controller: Orckestra.Composer.LanguageSwitchController },
                { name: 'General.AutocompleteSearchBox', controller: Orckestra.Composer.AutocompleteSearchBoxController },

                { name: 'Cart.FullCart', controller: Orckestra.Composer.FullCartController },
                { name: 'Cart.OrderSummary', controller: Orckestra.Composer.OrderSummaryController },
                { name: 'Cart.MiniCart', controller: Orckestra.Composer.MiniCartController },
                { name: 'Cart.MiniCartSummary', controller: Orckestra.Composer.MiniCartSummaryController },
                { name: 'Cart.Coupons', controller: Orckestra.Composer.CouponController },
                { name: 'Cart.AddToCartNotification', controller: Orckestra.Composer.AddToCartNotificationController },

                { name: 'Product.SortBySearch', controller: Orckestra.Composer.SortBySearchController },
                { name: 'Product.FacetSearch', controller: Orckestra.Composer.FacetSearchController },
                { name: 'Product.ProductDetail', controller: Orckestra.Composer.ProductDetailController },
                { name: 'Product.RelatedProducts', controller: Orckestra.Composer.RelatedProductController },
                { name: 'Product.SearchResults', controller: Orckestra.Composer.SearchResultsController },
                { name: 'Product.SearchSummary', controller: Orckestra.Composer.SearchSummaryController },
                { name: 'Product.QuickView', controller: Orckestra.Composer.QuickViewController },
                { name: 'Product.SelectedSearchFacets', controller: Orckestra.Composer.SelectedFacetSearchController },
                { name: 'Product.ProductSpecifications', controller: Orckestra.Composer.ProductSpecificationsController },
                { name: 'Product.ProductZoom', controller: Orckestra.Composer.ProductZoomController },

                { name: 'Checkout.GuestCustomerInfo', controller: Orckestra.Composer.GuestCustomerInfoCheckoutController },
                { name: 'Checkout.ShippingAddress', controller: Orckestra.Composer.ShippingAddressCheckoutController },
                { name: 'Checkout.ShippingAddressRegistered', controller: Orckestra.Composer.ShippingAddressRegisteredController },
                { name: 'Checkout.ShippingMethod', controller: Orckestra.Composer.ShippingMethodCheckoutController },
                { name: 'Checkout.OrderSummary', controller: Orckestra.Composer.CheckoutOrderSummaryController },
                { name: 'Checkout.CompleteOrderSummary', controller: Orckestra.Composer.CompleteCheckoutOrderSummaryController },
                { name: 'Checkout.CheckoutComplete', controller: Orckestra.Composer.CheckoutCompleteController },
                { name: 'Checkout.CheckoutOrderConfirmation', controller: Orckestra.Composer.CheckoutOrderConfirmationController },
                { name: 'Checkout.BillingAddress', controller: Orckestra.Composer.BillingAddressCheckoutController },
                { name: 'Checkout.BillingAddressRegistered', controller: Orckestra.Composer.BillingAddressRegisteredCheckoutController },
                { name: 'Checkout.Payment', controller: Orckestra.Composer.CheckoutPaymentController },
                { name: 'Checkout.Navigation', controller: Orckestra.Composer.CheckoutNavigationController },

                { name: 'MyAccount.AddressList', controller: Orckestra.Composer.AddressListController },
                { name: 'MyAccount.ChangePassword', controller: Orckestra.Composer.ChangePasswordController },
                { name: 'MyAccount.CreateAccount', controller: Orckestra.Composer.CreateAccountController },
                { name: 'MyAccount.EditAddress', controller: Orckestra.Composer.EditAddressController },
                { name: 'MyAccount.ForgotPassword', controller: Orckestra.Composer.ForgotPasswordController },
                { name: 'MyAccount.AccountHeader', controller: Orckestra.Composer.AccountHeaderController },
                { name: 'MyAccount.UpdateAccount', controller: Orckestra.Composer.UpdateAccountController },
                { name: 'MyAccount.NewPassword', controller: Orckestra.Composer.NewPasswordController },
                { name: 'MyAccount.ReturningCustomer', controller: Orckestra.Composer.ReturningCustomerController },
                { name: 'MyAccount.SignInHeader', controller: Orckestra.Composer.SignInHeaderController },
                { name: 'MyAccount.MyAccountMenu', controller: Orckestra.Composer.MyAccountController },
                { name: 'MyAccount.MyWishList', controller: Orckestra.Composer.MyWishListController },
                { name: 'MyAccount.SharedWishList', controller: Orckestra.Composer.SharedWishListController },
                { name: 'MyAccount.WishListInHeader', controller: Orckestra.Composer.WishListInHeaderController },
                { name: 'MyAccount.MyRecurringSchedule', controller: Orckestra.Composer.MyRecurringScheduleController },
                { name: 'MyAccount.MyRecurringScheduleDetails', controller: Orckestra.Composer.MyRecurringScheduleDetailsController },
                { name: 'MyAccount.MyRecurringCarts', controller: Orckestra.Composer.MyRecurringCartsController },
                { name: 'MyAccount.MyRecurringCartDetails', controller: Orckestra.Composer.MyRecurringCartDetailsController },

                { name: 'Orders.CurrentOrders', controller: Orckestra.Composer.CurrentOrdersController },
                { name: 'Orders.PastOrders', controller: Orckestra.Composer.PastOrdersController },
                { name: 'Orders.OrderDetails', controller: Orckestra.Composer.OrderDetailsController },
                { name: 'Orders.FindMyOrder', controller: Orckestra.Composer.FindMyOrderController },

                { name: 'Store.Locator', controller: Orckestra.Composer.StoreLocatorController },
                { name: 'Store.Details', controller: Orckestra.Composer.StoreDetailsController },
                { name: 'Store.Directory', controller: Orckestra.Composer.StoresDirectoryController },
                { name: 'Store.Inventory', controller: Orckestra.Composer.StoreInventoryController },
                { name: 'PageNotFound.Analytics', controller: Orckestra.Composer.PageNotFoundAnalyticsController }
            ]
        };

        Orckestra.Composer.bootstrap(window, document, composerConfiguration);
    });
})();
