///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/IControllerContext.ts' />
///<reference path='../../Mvc/ComposerClient.ts' />
///<reference path='../../Events/EventHub.ts' />
///<reference path='../RelatedProducts/ProductIdentifierDto.ts' />
///<reference path='./IProductService.ts' />

module Orckestra.Composer {
    'use strict';

    export class ProductService implements IProductService {

        private eventHub: IEventHub;
        private context: Orckestra.Composer.IControllerContext;

        constructor(eventHub: IEventHub, context: Orckestra.Composer.IControllerContext) {
            if (!eventHub) {
                throw new Error('Error: eventHub is required');
            }
            if (!context) {
                throw new Error('Error: context is required');
            }
            if (!context.viewModel) {
                throw new Error('Error: context.viewModel is required');
            }

            this.eventHub = eventHub;
            this.context = context;
        }

        public showQuickView() {
            $('#addToCartModal').modal('show');
        }

        public closeQuickView() {
            $('#addToCartModal').modal('hide');
        }

        public calculatePrice(productId: string, concern: string) {
            var data = { products: [productId] };

            return ComposerClient.post('/api/product/calculatePrices', data)
                .then((result) => {

                    var details = this.context.viewModel;
                    var mainResult = <any>_.find(result.ProductPrices, { ProductId: productId });

                    //TODO extend all other products on the page (if any)
                    //Extend the product details
                    _.extend(details, mainResult);

                    if (result && result.Currency) {
                       details.Currency = result.Currency;
                    }

                    //Extend the variants
                    var allVariants = this.context.viewModel.allVariants;
                    _.each(mainResult.VariantPrices, (variantPrice: any) => {
                        var variant = <any>_.find(allVariants, { Id: variantPrice.VariantId });
                        _.extend(variant, variantPrice);

                        if (variant !== undefined && variant.Id === details.displayedVariantId) {
                            _.extend(details, variant);
                        }
                    });

                    this.eventHub.publish(concern + 'PricesChanged', { data: details });
                    this.eventHub.publish(concern + 'PriceCalculated', { data: details });
                });
        }

        /*
         * Return the ViewModel of the Selected Variant.
         * If no variants are available,
         *    it returns the ViewModel of the Product
         * If no variant is selected (aka the KVA selection binds to an unavailable variant)
         *    it returns a None Buyable ViewModel
         */
        public getSelectedVariantViewModel() {
            var selectedVariantId  = this.context.viewModel.selectedVariantId;
            var displayedVariantId = this.context.viewModel.displayedVariantId;

            if (!displayedVariantId) {
                //This is mostlikely a product
                return this.context.viewModel;
            } else if (selectedVariantId === displayedVariantId) {
                //This is mostlikely a variant
                return this.getVariant(selectedVariantId);
            }

            return {
                'IsAvailableToSell': false
            };
        }

        // TODO (SAM) : getVariant and updateSelectedKvasWith shouldn't be in this file.
        //              They're more suited to a Controller or a helper class.

        //Get the Variant for the given id
        //<param name="variantId">The variant id to find</param>
        //<returns>KeyVariantAttributeItem ViewModel ready for render</returns>
        public getVariant(variantId: string): any {
            var allVariants = this.context.viewModel.allVariants;
            var variant = _.find(allVariants, {Id: variantId});

            return variant;
        }

