///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../../Typescript/Cache/CacheError.ts' />
///<reference path='../../Typescript/Cache/StorageBasedCache.ts' />
///<reference path='../../Typescript/Storage/IStorage.ts' />
///<reference path='../../Typescript/Storage/BackingStorage.ts' />
///<reference path='../../Typescript/Storage/StorageFactory.ts' />


module Orckestra.Composer {
    'use strict';

    (() => {

        describe('WHEN the storage is null', () => {

            var cache: StorageBasedCache;

            it('SHOULD throw an error when creating the cache', () => {
                expect(() => cache = new StorageBasedCache(null, 'oc-cache')).toThrowError(Error);
            });
        });

        describe('WHEN the storage is undefined', () => {

            var cache: StorageBasedCache;

            it('SHOULD throw an error when creating the cache', () => {
                expect(() => cache = new StorageBasedCache(undefined, 'oc-cache')).toThrowError(Error);
            });
        });

        describe('WHEN the type is null', () => {

            var cache: StorageBasedCache;

            var storage = StorageFactory.create(StorageType.localStorage, <any>{ name: ''});


            it('SHOULD throw an error when creating the cache', () => {
                expect(() => cache = new StorageBasedCache(new BackingStorage(storage), null)).toThrowError(Error);
            });
        });

        describe('WHEN the type is undefined', () => {

            var cache: StorageBasedCache;
            var storage = StorageFactory.create(StorageType.localStorage, <any>{ name: ''});

            it('SHOULD throw an error when creating the cache', () => {
                expect(() => cache = new StorageBasedCache(new BackingStorage(storage), undefined)).toThrowError(Error);
            });
        });

        describe('WHEN getting an item from the cache', () => {

            var cache: StorageBasedCache,
                storage: Storage;

            beforeEach(() => {
                storage = StorageFactory.create(StorageType.localStorage, <any>{ name: ''});
                cache = new StorageBasedCache(new BackingStorage(storage), 'oc-cache');
            });

            it('SHOULD throw an error when the key is null', () => {
                expect(() => cache.get(null)).toThrowError(Error);
            });

            it('SHOULD throw an error when the key is undefined', () => {
                expect(() => cache.get(undefined)).toThrowError(Error);
            });
        });

        describe('WHEN getting an item from the cache', () => {

            var spy: SinonSpy;

            beforeEach(done => {
                var storage = StorageFactory.create(StorageType.localStorage, <any>{ name: ''});
                var cacheStorage: IStorage = new BackingStorage(storage);
                spy = sinon.spy(cacheStorage, 'init');
                new StorageBasedCache(cacheStorage, 'oc-cache').get('any').then(() => done(), () => done());
            });

            afterEach(() => {
                spy.restore();
            });

            it('SHOULD initialize the storage once', () => {
                expect(spy.calledOnce).toBeTruthy();
            });
        });

        describe('WHEN getting an item from the cache', () => {

            var spy: SinonSpy;

            beforeEach(done => {
                var storage = StorageFactory.create(StorageType.localStorage, <any>{ name: ''});
                var cacheStorage: IStorage = new BackingStorage(storage);

                spy = sinon.spy(cacheStorage, 'initObjectStore');
                new StorageBasedCache(cacheStorage, 'oc-cache').get('any').then(() => done(), () => done());
            });

            afterEach(() => {
                spy.restore();
            });

            it('SHOULD initialize the object store once', () => {
                expect(spy.calledOnce).toBeTruthy();
            });
        });

        function initObjectStore(type: string): Q.Promise<void> {

            return Q.fcall(() => {
                return;
            });
        }

        function nullValueCacheItem(type: string, key: string): Q.Promise<ICacheItem<any>> {

            return Q.fcall<ICacheItem<any>>(() => null);
        }

        function expiredCacheItem(type: string, key: string): Q.Promise<ICacheItem<any>> {

            var now: Date = new Date();

            var policy: ICachePolicy = {
                absoluteExpiration: now.setSeconds(now.getSeconds() - 1)
            };

            var expired: ICacheItem<any> = {
                value: null,
                policy: policy,
                lastAccessed: now.getTime()
            };

            return Q.fcall<ICacheItem<any>>(() => expired);
        }

    })();
}
