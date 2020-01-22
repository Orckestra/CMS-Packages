module Orckestra.Composer {
    export interface IStoreLocatorInitializationOptions {
        mapId: string;
        coordinates: {
            Lat: number;
            Lng: number;
        };
        showNearestStoreInfo: boolean;
        zoomLevel?: number;
        markerPadding?: number;
    }
}