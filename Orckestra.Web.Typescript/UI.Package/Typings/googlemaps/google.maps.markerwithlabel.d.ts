/// <reference path="./google.maps.d.ts"/>
declare class MarkerWithLabelOptions extends MarkerWithLabel {
    constructor();
    crossImage: string;
    handCursor: string;
    labelAnchor: any;
    labelClass: string;
    labelContent: any;
    labelInBackground: boolean;
    labelStyle: any;
    labelVisible: boolean;
    optimized: boolean;
    raiseOnDrag: boolean;
    position: any;

}

declare class MarkerWithLabel extends google.maps.Marker {
    constructor(opts?: any);
    crossImage: string;
    handCursor: string;
    labelAnchor: any;
    labelClass: string;
    labelContent: any;
    labelInBackground: boolean;
    labelStyle: any;
    labelVisible: boolean;
    optimized: boolean;
    raiseOnDrag: boolean;
}
