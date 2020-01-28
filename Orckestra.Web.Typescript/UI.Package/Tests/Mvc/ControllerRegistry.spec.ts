/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../../Typescript/Mvc/ControllerRegistry.ts' />
///<reference path='../Mocks/MockController.ts' />

'use strict';

// TODO: Move this in to it's own file once the composer.mocks.js is built.


(() => {
    // Visit http://jasmine.github.io for more information on unit testing with Jasmine.
    // For more info on the Karma test runner, visit http://karma-runner.github.io

    describe('When instantatiating the controller registry', () => {
        it('should always return the same instance no matter how many times we instantiate it.', () => {
            var registryInstance1: Orckestra.Composer.ControllerRegistry = new Orckestra.Composer.ControllerRegistry(),
                registryInstance2: Orckestra.Composer.ControllerRegistry = new Orckestra.Composer.ControllerRegistry();
            expect(registryInstance1).toEqual(registryInstance2);
        });
    });

    describe('When registering a controller in the controller registry', () => {
        var controllerRegistry: Orckestra.Composer.ControllerRegistry,
            controller: Orckestra.Composer.IController,
            controllerName: string;

        beforeEach(() => {
            controllerName = 'mock';
            controllerRegistry = new Orckestra.Composer.ControllerRegistry();
        });

        afterEach(() => {
            controllerRegistry.unregister(controllerName);
        });

        it('should throw an error if it is registered more than once.', () => {
            expect(() => {
                controllerRegistry.register(controllerName, Orckestra.Composer.Mocks.MockController);
                controllerRegistry.register(controllerName, Orckestra.Composer.Mocks.MockController);
            }).toThrowError(Error);
        });
    });

    describe('When unregistering a controller from the controller registry', () => {
        var controllerRegistry: Orckestra.Composer.ControllerRegistry,
            controller: Orckestra.Composer.IController,
            controllerName: string;

        beforeEach(() => {
            controllerName = 'mock';
            controllerRegistry = new Orckestra.Composer.ControllerRegistry();
        });

        afterEach(() => {
            if (controllerRegistry.isRegistered(controllerName)) {
                controllerRegistry.unregister(controllerName);
            }
        });

        it('should throw an error if it the controller was never registered.', () => {
            expect(() => {
                controllerRegistry.unregister(controllerName);
            }).toThrowError(Error);
        });

        it('should throw an error if it the controller is unregistered more than once.', () => {
            expect(() => {
                controllerRegistry.register(controllerName, Orckestra.Composer.Mocks.MockController);
                controllerRegistry.unregister(controllerName);
                controllerRegistry.unregister(controllerName);
            }).toThrowError(Error);
        });
    });

    describe('When a controller is registered in the controller registry', () => {
        var controllerRegistry: Orckestra.Composer.ControllerRegistry,
            controller: Orckestra.Composer.IController,
            controllerName: string;

        beforeEach(() => {
            controllerRegistry = new Orckestra.Composer.ControllerRegistry();
        });

        afterEach(() => {
            controllerRegistry.unregister(controllerName);
        });

        it('should return true if we check if it is registered.', () => {
            controllerName = 'mock';
            controllerRegistry.register(controllerName, Orckestra.Composer.Mocks);
            expect(controllerRegistry.isRegistered(controllerName)).toEqual(true);
        });
    });

    describe('When a controller is not registered in the controller registry', () => {
        var controllerRegistry: Orckestra.Composer.ControllerRegistry,
            controller: Orckestra.Composer.IController,
            controllerName: string;

        beforeEach(() => {
            controllerRegistry = new Orckestra.Composer.ControllerRegistry();
        });

        it('should return false if we check if it is registered.', () => {
            expect(controllerRegistry.isRegistered(controllerName)).toEqual(false);
        });
    });
})();
