/// <reference path='./ICachePolicy.ts' />

module Orckestra.Composer {

    export interface ICacheItem<T> {

        /**
         * Represents the value to be cached.
         */
        value: T;

        /**
         * Represents a set of eviction and expiration details for a specific cache entry.
         */
        policy: ICachePolicy;

        /**
         * The number of milliseconds since 1970/01/01 corresponding to the time the cache item was last accessed.
         */
        lastAccessed: number;
    }
}
