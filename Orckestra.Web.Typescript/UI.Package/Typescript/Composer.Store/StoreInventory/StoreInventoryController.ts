///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='./Services/StoreInventoryService.ts' />
///<reference path='../StoreLocator/Services/GeoLocationService.ts' />
///<reference path='../../Cache/CacheProvider.ts' />

module Orckestra.Composer {

    export class StoreInventoryController extends Controller {

        protected _concern: string = 'StoreInventory_';
        protected _service: IStoreInventoryService = new StoreInventoryService();
        protected _geoService: GeoLocationService = new GeoLocationService();
        protected _searchPointAddressCacheKey: string = 'StoreLocatorSearchAddress';
        protected cache = CacheProvider.instance().defaultCache;
        private _searchBox: google.maps.places.SearchBox;
        private _searchBoxJQ: JQuery;
        private _searchPoint: google.maps.LatLng;
        private _selectedSku: string;
        private _isAuthenticated: boolean;
        private _pageSize: number;
        private _productId: any;
        private _getCurrentLocation: Q.Deferred<google.maps.LatLng> = Q.defer<google.maps.LatLng>();

        public getCurrentLocation(): Q.Promise<google.maps.LatLng> {
            return this._getCurrentLocation.promise;
        }


        public initialize() {
            var getDefaultsTasks: Q.Promise<any>[] = [];
            var getDefaultAddressTask: Q.Promise<any>;

            super.initialize();
            this.registerSubscriptions();
            this.initSearchBox();
            this.getDataFromContextViewModel();

            if (!this._selectedSku && this._productId) {
                getDefaultsTasks.push(this._service.getSkuSelection(this._productId).then(result => {
                    this._selectedSku = result.Sku;
                }));
            }

            getDefaultsTasks.push(this.getDefaultAddress());

            Q.all(getDefaultsTasks).then(() => {
                this.getStoresInventory();
            }).fail(reason => console.log(this._concern + reason));

            this._geoService.geolocate().then(location => {
                this._getCurrentLocation.resolve(location);
            }, reason => this._getCurrentLocation.resolve(null));
        }

        private registerSubscriptions() {
            this.eventHub.subscribe('productDetailSelectedVariantIdChanged', e => this.onSelectedVariantIdChanged(e));
            this.eventHub.subscribe('inventorySearchPointChanged', e => this.searchPointChanged(e));
            this.context.window.addEventListener('hashchange', () => this.onHashChanged());
        }

        private getDataFromContextViewModel() {
            this._selectedSku = this.context.viewModel.selectedSku;
            this._isAuthenticated = this.context.viewModel.isAuthenticated;
            this._pageSize = this.context.viewModel.pageSize;
            this._productId = this.context.viewModel.productId;
        }

        protected initSearchBox() {
            this._searchBoxJQ = this.context.container.find('input[name="storeInventorySearchInput"]');
            this._searchBox = new google.maps.places.SearchBox(<HTMLInputElement>this._searchBoxJQ[0]);
            this._searchBox.addListener('places_changed', () => {
                var places = this._searchBox.getPlaces();
                if (places && places.length && places[0].geometry) {
                    this.eventHub.publish('inventorySearchPointChanged', { data: places[0].geometry.location });
                }
            });
        }

        protected searchPointChanged(e: IEventInformation) {
            this._searchPoint = e.data;
            this.cache.set(this._searchPointAddressCacheKey, this._searchBoxJQ.val());
            this.getStoresInventory();
        }

        protected onSelectedVariantIdChanged(e: IEventInformation) {
            this._selectedSku = e.data.selectedSku;
            this.getStoresInventory();
        }

        protected onHashChanged() {
            if (location.hash === '#storeinventory' && !this._searchPoint) {
                this.getCurrentLocation()
                    .then(currentLocation => {
                        if (currentLocation) {
                            this._searchPoint = currentLocation;
                            this.getStoresInventory();
                            this._geoService.getAddtressByLocation(currentLocation).then(result => {
                                this.cache.set(this._searchPointAddressCacheKey, result);
                                this._searchBoxJQ.val(result);
                            });
                        }
                    });
            }
        }

        protected getStoresInventory(): Q.Promise<any> {
            var debounceHandle;

            if (this._selectedSku) {
                debounceHandle = _.debounce(() => this.render('StoreInventoryList', { IsLoading: true }), 300);

                return this._service.getStoresInventory(this.getStoresInventoryParam())
                    .then(result => {
                        debounceHandle.cancel();
                        this.render('StoreInventoryList', result);
                        this.setGoogleDirectionLinks();
                    })
                    .fail(reason => console.log(reason));
            }
        }

        protected nextPage(actionContext: IControllerActionContext) {

            actionContext.event.preventDefault();
            var page: number = <any>actionContext.elementContext.data('page');
            var busy = this.asyncBusy({ elementContext: actionContext.elementContext });

            this._service.getStoresInventory(this.getStoresInventoryParam(page))
                .then(result => {
                    var target = actionContext.elementContext[0].parentElement;
                    var targetHtml = this.getRenderedTemplateContents('StoreInventoryList', result);
                    busy.done();
                    $(target).replaceWith(targetHtml).stop().fadeIn();

                    this.setGoogleDirectionLinks();
                })
                .fail(reason => console.log(reason));
        }

        protected setGoogleDirectionLinks(): Q.Promise<any> {

            return this.getCurrentLocation().then(location => {

                this._geoService.updateDirectionLinksWithLatLngSourceAddress(this.context.container, location);

            });
        }

        protected getStoresInventoryParam(page: number = 1) {
            var param = new GetStoresInventoryParam();
            param.Sku = this._selectedSku;
            param.SearchPoint = this._searchPoint;
            param.Page = page;
            param.Pagesize = this._pageSize;

            return param;
        }

        protected getDefaultAddress(): Q.Promise<any> {
            // try get address from local storage
            return this.cache.get<any>(this._searchPointAddressCacheKey)
                .then(cachedAddr => {
                    this._searchBoxJQ.val(cachedAddr);
                    return this._geoService.getLocationByAddress(cachedAddr);
                }, (reason) => {
                    // try get customer default delivery address
                    if (this._isAuthenticated) {
                        return this._service.getDefaultAddress()
                            .then(defaultAddr => {
                                var formattedAddress
                                    = `${defaultAddr.City}, ${defaultAddr.RegionCode} ${defaultAddr.PostalCode}, ${defaultAddr.CountryCode}`;
                                this._searchBoxJQ.val(formattedAddress);
                                return this._geoService.getLocationByAddress(formattedAddress);
                            });
                    }
                })
                .then(locationByAddress => {
                    this._searchPoint = locationByAddress;
                });
        }
    }
}