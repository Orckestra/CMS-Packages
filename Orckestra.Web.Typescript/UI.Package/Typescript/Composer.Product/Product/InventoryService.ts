///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/ComposerClient.ts' />
///<reference path='./IInventoryService.ts' />

module Orckestra.Composer {
    'use strict';

    export class InventoryService implements IInventoryService {

        private _memoizeIsAvailableToSell: Function;

        public isAvailableToSell(sku: string): Q.Promise<boolean> {

            if (!sku) {
                throw new Error('The sku is required');
            }

            if (!this._memoizeIsAvailableToSell) {

                this._memoizeIsAvailableToSell = _.memoize(arg => this.isAvailableToSellImpl(arg));
            }

            return this._memoizeIsAvailableToSell(sku);
        }

        private isAvailableToSellImpl(sku: string): Q.Promise<boolean> {

            var data = { skus: [sku] };

            return ComposerClient.post('/api/inventory/findInventoryItems', data)
                                 .then(availableSkus => _.contains(availableSkus, sku));
        }
    }
}
