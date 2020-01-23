///<reference path='../Generics/Collections/IHashTable.ts' />
///<reference path='./IController.ts' />

module Orckestra.Composer {
    export class ControllerRegistry {
        private static _instance : ControllerRegistry;
        private static _registry : IHashTable<any> = {}; //IHashTable<IController> = {};

        constructor() {
            if (ControllerRegistry._instance === void 0) {
                ControllerRegistry._instance = this;
            }

            return ControllerRegistry._instance;
        }

        public isRegistered(controllerName): boolean {
            return ControllerRegistry._registry.hasOwnProperty(controllerName);
        }

        public retrieveController(controllerName: string) {
            if (!this.isRegistered(controllerName)) {
                throw new Error('Unable to unregister the controller ' + controllerName + ' because it does not exist in the registry');
            }

            return ControllerRegistry._registry[controllerName];
        }

        public register(controllerName: string, controller: any ) { //Orckestra.Composer.IController) {
            if (this.isRegistered(controllerName)) {
                throw new Error('The controller ' + controllerName + ' is already registered.');
            }

            ControllerRegistry._registry[controllerName] = controller;
        }

        public unregister(controllerName: string): any { //: IController {
            var unregisteredController: any; // IController;

            if (!this.isRegistered(controllerName)) {
                throw new Error('Unable to unregister the controller ' + controllerName + ' because it does not exist in the registry');
            }

            delete ControllerRegistry._registry[controllerName];

            return unregisteredController;
        }
    }
}
