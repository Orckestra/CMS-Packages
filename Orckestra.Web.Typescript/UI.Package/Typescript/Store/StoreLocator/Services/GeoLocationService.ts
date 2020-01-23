///<reference path='../../../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    'use strict';

    export class GeoLocationService {
        private _browserGeolocation: Geolocation = navigator.geolocation;
        private _geocoder: google.maps.Geocoder;
        private _currenctLocation: google.maps.LatLng;

        constructor() {
            this._geocoder = new google.maps.Geocoder();
        }

        public geolocate(): Q.Promise<any> {
            if (this._browserGeolocation) {
                return this.getCurrentLocation()
                    .catch((reason) => {
                        console.log(reason);
                        return this._currenctLocation;
                    });
            }

            return null;
        }

        public getCurrentLocation(): Q.Promise<google.maps.LatLng> {
            var deferred = Q.defer<google.maps.LatLng>();
            this._browserGeolocation.getCurrentPosition((pos) => {
                this._currenctLocation = new google.maps.LatLng(pos.coords.latitude, pos.coords.longitude);
                deferred.resolve(this._currenctLocation);
            }, () => { deferred.reject('problems to get current location'); });
            return deferred.promise;
        }

        public getAddtressByLocation(location: google.maps.LatLng): Q.Promise<string> {
            var deferred = Q.defer<string>();

            this._geocoder.geocode({ location: location },
                function (results, status) {
                    if (status === google.maps.GeocoderStatus.OK) {
                        deferred.resolve(results[0].formatted_address);
                    } else {
                        deferred.resolve('');
                    }
                }
            );

            return deferred.promise;
        }

        public getLocationByAddress(address: string): Q.Promise<google.maps.LatLng> {
            var deferred = Q.defer<google.maps.LatLng>();

            this._geocoder.geocode({ address: address },
                (results, status) => {
                    if (status === google.maps.GeocoderStatus.OK) {
                        var location = results[0].geometry.location;
                        deferred.resolve(location);
                    } else {
                        deferred.resolve(null);
                        console.log('Location not resolved by Google ' + address);
                    }
                }
            );

            return deferred.promise;
        }

        /// By default render with default value for ViewModel.GoogleDirectionsLink (direction with Empty Start Point), 
        /// and when User Accept his Current Location, just in async task update HREF attributes and attach current location coordinates.
        /// We do not update the ViewModel before rendering, as we need to wait for User Input
        public updateDirectionLinksWithLatLngSourceAddress(container: JQuery, sourceLocation: google.maps.LatLng) {

            if (!sourceLocation) { return; }

            var ctaDirs = container.find('.ctaGoogleDir');

            ctaDirs.each((ind, ctaDir) => {
                var href = $(ctaDir).attr('href');
                if (href.indexOf('saddr') === -1) {
                    $(ctaDir).attr('href', this.getDirectionLatLngSourceAddress(href, sourceLocation));
                }
            });
        }

        public getDirectionLatLngSourceAddress(baseUrl: string, sourceLocation: google.maps.LatLng) {
            return `${baseUrl}&saddr=${sourceLocation.lat()},${sourceLocation.lng()}`;
        }
    }
}