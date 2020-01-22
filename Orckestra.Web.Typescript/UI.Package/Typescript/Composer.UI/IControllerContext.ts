///<reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    export interface IControllerContext {
        templateName: string;
        dataItemId: string;
        container: JQuery;
        viewModel: any;
        window: Window;
    }
}

