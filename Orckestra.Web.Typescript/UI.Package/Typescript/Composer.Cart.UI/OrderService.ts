///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Composer.UI/ComposerClient.ts' />
///<reference path='./IGetOrderParameters.ts' />

module Orckestra.Composer {
    'use strict';

    export class OrderService {
        public getPastOrders(options: IGetOrderParameters = { page: 1 }) {
            return ComposerClient.post('/api/order/past-orders', options);
        }

        public getCurrentOrders(options: IGetOrderParameters = { page: 1 }) {
            return ComposerClient.post('/api/order/current-orders', options);
        }
    }
}
