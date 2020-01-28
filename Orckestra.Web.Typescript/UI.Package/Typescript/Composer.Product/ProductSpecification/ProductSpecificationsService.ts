///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/ComposerClient.ts' />
///<reference path='../../Events/EventHub.ts' />

module Orckestra.Composer {
    'use strict';

    export class ProductSpecificationsService {

        private memoizeProductSpecifications: Function;

        public getProductSpecifications(productId: string, variantId: string): Q.Promise<any> {

            if (!productId) {
                throw new Error('The product id is required');
            }

            if (!this.memoizeProductSpecifications) {

                this.memoizeProductSpecifications =
                    _.memoize(this.getProductSpecificationsImpl, (productId, variantId) => variantId);
            }

            return this.memoizeProductSpecifications(productId, variantId);
        }

        private getProductSpecificationsImpl(productId: string, variantId: string): Q.Promise<any> {

            var data = { productId: productId, variantId: variantId };
            return ComposerClient.post('/api/product/specifications', data);
        }
    }
}
