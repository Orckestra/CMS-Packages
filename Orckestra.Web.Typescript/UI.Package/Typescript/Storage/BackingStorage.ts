/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='./IStorage.ts' />
/// <reference path='./IStorageItem.ts' />

module Orckestra.Composer {

    export class BackingStorage implements Orckestra.Composer.IStorage {

        private _isInitialized: boolean = false;
        private _initializedObjectStores = {};

        constructor(private _storage: Storage) {
        }

        public init(): Q.Promise<void> {

            return Q.fcall(() => {
                this._isInitialized = true;
            });
        }

        public initObjectStore(type: string): Q.Promise<void> {

            if (!type) {
                throw new Error('The type is required');
            }

            return Q.fcall(() => {

                if (!this._isInitialized) {
                    throw new Error('The local storage has not been initialized');
                }

                this.initObjectStoreImpl(type);
            });
        }

        private initObjectStoreImpl(type: string): void {

            var objectStore: any = this.getObjectStore(type);

            if (!objectStore) {

                objectStore = {};
                this.setObjectStore(type, objectStore);
            }

            this._initializedObjectStores[type] = true;
        }

        public get<T>(type: string, id: string): Q.Promise<T> {

            if (!type) {
                throw new Error('The type is required');
            }
            if (!id) {
                throw new Error('The id is required');
            }

            return Q.fcall<T>(() => {

                if (!this._isInitialized) {
                    throw new Error('The local storage has not been initialized');
                }
                if (!this._initializedObjectStores[type]) {
                    throw new Error('The object store ' + type + ' has not been initialized');
                }

                return this.getImpl<T>(type, id);
            });
        }

        private getImpl<T>(type: string, id: string): T {

            var objectStore: any = this.getObjectStore(type);

            if (objectStore.hasOwnProperty(id)) {
                return objectStore[id];
            }

            return null;
        }

        public remove(type: string, id: string): Q.Promise<void> {

            if (!type) {
                throw new Error('The type is required');
            }
            if (!id) {
                throw new Error('The id is required');
            }

            return Q.fcall<void>(() => {

                if (!this._isInitialized) {
                    throw new Error('The local storage has not been initialized');
                }
                if (!this._initializedObjectStores[type]) {
                    throw new Error('The object store ' + type + ' has not been initialized');
                }

                this.removeImpl(type, id);
            });
        }

        public fullRemove(type: string): Q.Promise<void> {

            if (!type) {
                throw new Error('The type is required');
            }

            return Q.fcall<void>(() => {

                if (!this._isInitialized) {
                    throw new Error('The local storage has not been initialized');
                }
                if (!this._initializedObjectStores[type]) {
                    throw new Error('The object store ' + type + ' has not been initialized');
                }

                this.fullRemoveImpl(type);
            });
        }

        private removeImpl(type: string, id: string): void {

            var objectStore: any = this.getObjectStore(type);

            if (objectStore.hasOwnProperty(id)) {

                delete objectStore[id];
                this.setObjectStore(type, objectStore);
            }
        }

        private fullRemoveImpl(type: string): void {

            this.setObjectStore(type, null);
        }

        public set<T>(type: string, item: IStorageItem<T>): Q.Promise<void> {

            if (!type) {
                throw new Error('The type is required');
            }
            if (!item) {
                throw new Error('The item is required');
            }
            if (!item.id) {
                throw new Error('The item id is required');
            }

            return Q.fcall<void>(() => {

                if (!this._isInitialized) {
                    throw new Error('The local storage has not been initialized');
                }
                if (!this._initializedObjectStores[type]) {
                    throw new Error('The object store ' + type + ' has not been initialized');
                }

                this.setImpl<T>(type, item);
            });
        }

        private setImpl<T>(type: string, item: IStorageItem<T>): void {

            var objectStore: any = this.getObjectStore(type);
            objectStore[item.id] = item.value;

            this.setObjectStore(type, objectStore);
        }

        private getObjectStore(type: string): any {

            var objectStoreString: string = this._storage.getItem(type);
            var objectStore: any = JSON.parse(objectStoreString);

            return objectStore;
        }

        private setObjectStore(type: string, objectStore: any) {

            var objectStoreString: string = JSON.stringify(objectStore);
            this._storage.setItem(type, objectStoreString);
        }
    }
}
