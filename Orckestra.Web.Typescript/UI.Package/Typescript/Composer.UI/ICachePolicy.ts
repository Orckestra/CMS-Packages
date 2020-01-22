/// <reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    /**
     * Represents a set of eviction and expiration details for a specific cache entry.
    */
    export interface ICachePolicy {

        /**
         * The number of milliseconds since 1970/01/01 corresponding to the time when the cache entry should expire.
         */
        absoluteExpiration?: number;

        /**
         * Represents the seconds since the last cache access after which the cache entry should expire.
         */
        slidingExpiration?: number;
    }
}
