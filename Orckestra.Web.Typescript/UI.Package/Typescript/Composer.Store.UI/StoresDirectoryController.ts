///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Composer.UI/ComposerClient.ts' />
///<reference path='../Composer.UI/Controller.ts' />
///<reference path='../Composer.UI/IControllerActionContext.ts' />
///<reference path='./GeoLocationService.ts' />

module Orckestra.Composer {

    export class StoresDirectoryController extends Controller {

        protected _geoService: GeoLocationService = new GeoLocationService();
        private _searchBox: google.maps.places.SearchBox;

        public initialize() {
            super.initialize();
            this.initializeSearchBox();

            this.setGoogleDirectionLinks();
        }


        protected initializeSearchBox() {
            var input = <HTMLInputElement>this.context.container.find('#storeDirectorySearchInput')[0];
            this._searchBox = new google.maps.places.SearchBox(input);
        }

        // Action on Click on locator icon in search box
        public currentLocationAction(actionContext: IControllerActionContext) {
            actionContext.event.preventDefault();
            this.context.container.find('form').submit();
        }

        protected setGoogleDirectionLinks() {

            return this._geoService.geolocate().then(location => {
                this._geoService.updateDirectionLinksWithLatLngSourceAddress(this.context.container, location);
            });
        }
    }
}