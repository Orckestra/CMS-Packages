///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='./Services/StoreLocatorService.ts' />
///<reference path='./MapService.ts' />
///<reference path='./Services/GeoLocationService.ts' />
///<reference path='./IStoreLocatorInitializationOptions.ts' />
///<reference path='./IMapOptions.ts' />
///<reference path='./StoreLocatorHistoryState.ts' />
///<reference path='../../Cache/CacheProvider.ts' />

module Orckestra.Composer {

    export class StoreLocatorController extends Controller {

        protected _storeLocatorService: IStoreLocatorService = new StoreLocatorService();
        protected _geoService: GeoLocationService = new GeoLocationService();
        protected _mapService: MapService = new MapService(this.eventHub);
        protected _storeLocatorOptions: IStoreLocatorInitializationOptions;
        protected _historyState: StoreLocatorHistoryState;
        protected _isRestoreListPaging: boolean = false;
        protected _searchPointAddressCacheKey: string = 'StoreLocatorSearchAddress';
        protected cache = CacheProvider.instance().defaultCache;

        private _searchBox: google.maps.places.SearchBox;
        private _searchBoxJQ: JQuery;
        private _searchPoint: google.maps.LatLng;
        private _searchPointMarker: google.maps.Marker;
        private _isSearch: boolean = false;
        private _timer: number;
        private _enterPressedTimer: number;
        private _getCurrentLocation: Q.Deferred<google.maps.LatLng> = Q.defer<google.maps.LatLng>();

        public getCurrentLocation(): Q.Promise<google.maps.LatLng> {
            return this._getCurrentLocation.promise;
        }


        public initialize(options: IStoreLocatorInitializationOptions = {
            mapId: 'map',
            coordinates: { Lat: -33.8688, Lng: 151.2195 },
            showNearestStoreInfo: true
        }) {
            super.initialize();
            this.registerSubscriptions();
            this.initSearchBox();
            this._storeLocatorOptions = options;

            // get current location
            this._geoService.geolocate().then(location => {
                this._getCurrentLocation.resolve(location);
            }, reason => this._getCurrentLocation.resolve(null));

            // first check if address is posted from other page.
            var postedAddress = this._searchBoxJQ.val();

            if (!postedAddress) {
                // then check history state
                this.parseHistoryState();

                // then if any entered address saved in local storage
                this.cache.get<any>(this._searchPointAddressCacheKey)
                    .then(cachedAddr => {
                        postedAddress = cachedAddr;
                        this._searchBoxJQ.val(cachedAddr);
                    });
            }

            this._storeLocatorService.getMapConfiguration()
                .then(configuration => {
                    var mapOptions = this.getMapOptions();
                    if (configuration.ZoomLevel) {
                        this._storeLocatorOptions.zoomLevel = configuration.ZoomLevel;
                    }
                    if (configuration.MarkerPadding) {
                        this._storeLocatorOptions.markerPadding = configuration.MarkerPadding;
                    }
                    this._mapService.initialize(mapOptions);

                    if (!this._historyState) {
                        this._mapService.centerMap(configuration.Bounds);
                    }

                    this.searchBoxSetBounds(configuration.Bounds);

                    return this._mapService.mapInitialized();
                })
                .then(mapservice => {

                    if (this._historyState) {
                        this.restoreMapFromHistoryState();
                        return null;
                    }

                    if (postedAddress) {
                        return this._geoService.getLocationByAddress(postedAddress);
                    } else {
                        return this.getCurrentLocation();
                    }

                })
                .then(currentLocation => {
                    if (currentLocation) {
                        this.eventHub.publish('searchPointChanged', { data: currentLocation });
                    }
                })
                .fail(reason => this.handlePromiseFail('StoreLocator Initialize', reason));
        }

        private registerSubscriptions() {
            this.eventHub.subscribe('mapBoundsUpdated', e => this.onMapBoundsUpdated(e.data, this._isSearch));
            this.eventHub.subscribe('searchPointChanged', e => this.setSearchLocationInMap(e.data));
            this.eventHub.subscribe('markerClick', e => this.onMarkerClick(e.data));
            this.eventHub.subscribe('clusterClick', e => this.onClusterClick(e.data));
        }

        private initSearchBox() {
            this._searchBoxJQ = this.context.container.find('input[name="storeLocatorSearchInput"]');
            this._searchBox = new google.maps.places.SearchBox(<HTMLInputElement>this._searchBoxJQ[0]);

            this.searchBoxOnPlacesChanged();
            this.searchBoxOnEnterPressed();
        }

        private searchBoxOnPlacesChanged() {
            this._searchBox.addListener('places_changed', () => {
                clearTimeout(this._enterPressedTimer);
                var places = this._searchBox.getPlaces();
                if (places && places.length && places[0].geometry) {
                    this.eventHub.publish('searchPointChanged', { data: places[0].geometry.location });
                }
            });
        }

        private searchBoxOnEnterPressed() {
            this._searchBoxJQ.on('keypress', (e) => {
                var key = e.which || e.keyCode;
                if (key === 13) {
                    this._enterPressedTimer = setTimeout(() => {
                        if (this._searchPoint) {
                            this.setSearchLocationInMap(this._searchPoint);
                        }
                    }, 750);
                }
            });
        }

