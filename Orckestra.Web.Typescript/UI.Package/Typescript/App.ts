///<reference path='../Typings/tsd.d.ts' />
///<reference path='./Bootstrap.ts' />
///<reference path='./JQueryPlugins/IPopOverJqueryPlugin.ts' />
///<reference path='./IComposerConfiguration.ts' />
///<reference path='./Controller/SearchBoxController.ts' />
///<reference path='./Controller/PageNotFoundAnalyticsController.ts' />
///<reference path='./Controller/LanguageSwitchController.ts' />
///<reference path='./Composer.Product/ProductSearch/SortBySearchController.ts' />
///<reference path='./Composer.Product/ProductSearch/FacetSearchController.ts' />
///<reference path='./Composer.Product/ProductSearch/SearchResultsController.ts' />
///<reference path='./Composer.Product/ProductSearch/SearchSummaryController.ts' />
///<reference path='./Composer.Product/ProductSearch/QuickViewController.ts' />
///<reference path='./Composer.Product/ProductSearch/SelectedFacetSearchController.ts' />
///<reference path='./Composer.Cart/AddToCartNotification/AddToCartNotificationController.ts' />
///<reference path='./Composer.Cart/CartSummary/FullCartController.ts' />
///<reference path='./Composer.Cart/MiniCart/MiniCartController.ts' />
///<reference path='./Composer.Cart/MiniCart/MiniCartSummaryController.ts' />
///<reference path='./Composer.Cart/Coupons/CouponController.ts' />
///<reference path='./Composer.Product/ProductDetail/ProductDetailController.ts' />
///<reference path='./Composer.Product/RelatedProducts/RelatedProductsController.ts' />
///<reference path='./Composer.Product/ProductSpecification/ProductSpecificationsController.ts' />
///<reference path='./Composer.Product/ProductDetail/ProductZoomController.ts' />
///<reference path='./Composer.Cart/OrderSummary/OrderSummaryController.ts' />
///<reference path='./Composer.Cart/CheckoutGuestCustomerInfo/GuestCustomerInfoCheckoutController.ts' />
///<reference path='./Composer.Cart/CheckoutShippingAddress/ShippingAddressCheckoutController.ts' />
///<reference path='./Composer.Cart/CheckoutShippingMethod/ShippingMethodCheckoutController.ts' />
///<reference path='./Composer.Cart/CheckoutBillingAddress/BillingAddressCheckoutController.ts' />
///<reference path='./Composer.Cart/CheckoutBillingAddressRegistered/BillingAddressRegisteredCheckoutController.ts' />
///<reference path='./Composer.Cart/CheckoutOrderConfirmation/CheckoutOrderConfirmationController.ts' />
///<reference path='./Composer.Cart/CheckoutComplete/CheckoutCompleteController.ts' />
///<reference path='./Composer.MyAccount/AddressList/AddressListController.ts' />
///<reference path='./Composer.MyAccount/ChangePassword/ChangePasswordController.ts' />
///<reference path='./Composer.MyAccount/CreateAccount/CreateAccountController.ts' />
///<reference path='./Composer.MyAccount/EditAddress/EditAddressController.ts' />
///<reference path='./Composer.MyAccount/ForgotPassword/ForgotPasswordController.ts' />
///<reference path='./Composer.MyAccount/AccountHeader/AccountHeaderController.ts' />
///<reference path='./Composer.MyAccount/UpdateAccount/UpdateAccountController.ts' />
///<reference path='./Composer.MyAccount/NewPassword/NewPasswordController.ts' />
///<reference path='./Composer.MyAccount/ReturningCustomer/ReturningCustomerController.ts' />
///<reference path='./Composer.MyAccount/WishList/MyWishListController.ts' />
///<reference path='./Composer.MyAccount/WishListShared/SharedWishListController.ts' />
///<reference path='./Composer.MyAccount/WishList/WishListInHeaderController.ts' />
///<reference path='./Composer.Cart/CheckoutShippingAddressRegistered/ShippingAddressRegisteredController.ts' />
///<reference path='./Composer.Cart/CheckoutOrderSummary/CheckoutOrderSummaryController.ts' />
///<reference path='./Composer.Cart/CheckoutOrderSummary/CompleteCheckoutOrderSummaryController.ts' />
///<reference path='./Composer.Cart/CheckoutPayment/CheckoutPaymentController.ts' />
///<reference path='./Composer.Cart/CheckoutCommon/CheckoutNavigationController.ts' />
///<reference path='./Composer.MyAccount/SignInHeader/SignInHeaderController.ts' />
///<reference path='./Composer.MyAccount/MyAccount/MyAccountController.ts' />
///<reference path='./Composer.Cart/OrderHistory/CurrentOrdersController.ts' />
///<reference path='./Composer.Cart/OrderHistory/PastOrdersController.ts' />
///<reference path='./Composer.Cart/OrderDetails/OrderDetailsController.ts' />
///<reference path='./Composer.Cart/FindMyOrder/FindMyOrderController.ts' />
///<reference path='./ErrorHandling/ErrorController.ts' />

///<reference path='./Composer.Store/StoreLocator/StoreLocatorController.ts' />
///<reference path='./Composer.Store/StoreLocator/StoreDetailsController.ts' />
///<reference path='./Composer.Store/StoreDirectory/StoresDirectoryController.ts' />
///<reference path='./Composer.Store/StoreInventory/StoreInventoryController.ts' />
///<reference path='./Composer.MyAccount/RecurringSchedule/MyRecurringScheduleController.ts' />
///<reference path='./Composer.MyAccount/RecurringSchedule/MyRecurringScheduleDetailsController.ts' />
///<reference path='./Composer.MyAccount/RecurringCart/MyRecurringCartsController.ts' />
///<reference path='./Composer.MyAccount/RecurringCart/MyRecurringCartDetailsController.ts' />

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
