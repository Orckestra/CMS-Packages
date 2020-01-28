///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='./Marker.ts' />

module Orckestra.Composer {
    'use strict';

    export class MarkerPool {
        private markers: Marker[] = [];
        private indexedMarkersByKey = {};

        private _map: google.maps.Map;
        private _onMarkerCreate: any;

        constructor(map: google.maps.Map, onMarkerCreate) {
            this._map = map;
            this._onMarkerCreate = onMarkerCreate;
        }

        public getMarkers(): any {
            return this.markers;
        }

        public get(isCluster: boolean = false): Marker {
            var marker = isCluster ? this.createClusterMarker() : this.createMarker();
            marker.isCluster = isCluster;
            this._onMarkerCreate(marker);
            this.markers.push(marker);

            return marker;
        }

        public getExisting(key) {
            return this.indexedMarkersByKey[key];
        }

        public index(marker: Marker) {
            this.indexedMarkersByKey[marker.key] = marker;
        }

        public hasClusters() {
            for (var i = 0; i < this.markers.length; i++) {
                if (this.markers[i] && this.markers[i].isCluster) {
                    return true;
                }
            }
            return false;
        }

        protected createMarker() {
            var marker = new MarkerWithLabel({
                map: this._map,
                labelInBackground: false,
                labelClass: 'store-marker',
                icon: '/UI.Package/Images/map/marker.png',
                labelAnchor: new google.maps.Point(12, 42),
                labelVisible: false
            });

            return new Marker(marker);
        }

        protected createClusterMarker() {
            var marker = new MarkerWithLabel({
                map: this._map,
                labelInBackground: false,
                labelClass: 'store-cluster-marker',
                icon: '/UI.Package/Images/map/cluster.png',
                labelAnchor: new google.maps.Point(12, 30),
                labelVisible: false
            });

            return new Marker(marker);
        }

        public releaseAll() {
            for (var i = 0; i < this.markers.length; i++) {
                if (this.markers[i]) { this.markers[i].setMap(null); }
            }
            delete this.markers;
            this.markers = [];
            this.indexedMarkersByKey = {};
        }


        public releaseByIndex(index: string) {
            var marker = this.indexedMarkersByKey[index];
            if (marker) {
                delete this.indexedMarkersByKey[index];
                marker.setMap(null);

                delete this.markers[this.markers.indexOf(marker)];
            }
        }

        public releaseClusters() {
            this.markers.forEach(mr => {
                if (mr.isCluster) {
                    this.releaseByIndex(mr.key);
                }
            });
        }

        public releaseMarkersByIds(iscluster: boolean, id: string) {
            if (!iscluster) {
                this.markers.forEach(mr => {
                    if (mr && mr.isCluster && mr.storeNumber.indexOf(id) >= 0) {
                        this.releaseByIndex(mr.key);
                        return;
                    }
                });
            } else {
                this.markers.forEach(mr => {
                    if (mr && !mr.isCluster && id.indexOf(mr.storeNumber) >= 0) {
                        this.releaseByIndex(mr.key);
                        return;
                    }
                });
            }
        }
    }
}