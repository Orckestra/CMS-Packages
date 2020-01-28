///<reference path='../../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    'use strict';

    export interface IRegionService {

        /**
         * Return a Promise which returns an array of region.
         */
        getRegions(): Q.Promise<any>;
    }
}
