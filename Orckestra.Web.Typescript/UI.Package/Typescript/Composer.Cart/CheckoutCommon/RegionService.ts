///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/ComposerClient.ts' />
///<reference path='./IRegionService.ts' />

module Orckestra.Composer {
    'use strict';

export class RegionService implements IRegionService {

        private _memoizeGetRegions: Function;

        public getRegions(): Q.Promise<any> {

             if (_.isUndefined(this._memoizeGetRegions)) {

                this._memoizeGetRegions = _.memoize(arg => this.getRegionsImpl());
            }

            return this._memoizeGetRegions();
        }

         private getRegionsImpl(): Q.Promise<any> {

            return ComposerClient.get('/api/address/regions');
        }
    }
}
