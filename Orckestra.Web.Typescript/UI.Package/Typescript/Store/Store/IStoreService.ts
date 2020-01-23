///<reference path='../../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    export interface IStoreService {

        getStores(): Q.Promise<any>;
    }
}