        public updateSelectedKvasWith(selectionsToAdd: any, concern: string) {
            var allVariants = this.context.viewModel.allVariants;
            var initialSelectedKvas = this.context.viewModel.selectedKvas || {};
            var initialSelectedVariantId = this.context.viewModel.selectedVariantId;
            var initialDisplayedVariantId = this.context.viewModel.displayedVariantId;

            //Update current selection
            var selectedKvas = _.merge(initialSelectedKvas, selectionsToAdd);

            //Find possible matches
            var variants = _.filter(allVariants, { Kvas: selectedKvas });

            if (variants && variants.length === 1) {
                //Exactly one variant found, let's update the detail to display it.
                var variant = <any>variants[0];

                this.context.viewModel.selectedKvas = _.clone(variant.Kvas);
                this.context.viewModel.selectedVariantId  = variant.Id;
                this.context.viewModel.displayedVariantId = variant.Id;

            } else {
                //More than one possibile variants, let's not assume any selected ones.
                this.context.viewModel.selectedKvas = selectedKvas;
                this.context.viewModel.selectedVariantId = null;
            }

            //Superseed the product details with the selection variant details
            _.extend(this.context.viewModel, this.getSelectedVariantViewModel());

            this.buildKeyVariantAttributeItems(concern);

            if (initialDisplayedVariantId !== this.context.viewModel.displayedVariantId) {
                this.eventHub.publish(concern + 'DisplayedVariantIdChanged', {
                    data: {
                        initialDisplayedVariantId: initialDisplayedVariantId,
                        displayVariantId: this.context.viewModel.displayedVariantId,
                        selectedSku: this.context.viewModel.Sku
                    }
                });
            }

            if (initialSelectedVariantId !== this.context.viewModel.selectedVariantId) {
                this.eventHub.publish(concern + 'SelectedVariantIdChanged', {
                    data: {
                        initialSelectedVariantId: initialSelectedVariantId,
                        selectedVariantId: this.context.viewModel.selectedVariantId,
                        selectedSku: this.context.viewModel.Sku
                    }
                });
            }

            this.eventHub.publish(concern + 'ImagesChanged', { data: this.context.viewModel });
            //I think this publish is unnecessary because it calculate the price after on a API server call
            this.eventHub.publish(concern + 'PricesChanged', { data: this.context.viewModel });
        }

        public getRelatedProducts(relatedProductIdentifiers: ProductIdentifierDto[]): Q.Promise<any> {
            return ComposerClient.post('/api/product/relatedProducts', relatedProductIdentifiers);
        }

        public loadQuickBuyProduct(productId: string, variantId: string, concern: string, source: string): Q.Promise<any> {

            var data = {
                ProductId: productId,
                VariantId: variantId
            };

            return ComposerClient.post('/api/product/variantSelection', data)
                .then((quickBuyProductViewModel) => {
                    this.eventHub.publish(concern + 'QuickBuyLoaded', { data: quickBuyProductViewModel, source: source });
                    return quickBuyProductViewModel;
                }).fail((reason: any) => {
                    console.error('Failed loading the ProductQuickView', reason);
                    throw reason;
                });
        }

        public findInventoryItems(viewModel: any, concern: string) {
            var selectedSku: string  = viewModel.Sku,
                skus: string[];

            if (_.isEmpty(viewModel.allVariants)) {
                skus = [viewModel.Sku];
            } else {
                skus = _.pluck(viewModel.allVariants, 'Sku');
            }

            var data = { skus: skus };

            return ComposerClient.post('/api/inventory/findInventoryItems', data)
                .then((skusAvailableToSell) => {
                    var isAvailableToSell = _.include(skusAvailableToSell, selectedSku) && viewModel.IsAvailableToSell;
                    this.eventHub.publish(concern + 'InventoryRetrieved', { data: isAvailableToSell });
                });
        }

         public productAvailableToSell(selectedSku: string, productAvailableToSell: string[], productIsAvailableToSell: boolean) : boolean {
            return _.include(productAvailableToSell, selectedSku) && productIsAvailableToSell;
        }

        private buildKeyVariantAttributeItems(concern: string) {
            var selectedKvas = this.context.viewModel.selectedKvas;
            var builder = new Orckestra.Composer.KeyVariantAttributeItemsBuilder(this.context);
            var keyVariantAttributeItems = builder.BuildKeyVariantAttributeItemsFor(selectedKvas);

            this.eventHub.publish(concern + 'SelectedKvasChanged', { data: keyVariantAttributeItems });
        }

         public replaceHistory() {
            var variantId = this.context.viewModel.selectedVariantId;

            //Variant selection is not valid use last valid
            if (variantId === null) {
                return;
            }

            var fullPathArray = window.location.pathname.split('/').filter(Boolean);
            var shortPathArray = fullPathArray.slice(0, 3);
            shortPathArray.push(variantId);

            var builtPath = window.location.protocol
            + '//'
            + window.location.host
            + this.buildUrlPath(shortPathArray);

            history.replaceState( {} , null, builtPath);
        }

        public buildUrlPath(pathArray: string[]) : string {
            var newPathname = '';
            for (var i = 0; i < pathArray.length; i++) {
              newPathname += '/';
              newPathname += pathArray[i];
            }

            return newPathname;
        }
    }
}
