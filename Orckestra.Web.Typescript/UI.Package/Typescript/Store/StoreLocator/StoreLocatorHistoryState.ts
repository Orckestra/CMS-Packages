///<reference path='../../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    'use strict';

    export class StoreLocatorHistoryState {
        point: google.maps.LatLng;
        page: number;
        zoom: number;
        center: google.maps.LatLng;
        pos: number;
    }
}