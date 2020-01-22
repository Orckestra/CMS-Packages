/// <reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    /**
     * Defines a storage engine API capable of performing all the basic CRUD operations.
     * The API operates on Javascript objects, and all objects are stored according to their defined type.
     *
     * This API has been designed in such a way that the underlying storage engine could be any of those offered by popular browsers,
     * such as LocalStorage, SessionStorage, and IndexedDB
    */
    export interface IStorage {

        /**
         * The client must call this to initialize the storage engine before using it.
         * If the storage engine initializes successfully, the promise will be fulfilled.
         * The promise will be rejected if the storage engine cannot be used.
         * It should be possible to call this method multiple times, and the same result will be returned each time.
         */
        init(): Q.Promise<void>;

        /**
         * The client must call this to initialize the object store associated with a specific object type.
         * If the storage engine supports the object type, the promise will be fulfilled.
         * The promise will be rejected if the object type cannot be stored.
         * It should be possible to call this method multiple times, and the same result will be returned each time.
         */
        initObjectStore(type: string): Q.Promise<void>;

        /**
         * If an object with a specific id for a specific type is found, the promise will be fulfilled with that object.
         * If no object is found, the promise will be fulfilled with a null object.
         */
        get<T>(type: string, id: string): Q.Promise<T>;

        /**
         * This will remove an object with a specific id for a specific type.
         * If no object is found, the promise will be fulfilled.
         * If an object is found and removed, the promise will be fulfilled.
         */
        remove(type: string, id: string): Q.Promise<void>;

        /**
         * This will remove the specific type
         * If no type is found, the promise will be fulfilled.
         * If a type is found and removed, the promise will be fulfilled.
         */
        fullRemove(type: string): Q.Promise<void>;

        /**
         * This will handle adding and updating objects of a specific type.
         * On success, the promise will be fulfilled.
         */
        set<T>(type: string, item: IStorageItem<T>): Q.Promise<void>;
    }
}
