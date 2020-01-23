///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Mvc/Controller.ts' />

module Orckestra.Composer {
    export class SearchBoxController extends Controller {
        public initialize() {
            super.initialize();
            this.registerFormsForValidation(this.context.container.find('form'));
        }
    }
}