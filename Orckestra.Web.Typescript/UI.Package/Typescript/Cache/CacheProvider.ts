/// <reference path='../../Typings/tsd.d.ts' />
///<reference path='./ICache.ts' />
///<reference path='./ICacheProvider.ts' />
///<reference path='./StorageBasedCache.ts' />
///<reference path='../Storage/IStorage.ts' />
///<reference path='../Storage/BackingStorage.ts' />
/// <reference path='../Storage/StorageFactory.ts' />
/// <reference path='../Storage/StorageType.ts' />

module Orckestra.Composer {

    export class CacheProvider implements ICacheProvider {

        private static defaultCacheKey : string = 'oc-cache';
        private static customCacheKey : string = 'composer-signInHeaderCache';
        private static _instance: Orckestra.Composer.ICacheProvider = new CacheProvider();

        public window: Window = window;
        public defaultCache: ICache;
        public customCache: ICache;
        public localStorage: Storage;
        public sessionStorage: Storage;

        public static instance(): Orckestra.Composer.ICacheProvider {
            return CacheProvider._instance;
        }

        constructor() {
            if (CacheProvider._instance) {
                throw new Error('Error: Instantiation failed: Use CacheProvider.instance() instead of new.');
            }

            this.defaultCache = this.getDefaultCache();
            this.customCache = this.getCustomCache();

            this.localStorage = this.getLocalStorage();
            this.sessionStorage = this.getSessionStorage();

            CacheProvider._instance = this;
        }

        public getCache(cacheKey: string) {
            var backingStorage = this.getLocalStorage();
            return new StorageBasedCache(new BackingStorage(backingStorage), cacheKey);
        }

        public getDefaultCache(): ICache {
            return this.getCache(CacheProvider.defaultCacheKey);
        }

        public getCustomCache(): ICache {
            return this.getCache(CacheProvider.customCacheKey);
        }

        private getLocalStorage(): Storage {
            return StorageFactory.create(StorageType.localStorage, window);
        }

        private getSessionStorage(): Storage {
            return StorageFactory.create(StorageType.sessionStorage, window);
        }
    }
}