/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='./StorageType.ts' />

module Orckestra.Composer {
    var localStoragePolyFill: any;
    var sessionStoragePolyFill: any;
    export class StoragePolyfill {

        static create(windowHandle: Window, storageType : StorageType): Storage {
            var lastStorage: any,
                lastStorageKey: string,
                storagePolyFill: any;

                if (storageType === void 0) {
                    throw {
                            name: 'StoragePolyfillException',
                            message: `A storage type must be specified in the storage polyfill create method.`
                        };
                }

            switch (storageType) {
                case StorageType.localStorage:
                    storagePolyFill = localStoragePolyFill;
                    break;
                case StorageType.sessionStorage:
                    storagePolyFill = sessionStoragePolyFill;
                    break;
            }

            if (storagePolyFill !== void 0) {
                return storagePolyFill;
            }

            function persist(storage: any) {
                windowHandle.name = JSON.stringify(storage);
            }

            function getStorageKey(key: string) {
                return key.concat(storageType.toString());
            }

            function getStorageApi() {
                return {
                    clear: function(): void {
                        var key: string;

                        Object.keys(storagePolyFill).forEach((key: string) => {
                            if (storagePolyFill.hasOwnProperty(key)) {
                                delete storagePolyFill[key];
                            }
                        });

                        persist(storagePolyFill);
                    },

                    getItem: function(key: string): any {
                        key = getStorageKey(key);
                        return storagePolyFill[key] || null;
                    },

                    key: function(index: number): string {
                        var keys: string[] = null;

                        keys = Object.keys(storagePolyFill).filter((value: string, indexToMatch: number) => indexToMatch === index);

                        return keys.length > 0 ? keys[0] : null;
                    },

                    removeItem: function(key: string): void {
                        key = getStorageKey(key);
                        if (key in storagePolyFill && storagePolyFill.hasOwnProperty(key)) {
                            delete storagePolyFill[key];
                            persist(storagePolyFill);
                        }
                    },

                    setItem(key: string, data: string): void {
                        key = getStorageKey(key);
                        storagePolyFill[key] = data;
                        persist(storagePolyFill);
                    },

                    dispose: function() {
                        persist(storagePolyFill);
                    }
                };
            }

            storagePolyFill = Object.create(getStorageApi());

            if (windowHandle.name !== '') {
                lastStorage = JSON.parse(windowHandle.name);

                for (lastStorageKey in lastStorage) {
                    if (lastStorage.hasOwnProperty(lastStorageKey)) {
                        storagePolyFill[lastStorageKey] = lastStorage[lastStorageKey];
                    }
                }
            }

            switch (storageType) {
                case StorageType.localStorage:
                    localStoragePolyFill = storagePolyFill;
                    return localStoragePolyFill;
                case StorageType.sessionStorage:
                    sessionStoragePolyFill = storagePolyFill;
                    return sessionStoragePolyFill;
            }
        }
    };
}

