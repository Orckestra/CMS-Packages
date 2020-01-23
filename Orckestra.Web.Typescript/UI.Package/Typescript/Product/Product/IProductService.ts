///<reference path='../RelatedProducts/ProductIdentifierDto.ts' />

module Orckestra.Composer {
    export interface IProductService {
        calculatePrice(productId: string, concern: string);

        getSelectedVariantViewModel();

        getVariant(variantId: string);

        updateSelectedKvasWith(selectionsToAdd: any, concern: string);

        getRelatedProducts(relatedProductIdentifiers: ProductIdentifierDto[]);

        loadQuickBuyProduct(productId: string, variantId: string, concern: string, source: string);

        findInventoryItems(viewModel: any, concern: string);

        buildUrlPath(pathArray: string[]): string;
    }
}
