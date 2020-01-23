///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='./Services/GeoLocationService.ts' />

module Orckestra.Composer {

    export class StoreDetailsController extends Controller {

        protected _geoService: GeoLocationService = new GeoLocationService();

        private _map: google.maps.Map;
        private _marker: google.maps.Marker;

        public initialize() {
            super.initialize();

            var center = new google.maps.LatLng(this.context.viewModel.latitude, this.context.viewModel.longitude);
            var mapOptions: google.maps.MapOptions = {
                center: center,
                zoom: this.context.viewModel.zoom ? this.context.viewModel.zoom : 14,
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                panControl: false,
                keyboardShortcuts: false,
                scaleControl: false,
                scrollwheel: false,
                zoomControl: false,
                draggable: false,
                streetViewControl: false,
                overviewMapControl: false,
                overviewMapControlOptions: { opened: false },
                disableDefaultUI: true
            };

            this._map = new google.maps.Map(this.context.container.find(`#map`)[0], mapOptions);
            this._marker = new google.maps.Marker({
                position: center,
                map: this._map,
                icon: '/UI.Package/Images/map/marker-default.png'
            });

            this.context.window.addEventListener('resize', () => this._map.setCenter(this._marker.getPosition()));


            this.setGoogleDirectionLink();
        }

        protected setGoogleDirectionLink() {
            this._geoService.geolocate().then(location => {
                this._geoService.updateDirectionLinksWithLatLngSourceAddress(this.context.container, location);
            });
        }
    }
}