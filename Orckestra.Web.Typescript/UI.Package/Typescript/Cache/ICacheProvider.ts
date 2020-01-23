/// <reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    export interface ICacheProvider {

        defaultCache: ICache;
        customCache: ICache;

        localStorage: Storage;
        sessionStorage: Storage;
    }
}