        protected getMapOptions(): IMapOptions {
            var mapCenter = new google.maps.LatLng(this._storeLocatorOptions.coordinates.Lat, this._storeLocatorOptions.coordinates.Lng);
            var mapOptions: IMapOptions = {
                mapCanvas: this.context.container.find(`#${this._storeLocatorOptions.mapId}`)[0],
                infoWindowMaxWidth: 450,
                options: {
                    center: this._historyState ? this._historyState.point : mapCenter,
                    zoom: this._historyState ? this._historyState.zoom : 1,
                    mapTypeId: google.maps.MapTypeId.ROADMAP,
                    panControl: false,
                    keyboardShortcuts: true,
                    scaleControl: false,
                    scrollwheel: false,
                    zoomControl: true,
                    streetViewControl: false,
                    overviewMapControl: true,
                    overviewMapControlOptions: { opened: false }
                }
            };

            return mapOptions;
        }

        private searchBoxSetBounds(bounds: any) {
            var southWest = new google.maps.LatLng(bounds.SouthWest.Lat, bounds.SouthWest.Lng);
            var northEast = new google.maps.LatLng(bounds.NorthEast.Lat, bounds.NorthEast.Lng);
            bounds = new google.maps.LatLngBounds(southWest, northEast);

            this._searchBox.setBounds(bounds);
        }

        protected onMapBoundsUpdated(data?: any, isSearch?: boolean): void {
            clearTimeout(this._timer);
            this._timer = setTimeout(() => {
                this.updateMarkers(data, isSearch);
            }, 750);
        }

        protected onMarkerClick(marker?: Marker): void {
            if (marker != null && marker.storeNumber) {
                this._storeLocatorService.getStore(marker.storeNumber)
                    .then((store) => {

                        this.getCurrentLocation().then(location => {
                            if (location) {
                                store.GoogleDirectionsLink = this._geoService.getDirectionLatLngSourceAddress(store.GoogleDirectionsLink, location);
                            }
                            var content = this.getRenderedTemplateContents('StoreMapMarkerInfo', store);
                            this._mapService.openInformationWindow(content, marker.value);
                        });

                    })
                    .fail(reason => this.handlePromiseFail('StoreLocator OnMarkerClick', reason));
            }
        }

        protected onClusterClick(marker?: Marker): void {
            this._mapService.getInformationWindow().close();
            this._mapService.getMap().panTo(marker.value.getPosition());
            marker.value.setMap(null);
            this._mapService.getMap().setZoom(this._mapService.getMap().getZoom() + 1);
        }

        protected updateMarkers(data?: any, isSearch: boolean = false) {
            var mapBounds = this._mapService.getBounds(this._storeLocatorOptions.markerPadding);
            var zoomLevel = this._mapService.getZoom();
            var searchPoint = this._searchPoint;
            var pageSize = this._isRestoreListPaging ? this._historyState.page * this.context.viewModel.pageSize : this.context.viewModel.pageSize;


            this._storeLocatorService.getMarkers(mapBounds.getSouthWest(), mapBounds.getNorthEast(), zoomLevel, searchPoint, isSearch, pageSize)
                .then((result) => {
                    if (result.Lat && result.Lng) {
                        this._mapService.extendBounds(searchPoint, new google.maps.LatLng(result.Lat, result.Lng));
                    } else {
                        this._mapService.setMarkers(result.Markers, isSearch);

                        if (this._isRestoreListPaging && result.NextPage) {
                            result.NextPage.Page = this._historyState.page + 1;
                        }
                        this.renderStoresList(result, null);
                        if (this._isRestoreListPaging && this._historyState.pos) {
                            $('html, body').animate({
                                scrollTop: this._historyState.pos
                            }, 500);
                            this.historyPushState(null, null, null, null, 0);
                        }
                        this._isRestoreListPaging = false;


                        if (this._storeLocatorOptions.showNearestStoreInfo && result.Stores) {
                            var firstStore = result.Stores[0];
                            if (firstStore && firstStore.SearchIndex === 1) {
                                this.setNearestStoreInfo(firstStore.DestinationToSearchPoint);
                            }
                        }
                    }
                    this.historyPushState(1, searchPoint, zoomLevel, this._mapService.getBounds().getCenter());
                    this._isSearch = false;
                })
                .fail(reason => this.handlePromiseFail('StoreLocator UpdateMarkers getMarkers', reason));
        }

        private setSearchLocationInMap(point: google.maps.LatLng, zoomLevel: number = this._storeLocatorOptions.zoomLevel) {
            this._searchPoint = point;
            this.createSearchPoitMarker();
            this._isSearch = true;
            this.cache.set(this._searchPointAddressCacheKey, this._searchBoxJQ.val());
            this._mapService.setLocationInMap(point, zoomLevel);
        }

        private createSearchPoitMarker() {
            var title = this._searchBoxJQ.val();
            if (this._searchPointMarker == null) {
                this._searchPointMarker = this._mapService.createMarkerOnMap(this._searchPoint, title);
            } else {
                this._searchPointMarker.setPosition(this._searchPoint);
                this._searchPointMarker.setTitle(title);
            }
        }

