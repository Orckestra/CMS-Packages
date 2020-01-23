module Orckestra.Composer {

    export interface IStorageItem<T> {

        /**
         * The unique identifier of the object stored or to be stored.
         */
        id: string;

        /**
         * The object stored or to be stored.
         */
        value: T;
    }
}
