/// <reference path="../../Typings/tsd.d.ts" />
/// <reference path="./IController.ts" />
/// <reference path="./ControllerRegistry.ts" />
/// <reference path="./ICreateControllerOptions.ts" />
/// <reference path="../Cache/ICache.ts" />
/// <reference path="../Events/IEventHub.ts" />
/// <reference path="../IComposerContext.ts" />
/// <reference path="../IComposerConfiguration.ts" />

module Orckestra.Composer {
    'use strict';

    /**
    * Factory for creating controllers.
    */
    export class ControllerFactory {
        private static _controllerRegistry = new Orckestra.Composer.ControllerRegistry();

        /**
        * Creates and returns an instance of a controller.
        */
        static createController(options: ICreateControllerOptions) : IController {

            var controllerConstructor =
                ControllerFactory._controllerRegistry.retrieveController(options.controllerName);

            return new controllerConstructor(
                options.context,
                options.eventHub,
                options.composerContext,
                options.composerConfiguration);
        }
    }
}
