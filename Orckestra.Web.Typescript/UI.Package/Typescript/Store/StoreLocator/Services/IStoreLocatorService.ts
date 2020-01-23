///<reference path='../../../../Typings/tsd.d.ts' />
module Orckestra.Composer {
    export interface IStoreLocatorService {
        getMapConfiguration(): Q.Promise<any>;

        getStore(storeNumber: string): Q.Promise<any>;

        getStores(southWest: google.maps.LatLng, northEast: google.maps.LatLng, searchPoint: google.maps.LatLng,
            page: number, pageSize: number): Q.Promise<any>;

        getMarkers(southWest: google.maps.LatLng, northEast: google.maps.LatLng,
            zoomLevel: number, searchPoint: google.maps.LatLng, isSearch: boolean, pageSize: number): Q.Promise<any>;
    }
}
