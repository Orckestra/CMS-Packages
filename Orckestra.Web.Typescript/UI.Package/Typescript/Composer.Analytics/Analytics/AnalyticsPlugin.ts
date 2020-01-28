/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../Events/EventHub.ts' />
/// <reference path='./IAnalyticsPlugin.ts' />
/// <reference path='./IAnalyticsOrder.ts' />
/// <reference path='./IAnalyticsTransaction.ts' />
/// <reference path='./IAnalyticsSearchFilters.ts' />
/// <reference path='../../Plugins/IPlugin.ts' />
/// <reference path='../../Composer.MyAccount/Common/MyAccountEvents.ts' />

module Orckestra.Composer {
    export class AnalyticsPlugin implements IAnalyticsPlugin, IPlugin {
        protected cacheProvider: ICacheProvider;
        protected eventHub: IEventHub;
        protected useVariantIdWhenPossible: boolean;

        public initialize() {
            this.useVariantIdWhenPossible = true;
            this.eventHub = EventHub.instance();
            this.cacheProvider = CacheProvider.instance();
            this.registerSubscriptions();
        }

        public static setCheckoutOrigin(checkoutOrigin: string) {
            CacheProvider.instance().sessionStorage.setItem('analytics.checkoutOrigin', checkoutOrigin);
        }

        public static getCheckoutOrigin(): string {
            return CacheProvider.instance().sessionStorage.getItem('analytics.checkoutOrigin');
        }

        /**
         * Binds all the events for Analytics
         */
        public registerSubscriptions() {
            this.eventHub.subscribe('lineItemAdding', (eventInfo: IEventInformation) => {
                this.onLineItemAdding(eventInfo);
            });

            this.eventHub.subscribe('lineItemRemoving', (eventInfo: IEventInformation) => {
                this.onLineItemRemoving(eventInfo);
            });

            this.eventHub.subscribe('productDetailsRendered', (eventInfo: IEventInformation) => {
                this.onProductDetailsRendered(eventInfo);
            });

            this.eventHub.subscribe('checkoutStepRendered', (eventInfo: IEventInformation) => {
                this.onCheckoutStepRendered(eventInfo);
            });

            this.eventHub.subscribe('checkoutNavigationRendered', (eventInfo: IEventInformation) => {
                this.onCheckoutNavigationRendered(eventInfo);
            });

            this.eventHub.subscribe('CheckoutConfirmation', (eventInfo: IEventInformation) => {
                this.onCheckoutCompleted(eventInfo);
            });

            this.eventHub.subscribe('searchResultRendered', (eventInfo: IEventInformation) => {
                this.onSearchResultRendered(eventInfo);
            });

            this.eventHub.subscribe('relatedProductsLoaded', (eventInfo: IEventInformation) => {
                this.onRelatedProductsLoaded(eventInfo);
            });

            this.eventHub.subscribe('productClick', (eventInfo: IEventInformation) => {
                this.onProductClick(eventInfo);
            });

            this.eventHub.subscribe('pageNotFound', (eventInfo: IEventInformation) => {
                this.onPageNotFound(eventInfo);
            });

            this.eventHub.subscribe('wishListLineItemAdding', (eventInfo: IEventInformation) => {
                this.onWishListLineItemAdding(eventInfo);
            });

            this.eventHub.subscribe('wishListLineItemAddingToCart', (eventInfo: IEventInformation) => {
                this.onLineItemAdding(eventInfo);
            });

            this.eventHub.subscribe('wishListCopyingShareUrl', (eventInfo: IEventInformation) => {
                this.onWishListCopingShareUrl(eventInfo);
            });

            this.eventHub.subscribe(MyAccountEvents[MyAccountEvents.LoggedIn], (eventInfo: IEventInformation) => {
                this.onUserLoggedIn(eventInfo);
            });

            this.eventHub.subscribe(MyAccountEvents[MyAccountEvents.AccountCreated], (eventInfo: IEventInformation) => {
                this.onUserCreated(eventInfo);
            });

            this.eventHub.subscribe(MyAccountEvents[MyAccountEvents.ForgotPasswordInstructionSent], (eventInfo: IEventInformation) => {
                this.onRecoverPassword(eventInfo);
            });

            this.eventHub.subscribe('noResultsFound', (eventInfo: IEventInformation) => {
                this.onNoResultsFound(eventInfo);
            });

            this.eventHub.subscribe('searchTermCorrected', (eventInfo: IEventInformation) => {
                this.onSearchTermCorrected(eventInfo);
            });

            this.eventHub.subscribe('singleCategoryAdded', (eventInfo: IEventInformation) => {
                this.onSingleFacetChanged(eventInfo);
            });

            this.eventHub.subscribe('singleFacetsChanged', (eventInfo: IEventInformation) => {
                this.onSingleFacetChanged(eventInfo);
            });

            this.eventHub.subscribe('multiFacetChanged', (eventInfo: IEventInformation) => {
                this.onMultiFacetChanged(eventInfo);
            });

            this.eventHub.subscribe('sortingChanged', (eventInfo: IEventInformation) => {
                this.onSortingChanged(eventInfo);
            });
        }