        // Action on Click on locator icon in search box
        public currentLocationAction(actionContext: IControllerActionContext) {
            actionContext.event.preventDefault();

            this._geoService.geolocate()
                .then(currentLocation => {
                    this.eventHub.publish('searchPointChanged', { data: currentLocation });

                    return this._geoService.getAddtressByLocation(currentLocation);
                })
                .then(address => {
                    this._searchBoxJQ.val(address);
                    this._searchPointMarker.setTitle(address);
                })
                .fail(reason => this.handlePromiseFail('StoreLocator CurrentLocationAction', reason));
        }

        // Next Page Action
        public nextPage(actionContext: IControllerActionContext) {
            var page: number = <any>actionContext.elementContext.data('page');

            this.getStoresForPage(page, this.context.viewModel.pageSize, actionContext.elementContext);
            actionContext.event.preventDefault();
        }

        // Remember element position in history
        public rememberPosition(actionContext: IControllerActionContext) {
            var position = $(document).scrollTop();
            this.historyPushState(null, null, null, null, position);
        }



        protected setNearestStoreInfo(info: string) {
            var nearestInfoPanel = $('#store-locator-nearest');

            if (!$('#nearestInfo').length) {
                nearestInfoPanel.html(nearestInfoPanel.html().replace('{0}', '<strong id=\'nearestInfo\'></strong>'));
            }

            $('#nearestInfo').html(info);
            nearestInfoPanel.removeClass('hide');
        }

        protected getStoresForPage(page: number, pageSize?: number, element?: any) {
            var mapBounds = this._mapService.getBounds(this._storeLocatorOptions.markerPadding);
            var searchPoint = this._searchPoint;
            var busy = this.asyncBusy({ elementContext: element });

            this._storeLocatorService.getStores(mapBounds.getSouthWest(), mapBounds.getNorthEast(), searchPoint,
                page, pageSize)
                .then((stores) => {
                    busy.done();
                    this.renderStoresList(stores, element[0].parentElement);
                });

            this.historyPushState(page);
        }

        protected renderStoresList(stores: any, target: HTMLElement): void {

            var listHtml = this.getRenderedTemplateContents('StoresList', stores);

            if (target == null) {
                var $list = $('#storesList').html('').stop().fadeOut();
                $list.html(listHtml).stop().fadeIn();
            } else {
                var position = $(target).offset().top + $(document).scrollTop();
                $(target).replaceWith(listHtml).stop().fadeIn();
                $('html, body').animate({
                    scrollTop: position
                }, 500);
            }

            this.setGoogleDirectionLinks();
        }

        protected setGoogleDirectionLinks(): Q.Promise<any> {

            return this.getCurrentLocation().then(location => {
                this._geoService.updateDirectionLinksWithLatLngSourceAddress(this.context.container, location);
            });
        }


        protected historyPushState(page: number, point?: google.maps.LatLng, zoom?: number, center?: google.maps.LatLng, elementPos?: number) {
            if (!this._historyState) {
                this._historyState = new StoreLocatorHistoryState();
            }
            if (page) {
                this._historyState.page = page;
            }
            if (point) {
                this._historyState.point = point;
            }
            if (zoom) {
                this._historyState.zoom = zoom;
            }

            if (center) {
                this._historyState.center = center;
            }

            if (elementPos >= 0) {
                this._historyState.pos = elementPos;
            }

            if (this._historyState.point) {
                var obj = {
                    'p_lat': this._historyState.point.lat(),
                    'p_lng': this._historyState.point.lng(),
                    'page': this._historyState.page,
                    'zoom': this._historyState.zoom,
                    'c_lat': this._historyState.center.lat(),
                    'c_lng': this._historyState.center.lng(),
                    'pos': this._historyState.pos
                };

                if (history.state) {
                    history.replaceState(obj, null, null);
                } else {
                    history.pushState(obj, null, null);
                }
            }
        }

        protected parseHistoryState() {
            if (history.state) {

                this._historyState = new StoreLocatorHistoryState();
                if (history.state.p_lat && history.state.p_lng) {
                    this._historyState.point = new google.maps.LatLng(history.state.p_lat, history.state.p_lng);
                }

                if (history.state.c_lat && history.state.c_lng) {
                    this._historyState.center = new google.maps.LatLng(history.state.c_lat, history.state.c_lng);
                }

                this._historyState.zoom = history.state.zoom;
                this._historyState.page = history.state.page;
                this._historyState.pos = history.state.pos;
            }
        }

        protected restoreMapFromHistoryState() {
            console.log('Restore data from history state');
            this._searchPoint = this._historyState.point;
            this.createSearchPoitMarker();

            if (this._historyState.center) {
                this._mapService.getMap().setCenter(this._historyState.center);
            }

            if (this._historyState.page > 1) {
                this._isRestoreListPaging = true;
            }
        }

        protected handlePromiseFail(title: string, reason: any) {
            console.log(title + ': ' + reason);
        }
    }
}