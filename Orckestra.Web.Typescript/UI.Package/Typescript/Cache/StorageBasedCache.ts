/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../Storage/IStorage.ts' />
/// <reference path='../Storage/IStorageItem.ts' />
/// <reference path='./CacheError.ts' />
/// <reference path='./ICache.ts' />
/// <reference path='./ICacheItem.ts' />
/// <reference path='./ICachePolicy.ts' />

module Orckestra.Composer {

    var NoExpirationPolicy: ICachePolicy = { };

    /**
     * This cache uses a client-side storage engine to provide object caching
     */
    export class StorageBasedCache implements Orckestra.Composer.ICache {

        private _storageInitializing: Q.Promise<void>;
        private _storage: IStorage;
        private _type: string;

        constructor(storage: IStorage, type: string) {

            if (!storage) {
                throw new Error('The storage is required');
            }

            if (!type) {
                throw new Error('The type is required');
            }

            this._type = type;
            this._storage = storage;
            this._storageInitializing =
                storage.init().then(() => storage.initObjectStore(this._type));
        }

        public get<T>(key: string): Q.Promise<T> {

            if (!key) {
                throw new Error('The key is required');
            }

            return this._storageInitializing
                       .then(() => this._storage.get<ICacheItem<T>>(this._type, key))
                       .then(cacheItem => this.validate(key, cacheItem).then(() => cacheItem.value));
        }

        private validate<T>(key: string, item: ICacheItem<T>): Q.Promise<void> {

            return Q.Promise<void>((resolve, reject) => {

                if (_.isNull(item) || _.isNull(item.value)) {

                    reject(CacheError.NotFound);

                } else if (this.isExpired(item)) {

                    this._storageInitializing
                        .then(() => this._storage.remove(this._type, key))
                        .done(() => reject(CacheError.Expired), reason => reject(reason));

                } else {

                    item.lastAccessed = new Date().getTime();

                    var storageItem: IStorageItem<ICacheItem<T>> = {
                        id: key,
                        value: item
                    };

                    this._storageInitializing
                        .then(() => this._storage.set(this._type, storageItem))
                        .done(() => resolve(void 0), reason => reject(reason));
                }
            });
        }

        private isExpired<T>(item: ICacheItem<T>): boolean {

            var expirationTime: number, now: number = new Date().getTime();

            if (item.policy && item.policy.absoluteExpiration) {
                expirationTime = item.policy.absoluteExpiration;
            } else if (item.policy && item.policy.slidingExpiration) {
                expirationTime = item.lastAccessed + (item.policy.slidingExpiration * 1000);
            } else {
                return false;
            }

            return expirationTime < now;
        }

        public set<T>(key: string, value: T, policy?: ICachePolicy, type?: string): Q.Promise<T> {

            if (!key) {
                throw new Error('The key is required');
            }

            var typeItem = !type ? this._type : type;

            var cacheItem: ICacheItem<T> = {
                value: value,
                policy: !policy ? NoExpirationPolicy : policy,
                lastAccessed: new Date().getTime()
            };

            var storageItem: IStorageItem<ICacheItem<T>> = {
                id: key,
                value: cacheItem
            };

            return this._storageInitializing
                       .then(() => this._storage.set(typeItem, storageItem))
                       .then(() => value);
        }

        public clear(key: string): Q.Promise<void> {

            if (!key) {
                throw new Error('The key is required');
            }

            return this._storageInitializing
                .then(() => this._storage.remove(this._type, key));
        }

        public fullClear(): Q.Promise<void> {

            return this._storageInitializing
                .then(() => this._storage.fullRemove(this._type));
        }
    }
}