        /**
         * Occurs when a user log in
         */
        protected onUserLoggedIn(eventInfo: IEventInformation): void {
            if (!eventInfo) {
                return;
            }

            var data = eventInfo.data;

            this.userLoggedIn('regular', data.ReturnUrl);
        }

        /**
         * Occurs when a user creates an account
         */
        protected onUserCreated(eventInfo: IEventInformation): void {
            if (!eventInfo) {
                return;
            }

            this.userCreated();
        }

        /**
         * Occurs when a user log in
         */
        protected onRecoverPassword(eventInfo: IEventInformation): void {
            if (!eventInfo) {
                return;
            }

            this.recoverPassword();
        }

        protected onSingleFacetChanged(eventInfo: IEventInformation): void {
            if (!eventInfo) {
                return;
            }

            const data = eventInfo.data;
            let facetKey = data.facetKey;
            if (_.isString(data.facetKey)) {
                facetKey = data.facetKey.replace('[]', '').toLowerCase();
                if (facetKey.indexOf('category') !== -1) {
                    facetKey = 'category';
                }
            };

            let searchFilters: IAnalyticsSearchFilters = {
                facetKey: facetKey,
                facetValue: data.facetValue,
                pageType: data.pageType
            };

            this.singleFacetChanged(searchFilters);
        }

        protected onMultiFacetChanged(eventInfo: IEventInformation): void {
            if (!eventInfo) {
                return;
            }

            var data = eventInfo.data;

            let facetKey = _.isString(data.facetKey) ? data.facetKey.replace('[]', '').toLowerCase() : data.facetKey;

            var searchFilters: IAnalyticsSearchFilters = {
                facetKey: facetKey,
                facetValue: data.facetValue,
                pageType: data.pageType
            };

            this.multiFacetChanged(searchFilters);
        }

        protected onSortingChanged(eventInfo: IEventInformation): void {
            if (!eventInfo) {
                return;
            }

            var data = eventInfo.data;

            this.sortingChanged(data.sortingType, data.pageType);
        }

        /**
         * Occurs when a 404 page loads.
         */
        protected onPageNotFound(eventInfo: IEventInformation): void {
            if (!eventInfo) {
                return;
            }

            var data = eventInfo.data;

            this.sendEvent('event', '404 Errors', data.PageUrl, data.ReferrerUrl);
        }

        /**
        * Occurs when a user click on 'Copy Share Url' button
        */
        protected onWishListCopingShareUrl(eventInfo: IEventInformation): void {
            if (!eventInfo) {
                return;
            }
            var data = eventInfo.data;

            this.shareWishList(data);
        }

        /**
         * Occurs when a Line Item is being added to the Wish List.
         */
        protected onWishListLineItemAdding(eventInfo: IEventInformation): void {
            if (!eventInfo) {
                return;
            }

            var data = eventInfo.data;
            var analyticsProduct: IAnalyticsProduct = {
                name: data.DisplayName,
                price: this.trimPrice(data.ListPrice)
            };

            this.addToWishList(analyticsProduct);
        }


        /**
         * Occurs when a Line Item is being added.
         */
        protected onLineItemAdding(eventInfo: IEventInformation): void {
            if (!eventInfo) {
                return;
            }

            var data = eventInfo.data;
            var analyticsProduct: IAnalyticsProduct = {
                name: data.DisplayName,
                id: data.ProductId,
                variant: data.Variant,
                price: this.trimPriceAndUnlocalize(data.ListPrice),
                quantity: data.Quantity,
                category: data.CategoryId,
                brand: data.Brand,
                list: data.List
            };

            this.addToCart(analyticsProduct, data.List);
        }

