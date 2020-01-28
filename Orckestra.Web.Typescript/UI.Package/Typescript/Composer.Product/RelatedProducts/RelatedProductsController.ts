/// <reference path='../Product/ProductController.ts' />
///<reference path='../../Plugins/SlickCarouselPlugin.ts' />
module Orckestra.Composer {
    export class RelatedProductController extends Orckestra.Composer.ProductController  {

        protected concern: string = 'relatedProduct';
        private source: string = 'Related Products';

        private products: any[];

        public initialize() {

            super.initialize();
            this.getRelatedProducts();
        }

        private getRelatedProducts() {
            let vm = this.context.viewModel;
            let identifiers = vm.ProductIdentifiers;
            this.productService.getRelatedProducts(identifiers)
                .then(data => {
                    this.products = data.Products;
                    //Need to map parent items to child item since handlebar doesnt seem to support
                    //partial parameters or path properly while it suppose to. Maybe an update of handlebar could solve the issue
                    //(3.01 right now).
                    vm.Products = _.each(data.Products, (lineItem: any) => {
                        lineItem.DisplayAddToCart = vm.DisplayAddToCart;
                        lineItem.DisplayPrices = vm.DisplayPrices;
                    });

                    this.render('RelatedProducts', vm);
                    this.eventHub.publish('iniCarousel', vm);
                    return vm;
                })
                .then(vm => {
                    if (vm && vm.Products && vm.Products.length > 0) {
                        this.eventHub.publish('relatedProductsLoaded',
                            {
                                data: {
                                    ListName: this.getPageSource(),
                                    Products: vm.Products
                                }
                            });
                    }
                })
                .then((vm: any) => this.onGetRelatedProductsSuccess(vm),
                    (reason: any) => this.onGetRelatedProductsFailed(reason));
        }

        protected onGetRelatedProductsSuccess(vm: any): void {
            //Hook for other projects
        }

        protected onGetRelatedProductsFailed(reason: any): void {
            console.error('Failed loading the related products', reason);
        }

        protected getPageSource(): string {
            return 'Related Products';
        }

        protected getListNameForAnalytics(): string {
            return 'Related Products';
        }

        protected onLoadingFailed(reason: any) {
            console.error('Failed loading the Related Product View');

            ErrorHandler.instance().outputErrorFromCode('RelatedProductLoadFailed');
        }

         public addToCart(actionContext: IControllerActionContext) {
            var productContext: JQuery = $(actionContext.elementContext).closest('[data-product-id]');

            var hasVariants: boolean = productContext.data('hasVariants');
            var productId: string = productContext.attr('data-product-id');
            var variantId: string = productContext.attr('data-product-variant-id');
            var price: string = productContext.data('price');
            var recurringProgramName: string = productContext.data('recurringorderprogramname');

            var busy = this.asyncBusy({ elementContext: actionContext.elementContext, containerContext: productContext });
            var promise: Q.Promise<any>;

            if (hasVariants) {
                promise = this.addVariantProductToCart(productId, variantId, price);
            } else {
                promise = this.addNonVariantProductToCart(productId, price, recurringProgramName);
            }

            promise.fin(() => busy.done());
        }

        /**
         * Occurs when adding a product to the cart that happens to have variants.
         */
        protected addVariantProductToCart(productId: string, variantId: string, price: string): Q.Promise<any> {
            var promise = this.productService.loadQuickBuyProduct(productId, variantId, this.concern, this.source);
            promise.fail((reason: any) => this.onLoadingFailed(reason));

            return promise;
        }

        /**
         * Occurs when adding a product to the cart that has no variant.
         */
        protected addNonVariantProductToCart(productId: string, price: string, recurringProgramName: string): Q.Promise<any> {
            var vm = this.getProductViewModel(productId);
            if (vm) {
                var quantity = this.getCurrentQuantity();
                var data: any = this.getProductDataForAnalytics(vm);
                data.Quantity = quantity.Value ? quantity.Value : 1;

                this.eventHub.publish('lineItemAdding',
                {
                    data: data
                });
            }

            var promise = this.cartService.addLineItem(productId, price, null, 1, null, recurringProgramName)
                .then((vm: any) => this.onAddLineItemSuccess(vm),
                    (reason: any) => this.onAddLineItemFailed(reason));

            return promise;
        }

        protected getProductViewModel(productId: string): any {
            var productVM = _.find(this.products, p => {
                return p.ProductId === productId;
            });

            if (!productVM) {
                console.warn(`Could not find the product with ID of ${productId} within related products.
                    This will cause the product to not be reported to Analytics.`);
            }

            return productVM;
        }

        protected getCurrentQuantity(): any {
            return {
                Min: 1,
                Max: 1,
                Value: 1
            };
        }

        public relatedProductsClick(actionContext: IControllerActionContext) {

            var index: number = <any>actionContext.elementContext.data('index');
            var productId: string = actionContext.elementContext.data('productid').toString();
            var product: any = _.find(this.products, {ProductId : productId});

            this.eventHub.publish('productClick', { data : {
                Product : product,
                ListName : 'Related Products',
                Index : index
            }});
        }
    }
}
