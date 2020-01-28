///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../../Typescript/Mvc/ControllerFactory.ts' />
///<reference path='../../Typescript/Mvc/ControllerRegistry.ts' />
///<reference path='../../Typescript/Events/EventHub.ts' />
///<reference path='../Mocks/MockController.ts' />
///<reference path='../Mocks/MockControllerContext.ts' />
///<reference path='../../Typescript/IComposerContext.ts' />
///<reference path='../../Typescript/IComposerConfiguration.ts' />

(() => {
    'use strict';

    // TODO: Move this in to it's own file once the composer.mocks.js is built.

    var composerContext: Orckestra.Composer.IComposerContext = {
        language: 'en-CA'
    };
    var composerConfiguration: Orckestra.Composer.IComposerConfiguration = {
        controllers: []
    };
    // Visit http://jasmine.github.io for more information on unit testing with Jasmine.
    // For more info on the Karma test runner, visit http://karma-runner.github.io

    describe('When the controller factory creates a controller', () => {
        var controllerRegistry: Orckestra.Composer.ControllerRegistry,
            controller: Orckestra.Composer.IController,
            controllerName: string;

        beforeEach(() => {
            controllerName = 'mock';
            controllerRegistry = new Orckestra.Composer.ControllerRegistry();
            controllerRegistry.register(controllerName, Orckestra.Composer.Mocks.MockController);
            controller = Orckestra.Composer.ControllerFactory
                .createController({
                    controllerName: controllerName,
                    context: Orckestra.Composer.Mocks.MockControllerContext,
                    eventHub: Orckestra.Composer.EventHub.instance(),
                    composerContext: composerContext,
                    composerConfiguration: composerConfiguration
                });
        });

        afterEach(() => {
            controllerRegistry.unregister(controllerName);
        });

        it('should not be null', () => {
            expect(controller).not.toBeNull();
        });

        it('should not be undefined', () => {
            expect(controller).not.toBeUndefined();
        });
    });
})();