        /**
         * Occurs when a Line item is being removed.
         */
        protected onLineItemRemoving(eventInfo: IEventInformation): void {
            if (!eventInfo) {
                return;
            }

            var data = eventInfo.data;
            var analyticsProduct: IAnalyticsProduct = {
                name: data.DisplayName,
                id: data.ProductId,
                variant: data.Variant,
                price: this.trimPriceAndUnlocalize(data.ListPrice),
                quantity: data.Quantity,
                category: data.CategoryId,
                brand: data.Brand
            };

            this.removeFromCart(analyticsProduct, data.List);
        }

        /**
         * Occurs when a product Details is rendered.
         */
        protected onProductDetailsRendered(eventInfo: IEventInformation): void {
            if (!eventInfo) {
                return;
            }

            var data = eventInfo.data;
            var analyticsProducts: IAnalyticsProduct[] = [];

            analyticsProducts.push({
                name: data.DisplayName,
                id: data.ProductId,
                price: this.trimPriceAndUnlocalize(data.ListPrice),
                brand: data.Brand,
                category: data.CategoryId,
                variant: data.Variant
            });

            this.productDetailImpressions(analyticsProducts, 'Detail');
        }

        /**
         *  Occurs when a Checkout Step is rendered.
         */
        protected onCheckoutStepRendered(eventInfo: IEventInformation): void {
            if (!eventInfo) {
                return;
            }

            var data = eventInfo.data;

            var analyticsProducts: IAnalyticsProduct[] = [];

            if (data.Cart) {
                Q.resolve(data.Cart).then(cart => {
                    _.each(cart.LineItemDetailViewModels, (lineItemDetailViewModel: any) => {
                        var analyticsProduct: IAnalyticsProduct = {
                            name: lineItemDetailViewModel.ProductSummary.DisplayName,
                            id: lineItemDetailViewModel.ProductId,
                            price: this.trimPriceAndUnlocalize(lineItemDetailViewModel.ListPrice),
                            quantity: lineItemDetailViewModel.Quantity,
                            category: lineItemDetailViewModel.ProductSummary.CategoryId.replace(/-/g, '/'),
                            variant: this.buildVariantForLineItem(lineItemDetailViewModel),
                            brand: lineItemDetailViewModel.ProductSummary.Brand
                        };

                        if (this.useVariantIdWhenPossible && lineItemDetailViewModel.VariantId) {
                            analyticsProduct.id = lineItemDetailViewModel.VariantId;
                        }

                        analyticsProducts.push(analyticsProduct);
                    });

                    let checkoutOrigin = AnalyticsPlugin.getCheckoutOrigin();
                    let transaction: IAnalyticsTransaction = {
                        checkoutOrigin: checkoutOrigin
                    };

                    this.checkout(data.StepNumber, transaction, analyticsProducts);
                });
            }
        }

        /**
         * Occurs when a the Checkout Navigation strip is rendered.
         */
        protected onCheckoutNavigationRendered(eventInfo: IEventInformation): void {
            if (!eventInfo) {
                return;
            }

            var data = eventInfo.data;

            this.checkoutOption(data.StepNumber);
        }

        /**
         * Occurs when a the Checkout completes, creating an order out of a Cart.
         */
        protected onCheckoutCompleted(eventInfo: IEventInformation): void {
            if (!eventInfo) {
                return;
            }

            var data = eventInfo.data;

            var order: IAnalyticsOrder = {
                id: data.OrderNumber,
                affiliation: data.Affiliation,
                revenue: data.Revenu,
                tax: data.Tax,
                shipping: data.Shipping,
                coupon: _.isEmpty(data.Coupons) ? '' : data.Coupons.map(c => c.CouponCode).join(', '),
                currencyCode: data.BillingCurrency
            };

            var transaction: IAnalyticsTransaction = this.mapAnalyticTransactionFromOrder(data);

            var products: IAnalyticsProduct[] = this.mapAnalyticProductsFromLineItems(data);

            var coupons: IAnalyticsCoupon[] = this.mapAnalyticCouponsFromOrder(data);

            _.each(coupons, (coupon: IAnalyticsCoupon) => {
                this.couponsUsed(coupon);
            });

            this.purchase(order, transaction, products);
        }

