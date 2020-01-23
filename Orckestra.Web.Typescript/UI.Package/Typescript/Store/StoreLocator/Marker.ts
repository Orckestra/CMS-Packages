///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../../Typings/googlemaps/google.maps.d.ts' />

module Orckestra.Composer {
    'use strict';

    export class Marker {
        private _key: any;
        private _value: MarkerWithLabel;
        private _storeNumber: string;
        private _isCluster: boolean;

        constructor(marker: MarkerWithLabel) {
            this._value = marker;
        }

        get key(): any {
            return this._key;
        }

        get value(): MarkerWithLabel {
            return this._value;
        }

        set key(key: any) {
            this._key = key;
        }

        set value(marker: MarkerWithLabel) {
            this._value = marker;
        }

        get storeNumber() {
            return this._storeNumber;
        }
        set storeNumber(value: string) {
            this._storeNumber = value;
        }

        get isCluster() {
            return this._isCluster;
        }

        set isCluster(value: boolean) {
            this._isCluster = value;
        }

        public setMap(map) {
            this._value.setMap(map);
        }

        public setPosition(position: google.maps.LatLng) {
            this._value.setPosition(position);
        }
    }
}