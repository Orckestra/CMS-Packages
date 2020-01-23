///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Generics/Collections/IHashTable.ts' />
///<reference path='./IMapOptions.ts' />
///<reference path='./MarkerPool.ts' />

module Orckestra.Composer {
    'use strict';

    export class MapService {

        private eventHub: IEventHub;
        private _map: google.maps.Map;
        private _markerPool: MarkerPool;
        private _prevZoom: number;
        private _currentLocationMarker: google.maps.Marker;
        private _informationWindow: google.maps.InfoWindow;
        private _projectionOverlay: google.maps.OverlayView;

        private _mapInitialized: Q.Deferred<MapService> = Q.defer<MapService>();
        private _mapIdle: Q.Deferred<MapService> = Q.defer<MapService>();
        private _mapDragEnded: Q.Deferred<MapService> = Q.defer<MapService>();

        constructor(eventHub: IEventHub) {
            this.eventHub = eventHub;
        }

        public initialize(mapOptions: IMapOptions) {

            this._map = new google.maps.Map(mapOptions.mapCanvas, mapOptions.options);
            this._informationWindow = new google.maps.InfoWindow({ maxWidth: mapOptions.infoWindowMaxWidth });
            this._markerPool = new MarkerPool(this._map, (marker) => { this.onNewMarkerCreated(marker); });
            this.setProjectionOverlay();

            google.maps.event.addListener(this._map, 'click', () => this._informationWindow.close());

            google.maps.event.addListener(this._map, 'zoom_changed', () => this._markerPool.releaseClusters());

            google.maps.event.addListener(this._map, 'bounds_changed', () => {
                this.mapDragEnded()
                    .then(() => {
                        this.eventHub.publish('mapBoundsUpdated', { data: this._map.getBounds() });
                    });
            });

            google.maps.event.addListener(this._map, 'idle', () => {
                this._mapIdle.resolve(this);
            });

            google.maps.event.addListener(this._map, 'dragstart', () => {
                this._mapDragEnded = Q.defer<MapService>();
            });
            google.maps.event.addListener(this._map, 'dragend', () => {
                this._mapDragEnded.resolve(this);
            });

            this._mapInitialized.resolve(this);
            this._mapDragEnded.resolve(this);
        }

        private setProjectionOverlay() {
            this._projectionOverlay = new google.maps.OverlayView();
            this._projectionOverlay.draw = function name() {
                //
            };
            this._projectionOverlay.setMap(this._map);
        }

        public getMap() {
            return this._map;
        }

        public getInformationWindow(): google.maps.InfoWindow {
            return this._informationWindow;
        }

        public getBounds(markerPadding?: number): google.maps.LatLngBounds {
            var bounds = this._map.getBounds();

            if (markerPadding) {
                var tr = new google.maps.LatLng(bounds.getNorthEast().lat(),
                    bounds.getNorthEast().lng());
                var bl = new google.maps.LatLng(bounds.getSouthWest().lat(),
                    bounds.getSouthWest().lng());
                var projection = this._projectionOverlay.getProjection();

                if (projection) {
                    var trPix = projection.fromLatLngToDivPixel(tr);
                    var blPix = projection.fromLatLngToDivPixel(bl);

                    blPix.y = blPix.y + markerPadding;
                    trPix.y = trPix.y + markerPadding;

                    var sw = projection.fromDivPixelToLatLng(blPix);
                    var ne = projection.fromDivPixelToLatLng(trPix);

                    return new google.maps.LatLngBounds(sw, ne);
                }
            }

            return bounds;
        }

        public getZoom(): number {
            return this._map.getZoom();
        }

        public onNewMarkerCreated(marker: Marker) {

            if (!marker) {
                return;
            }

            if (marker.isCluster) {
                google.maps.event.addListener(marker.value, 'click', () => {
                    this.eventHub.publish('clusterClick', { data: marker });
                });
            } else {
                google.maps.event.addListener(marker.value, 'click', () => {
                    this.eventHub.publish('markerClick', { data: marker });
                });
            }
        }