        /**
         * Occurs when the Search results on a page are rendered.
         */
        protected onSearchResultRendered(eventInfo: IEventInformation): void {
            if (!eventInfo) {
                return;
            }

            var data = eventInfo.data;

            if (data.ProductSearchResults.length > 0) {
                var products: IAnalyticsProduct[] = [];

                _.each(data.ProductSearchResults, (product: any, i: number) => {
                    var analyticsProduct: IAnalyticsProduct = {
                        id: product.ProductId,
                        name: product.DisplayName,
                        price: this.trimPriceAndUnlocalize(product.Pricing.IsOnSale ? product.Pricing.Price : product.Pricing.ListPrice),
                        brand: product.Brand,
                        category: product.CategoryId,
                        list: data.ListName,
                        position: (i + 1) + (data.MaxItemsPerPage * (data.PageNumber - 1))
                    };

                    products.push(analyticsProduct);
                });

                this.productImpressions(products);
            }

            this.sendEvent('event', 'Search Results', 'Rendered', data.Keywords, data.TotalCount);
        }

        /**
         * Occurs when Related Products are loaded.
         */
        protected onRelatedProductsLoaded(eventInfo: IEventInformation): void {
            if (!eventInfo) {
                return;
            }

            var data = eventInfo.data;

            var products: IAnalyticsProduct[] = [];

            _.each(data.Products, (product: any, i: number) => {
                var analyticsProduct: IAnalyticsProduct = {
                    id: product.ProductId,
                    name: product.DisplayName,
                    price: this.trimPriceAndUnlocalize(product.Price),
                    brand: product.Brand,
                    list: data.ListName,
                    category: product.CategoryId,
                    position: i + 1
                };

                products.push(analyticsProduct);
            });

            this.productImpressions(products);
        }

        /**
         * Occurs when the user clicks on a product.
         */
        protected onProductClick(eventInfo: IEventInformation): void {
            if (!eventInfo) {
                return;
            }

            var data = eventInfo.data;

            var position: number = data.Index + 1;

            if (data.MaxItemsPerPage && data.PageNumber) {
                position = position + (data.MaxItemsPerPage * (parseInt(data.PageNumber, 10) - 1));
            }

            var product: IAnalyticsProduct = {
                id: data.Product.ProductId,
                name: data.Product.DisplayName,
                price: this.trimPriceAndUnlocalize(data.Product.Price || data.Product.Pricing.Price),
                brand: data.Product.Brand,
                category: data.Product.CategoryId,
                position: position
            };

            var products: IAnalyticsProduct[] = [product];
            this.productClick(products, data.ListName);
        }

        /**
         * Occurs when no results were found for a search.
         */
        protected onNoResultsFound(eventInfo: IEventInformation): void {
            if (!eventInfo) {
                return;
            }

            var data = eventInfo.data;

            this.noResultsFound(data.Keyword);
        }

        /**
         * Occurs when a search term has been auto corrected during a search.
         */
        protected onSearchTermCorrected(eventInfo: IEventInformation): void {
            if (!eventInfo) {
                return;
            }

            const data = eventInfo.data;

            const searchResults: IAnalyticsSearchResults = {
                keywordCorrected: data.KeywordCorrected,
                keywordEntered: data.KeywordEntered
            };

            this.searchKeywordCorrection(searchResults);
        }

        protected buildVariantForLineItem(lineItem: any): string {
            if (lineItem.VariantId && lineItem.KeyVariantAttributesList) {
                return this.buildVariantName(lineItem.KeyVariantAttributesList);
            }

            return undefined;
        }

        protected buildVariantName(kvas: any[]): string {
            var nameParts: string[] = [];

            for (var i: number = 0; i < kvas.length; i++) {
                var value: any = kvas[i].OriginalValue;

                nameParts.push(value);
            }

            return nameParts.join(' ');
        }

        protected mapAnalyticProductsFromLineItems(data: any): IAnalyticsProduct[] {

            var products: IAnalyticsProduct[] = [];

            products = _.map(data.LineItems, (lineItem: any) => {
                var analyticsProduct: IAnalyticsProduct = {
                    id: lineItem.ProductId,
                    name: lineItem.Name,
                    price: lineItem.Price,
                    variant: this.buildVariantForLineItem(lineItem),
                    quantity: lineItem.Quantity,
                    category: lineItem.CategoryId,
                    brand: lineItem.Brand
                };

                if (this.useVariantIdWhenPossible && lineItem.VariantId) {
                    analyticsProduct.id = lineItem.VariantId;
                }

                return analyticsProduct;
            });

            return products;
        }

        protected mapAnalyticCouponsFromOrder(data: any): IAnalyticsCoupon[] {
            var coupons: IAnalyticsCoupon[] = [];
            var billingCurrency = data.BillingCurrency;
            coupons = _.map(data.Coupons, (coupon: any) => {
                var analyticsCoupon: IAnalyticsCoupon = {
                    code: coupon.CouponCode,
                    discountAmount: coupon.Amount,
                    currencyCode: billingCurrency,
                    promotionName: coupon.PromotionName
                };
                return  analyticsCoupon;
            });

            return coupons;
        }

