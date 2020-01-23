///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../Mvc/IControllerContext.ts' />
///<reference path='../../../Mvc/ComposerClient.ts' />
///<reference path='../../../Events/EventHub.ts' />
///<reference path='./IStoreLocatorService.ts' />
///<reference path='./StoreLocatorEndPointUrls.ts' />


module Orckestra.Composer {
    'use strict';

    export class StoreLocatorService implements IStoreLocatorService {

        private memoizeStore: Function;

        public getStore(storeNumber: string): Q.Promise<any> {
            if (!storeNumber) {
                throw new Error('The Store Number is required');
            }

            if (!this.memoizeStore) {
                this.memoizeStore =
                    _.memoize(this.getStoreImpl, (storeNumber) => storeNumber);
            }

            return this.memoizeStore(storeNumber);
        }

        private getStoreImpl(storeNumber: string): Q.Promise<any> {
            var data = { StoreNumber: storeNumber };
            return ComposerClient.post(StoreLocatorEndPointUrls.GetStoreEndPointUrl, data);
        }

        public getStores(southWest: google.maps.LatLng, northEast: google.maps.LatLng,
            searchPoint: google.maps.LatLng, page: number, pageSize: number): Q.Promise<any> {

            var data = {
                page: page,
                pageSize: pageSize,
                mapBounds: {
                    southWest: southWest,
                    northEast: northEast
                },
                searchPoint: searchPoint
            };

            return ComposerClient.post(StoreLocatorEndPointUrls.GetStoresEndPointUrl, data);
        }

        public getMapConfiguration(): Q.Promise<any> {
            return ComposerClient.get(StoreLocatorEndPointUrls.GetMapConfigurationEndPointUrl);
        }

        public getMarkers(southWest: google.maps.LatLng, northEast: google.maps.LatLng, zoomLevel: number,
            searchPoint: google.maps.LatLng, isSearch: boolean, pageSize: number): Q.Promise<any> {

            var data = {
                zoomLevel: zoomLevel,
                mapBounds: {
                    southWest: southWest,
                    northEast: northEast
                },
                searchPoint: searchPoint,
                isSearch: isSearch,
                pageSize: pageSize
            };

            return ComposerClient.post(StoreLocatorEndPointUrls.GetMarkersEndPointUrl, data);
        }
    }
}