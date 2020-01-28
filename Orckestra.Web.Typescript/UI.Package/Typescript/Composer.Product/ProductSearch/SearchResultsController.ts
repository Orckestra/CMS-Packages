/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../ErrorHandling/ErrorHandler.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
/// <reference path='../../Repositories/CartRepository.ts' />
/// <reference path='../../Composer.Cart/CartSummary/CartService.ts' />
/// <reference path='../Product/ProductService.ts' />

module Orckestra.Composer {
    'use strict';

    export class SearchResultsController extends Orckestra.Composer.Controller {
        protected cartService: CartService = new CartService(new CartRepository(), this.eventHub);
        protected productService: ProductService = new ProductService(this.eventHub, this.context);
        protected currentPage: any;

        public initialize() {

            super.initialize();

            this.currentPage = this.getCurrentPage();

            let pageDisplayName;
            if (!this.currentPage || _.isUndefined(this.currentPage) || this.currentPage === null) {
                pageDisplayName = '';
            } else {
                pageDisplayName = this.currentPage.DisplayName;
            }

            this.eventHub.publish('searchResultRendered', {
                data: {
                    ProductSearchResults: this.context.viewModel.SearchResults,
                    Keywords: this.context.viewModel.Keywords,
                    TotalCount: this.context.viewModel.TotalCount,
                    ListName: this.context.viewModel.ListName,
                    PageNumber: pageDisplayName,
                    MaxItemsPerPage: this.context.viewModel.MaxItemsPerPage
                }
            }
            );
        }

        private getCurrentPage(): any {

            return <any>this.context.viewModel.PaginationCurrentPage;
        }

        public addToCart(actionContext: IControllerActionContext) {

            var productContext: JQuery = $(actionContext.elementContext).closest('[data-product-id]');

            var hasVariants: string = <any>productContext.data('hasVariants');

            //Do not use .data since it may parse the id as a number.
            var productId: string = productContext.attr('data-product-id');
            var variantId: string = productContext.attr('data-product-variant-id');
            var recurringOrderProgramName: string = productContext.attr('data-recurring-order-program-name');

            var product = _.find(this.context.viewModel.SearchResults, function (product: any) {
                if (_.isEmpty(variantId)) {
                    return product.ProductId === productId;
                } else {
                    return product.ProductId === productId && product.VariantId === variantId;
                }
            });
            var price: number = product.Pricing.IsOnSale ? product.Pricing.Price : product.Pricing.ListPrice;

            var busy = this.asyncBusy({ elementContext: actionContext.elementContext, containerContext: productContext });

            if (hasVariants === 'True') {

                this.productService.loadQuickBuyProduct(productId, variantId, 'productSearch', this.context.viewModel.ListName)
                    .then((data: any) => {
                        ErrorHandler.instance().removeErrors();
                        return data;
                    }, (reason: any) => this.onAddToCartFailed(reason))
                    .fin(() => busy.done());

            } else {
                var productData: any = this.getProductDataForAnalytics(productId, price);
                this.eventHub.publish('lineItemAdding', { data: productData });

                this.cartService.addLineItem(productId, '' + price, null, 1, null, recurringOrderProgramName)
                    .then((data: any) => {
                        ErrorHandler.instance().removeErrors();
                        return data;
                    }, (reason: any) => this.onAddToCartFailed(reason))
                    .fin(() => busy.done());
            }
        }

        protected onAddToCartFailed(reason: any): void {
            console.error('Error on adding item to cart', reason);

            ErrorHandler.instance().outputErrorFromCode('AddToCartFailed');
        }

        public searchProductClick(actionContext: IControllerActionContext) {
            var index: number = <any>actionContext.elementContext.data('index');
            var productId: string = actionContext.elementContext.data('productid').toString();
            var product: any = _.find(this.context.viewModel.SearchResults, { ProductId: productId });

            this.eventHub.publish('productClick', {
                data: {
                    Product: product,
                    ListName: this.context.viewModel.ListName,
                    Index: index,
                    PageNumber: this.currentPage.DisplayName,
                    MaxItemsPerPage: this.context.viewModel.MaxItemsPerPage
                }
            });
        }

        public pagerPageChanged(actionContext: IControllerActionContext) {
            this.context.window.location.href = actionContext.elementContext.val();
        }

        protected getProductDataForAnalytics(productId: string, price: any): any {
            var results = this.context.viewModel.SearchResults;
            var vm = _.find(results, (r: any) => r.ProductId === productId);

            if (!vm) {
                throw new Error(`Could not find a product with the ID '${productId}'.`);
            }

            var data = {
                List: this.context.viewModel.ListName,
                ProductId: vm.ProductId,
                DisplayName: vm.DisplayName,
                ListPrice: price,
                Brand: vm.Brand,
                CategoryId: vm.CategoryId,
                Quantity: 1
            };

            return data;
        }
    }
}
