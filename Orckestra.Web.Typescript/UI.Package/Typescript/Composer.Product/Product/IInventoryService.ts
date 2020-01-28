///<reference path='../RelatedProducts/ProductIdentifierDto.ts' />

module Orckestra.Composer {

    export interface IInventoryService {

        isAvailableToSell(sku: string): Q.Promise<boolean>;
    }
}
