///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../../Typescript/Mvc/Controller.ts' />
///<reference path='../../Typescript/Events/EventHub.ts' />
///<reference path='../Mocks/MockController.ts' />
///<reference path='../Mocks/MockControllerContext.ts' />

(() => {
    'use strict';

    // Visit http://jasmine.github.io for more information on unit testing with Jasmine.
    // For more info on the Karma test runner, visit http://karma-runner.github.io
    var dummyActionName = 'dummy',
        dummyActionDelegate = () => {
            console.log('dummy controller action');
        };

    describe('Controller_registerAction.spect.ts', () => {
        describe('When registering an action that does not exist on the controller', () => {
            it('SHOULD add the action to the controller', () => {
                Orckestra.Composer.Controller.registerAction(Orckestra.Composer.Mocks.MockController, {
                    actionName: dummyActionName,
                    actionDelegate: dummyActionDelegate
                });

                expect(Orckestra.Composer.Mocks.MockController.prototype.hasOwnProperty(dummyActionName)).toBe(true);
                expect(_.isFunction(Orckestra.Composer.Mocks.MockController.prototype[dummyActionName])).toBe(true);
            });
        });

        describe('When registering an action that exists on the controller but is not set to overwrite', () => {
            it('SHOULD throw error', () => {
                expect(() => {
                    Orckestra.Composer.Controller.registerAction(Orckestra.Composer.Mocks.MockController, {
                        actionName: dummyActionName,
                        actionDelegate: dummyActionDelegate
                    });

                    Orckestra.Composer.Controller.registerAction(Orckestra.Composer.Mocks.MockController, {
                        actionName: dummyActionName,
                        actionDelegate: dummyActionDelegate
                    });
                }).toThrowError(Error);
            });
        });

        describe('When registering an action that exists on the controller and overwrite is set to true', () => {
            it('SHOULD not throw error', () => {
                expect(() => {
                    Orckestra.Composer.Controller.registerAction(Orckestra.Composer.Mocks.MockController, {
                        actionName: dummyActionName,
                        actionDelegate: dummyActionDelegate,
                        overwrite: true
                    });

                    Orckestra.Composer.Controller.registerAction(Orckestra.Composer.Mocks.MockController, {
                        actionName: dummyActionName,
                        actionDelegate: dummyActionDelegate,
                        overwrite: true
                    });
                }).not.toThrowError(Error);
            });
        });

        afterEach(() => {
            if (Orckestra.Composer.Mocks.MockController.prototype[dummyActionName] !== void 0) {
                delete Orckestra.Composer.Mocks.MockController.prototype[dummyActionName];
            }
        });
    });
})();
