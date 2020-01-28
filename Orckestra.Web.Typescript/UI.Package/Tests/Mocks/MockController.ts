///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../../Typescript/Mvc/IController.ts' />
///<reference path='../../Typescript/Mvc/IControllerContext.ts' />

module Orckestra.Composer.Mocks {

    export class MockController extends Orckestra.Composer.Controller {
        public initialize() {
            console.log('just a mock.');
        }

        public dispose() {
            console.log('just a mock.');
        }
    }
}
