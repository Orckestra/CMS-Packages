///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='./GetStoresInventoryParam.ts' />

module Orckestra.Composer {
    export interface IStoreInventoryService {
        getStoresInventory(param: GetStoresInventoryParam): Q.Promise<any>;

        getDefaultAddress(): Q.Promise<any>;

        getSkuSelection(productId: string): Q.Promise<any>;
    }
}