        public mapInitialized(): Q.Promise<MapService> {
            return this._mapInitialized.promise;
        }

        public mapIdle(): Q.Promise<MapService> {
            return this._mapIdle.promise;
        }

        public mapDragEnded(): Q.Promise<MapService> {
            return this._mapDragEnded.promise;
        }

        public centerMap(storeBounds: any): void {
            if (storeBounds != null) {
                this.mapIdle().then(
                    () => {
                        var southWest = new google.maps.LatLng(storeBounds.SouthWest.Lat, storeBounds.SouthWest.Lng);
                        var northEast = new google.maps.LatLng(storeBounds.NorthEast.Lat, storeBounds.NorthEast.Lng);
                        var bounds = new google.maps.LatLngBounds(southWest, northEast);
                        this._map.fitBounds(bounds);
                        this._mapIdle = Q.defer<MapService>();
                    });
            }
        }

        public openInformationWindow(content: any, anchor?: any) {
            this._informationWindow.setContent(content);
            this._informationWindow.open(this._map, anchor);
        }

        public setLocationInMap(point: google.maps.LatLng, zoomLevel = 11) {
            this.mapIdle().then(
                () => {
                    this._map.setCenter(point);
                    this._map.setZoom(zoomLevel);
                    this._mapIdle = Q.defer<MapService>();
                });
        }

        public extendBounds(point1: google.maps.LatLng, point2: google.maps.LatLng) {
            this.mapIdle().then(
                () => {
                    var bounds: google.maps.LatLngBounds = new google.maps.LatLngBounds();
                    bounds.extend(point1);
                    bounds.extend(point2);
                    this._map.fitBounds(bounds);
                    this._map.setCenter(bounds.getCenter());
                    this._mapIdle = Q.defer<MapService>();
                });
        }


        public createMarkerOnMap(location: google.maps.LatLng, title: string): google.maps.Marker {
            return new google.maps.Marker({
                position: location,
                map: this._map,
                title: title,
                icon: 'https://maps.google.com/mapfiles/marker_orange.png'
            });
        }

        public setMarkers(markerInfos: any[], isSearch: boolean = false): void {

            var curZoom = this.getZoom();
            var action = this._prevZoom === curZoom ? 'PAN' : this._prevZoom < curZoom ? 'ZOOM_IN' : 'ZOOM_OUT';

            if (!this._markerPool.hasClusters() && action === 'ZOOM_IN') {
                this._markerPool.releaseAll();
            }
            if (isSearch) {
                this._markerPool.releaseAll();
            }

            this._prevZoom = curZoom;

            this.transformResult(markerInfos, this._markerPool, action)
                .then((newMarkers) => {
                    newMarkers.forEach(m => {
                        m.value.labelVisible = true;
                    });
                });
        }

        private transformResult(result, markerPool: MarkerPool, action): Q.Promise<any[]> {
            var deferred = Q.defer<Marker[]>();
            var markers: Marker[] = [];


            function buildMarker(m) {
                var key = m.Center.Lat + '-' + m.Center.Lng;
                var marker: Marker = null;
                var isCluster = m.ItemsCount > 1;

                markerPool.releaseMarkersByIds(isCluster, m.StoreNumber);
                marker = markerPool.getExisting(key);

                if (!marker) {
                    marker = markerPool.get(isCluster);
                    var position = new google.maps.LatLng(m.Center.Lat, m.Center.Lng);
                    marker.value.setPosition(position);
                    marker.value.labelContent = (!isCluster ? m.SearchIndex : m.ItemsCount);
                    marker.key = key;
                    marker.storeNumber = m.StoreNumber;
                    marker.isCluster = isCluster;
                    markerPool.index(marker);
                    markers.push(marker);
                }

                marker.value.labelContent = (!isCluster ? m.SearchIndex : m.ItemsCount); //update search index;
            }

            for (var i = 0; i < result.length; i++) {
                var ma = result[i];
                buildMarker(ma);
            }

            deferred.resolve(markers);

            return deferred.promise;
        }
    }
}
