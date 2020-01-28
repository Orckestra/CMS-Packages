///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../../Typescript/Mvc/IControllerContext.ts' />

module Orckestra.Composer.Mocks {
    'use strict';

    import IControllerContext = Orckestra.Composer.IControllerContext;

    export var MockControllerContext: IControllerContext = {
        container: $(),
        dataItemId: 'SomeDataItemId',
        templateName: 'SomeTemplateName',
        viewModel: {},
        window: window
    };
}
