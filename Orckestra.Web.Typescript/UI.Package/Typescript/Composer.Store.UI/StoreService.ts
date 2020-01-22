///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Composer.UI/IControllerContext.ts' />
///<reference path='../Composer.UI/ComposerClient.ts' />
///<reference path='./IStoreService.ts' />

module Orckestra.Composer {
    'use strict';

    export class StoreService implements IStoreService {
        private static _instance: StoreService = new StoreService();

        public static instance(): StoreService {
            return StoreService._instance;
        }

        public getStores() {
            return ComposerClient.get('/api/store/stores');
        }
    }
}