        protected mapAnalyticTransactionFromOrder(data: any): IAnalyticsTransaction {
            let checkoutOrigin = AnalyticsPlugin.getCheckoutOrigin();
            let analyticsTransaction: IAnalyticsTransaction = {
                shippingType: data.ShippingOptions,
                checkoutOrigin : checkoutOrigin
            };

            return analyticsTransaction;
        }

        //Abstract methods
        public userLoggedIn(type: string, source: string) {
            console.error('Not implemented Exception');
        }

        public userCreated() {
            console.error('Not implemented Exception');
        }

        public recoverPassword() {
            console.error('Not implemented Exception');
        }

        public singleFacetChanged(searchFilters: IAnalyticsSearchFilters) {
            console.error('Not implemented Exception');
        }

        public multiFacetChanged(searchFilters: IAnalyticsSearchFilters) {
            console.error('Not implemented Exception');
        }

        public sortingChanged(sortingType: string, pageType: string) {
            console.error('Not implemented Exception');
        }

        public productImpressions(products: IAnalyticsProduct[]) {
            console.error('Not implemented Exception');
        }

        public productClick(product: IAnalyticsProduct, listName: string) {
            console.error('Not implemented Exception');
        }

        public productDetailImpressions(products: IAnalyticsProduct[], listName: string) {
            console.error('Not implemented Exception');
        }

        public addToCart(product: IAnalyticsProduct, listName: string) {
            console.error('Not implemented Exception');
        }

        public addToWishList(product: IAnalyticsProduct) {
            console.error('Not implemented Exception');
        }

        public removeFromCart(product: IAnalyticsProduct, listName: string) {
            console.error('Not implemented Exception');
        }

        public checkout(step: number, transaction: IAnalyticsTransaction, products: IAnalyticsProduct[]) {
            console.error('Not implemented Exception');
        }

        public checkoutOption(step: number) {
            console.error('Not implemented Exception');
        }

        public purchase(order: IAnalyticsOrder, transaction: IAnalyticsTransaction, products: IAnalyticsProduct[]) {
            console.error('Not implemented Exception');
        }

        public couponsUsed(order: IAnalyticsCoupon) {
            console.error('Not implemented Exception');
        }

        public shareWishList(data: any) {
            console.error('Not implemented Exception');
        }

        public searchKeywordCorrection(data: IAnalyticsSearchResults) {
            console.error('Not implemented Exception');
        }

        public noResultsFound(keywordNotFound: string) {
            console.error('Not implemented Exception');
        }

        /**
         * Send a custom event to the analytics providers with multiples informations concerning the event
         * https://support.google.com/analytics/answer/1033068?hl=en
         * @param {string} eventName - The name of the custom event to send (e.g. productClick)
         * @param {string} category - The name that you supply as a way to group objects that you want to track (e.g. Product)
         * @param {string} action - The name the type of event or interaction you want to track for a particular web object (e.g. Click)
         * @param {string} label - Provide additional information for events that you want to track (e.g. Url of the clicked product)
         * @param {number} value - Use it to assign a numerical value to a tracked page object (e.g. Price of the product)
         */
        public sendEvent(eventName: string, category: string, action: string, label?: string, value?: number) {
            console.error('Not implemented Exception');
        }

        public trimPrice(price) {
            if (typeof price === 'number') {
                return price;
            }

            return price ? parseFloat(price.match(/[\d\.\d]+/i)[0]) : null;
        }

        public trimPriceAndUnlocalize(price) {
            if (!price || typeof price === 'number') {
                return price;
            }

            // remove anything that is not a digit, '.' or ','
            price = price.replace(/[^0-9,.]/, '');

            // if price contains a '.' its an English price and you can remove any ','
            if (price.indexOf('.') !== -1) {
                price = price.replace(',', '');
            } else {
                price = price.replace(',', '.');
            }

            return price ? parseFloat(price.match(/[\d\.\d]+/i)[0]) : null;
        }

        public formatDate(date) {
            var d = new Date(date),
                month = '' + (d.getMonth() + 1),
                day = '' + d.getDate(),
                year = d.getFullYear();

            if (month.length < 2) {
                month = '0' + month;
            }
            if (day.length < 2) {
                day = '0' + day;
            }

            return [year, month, day].join('-');
        }
    }
}
