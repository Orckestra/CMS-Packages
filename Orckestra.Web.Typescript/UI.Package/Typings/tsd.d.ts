/// <reference path="./bootstrap/bootstrap.d.ts" />
/// <reference path="./jquery/jquery.d.ts" />
/// <reference path="./jasmine/jasmine.d.ts" />
/// <reference path="./lodash/lodash.d.ts" />
/// <reference path="./q/Q.d.ts" />
/// <reference path="./sinon/sinon.d.ts" />
/// <reference path="./handlebars/handlebars.d.ts" />
/// <reference path="./chance/chance.d.ts" />
/// <reference path="./nouislider/nouislider.d.ts" />
/// <reference path="./slick-carousel/slick-carousel.d.ts" />
/// <reference path="./wnumb/wnumb.d.ts" />
/// <reference path="./googlemaps/google.maps.d.ts" />
/// <reference path="./googlemaps/google.maps.markerwithlabel.d.ts" />
/// <reference path="./bootstrap-datepicker/bootstrap-datepicker.d.ts" />
/// <reference path="./typeahead/typeahead.d.ts" />

// Very barebones interface for jasmine.Ajax as there is currently no d.ts file on DefinitelyTyped.
declare module jasmine {
    export var Ajax: any;
}
