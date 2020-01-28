/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../../Typescript/Composer.Product/ProductSearch/SortBySearchController.ts' />
/// <reference path='../../Mocks/MockControllerContext.ts' />
/// <reference path='../../Mocks/MockJqueryEventObject.ts' />
/// <reference path='../../../Typescript/Events/EventHub.ts' />
/// <reference path='../../../Typescript/Events/IEventHub.ts' />
/// <reference path='../../../Typescript/Events/IEventInformation.ts' />
/// <reference path='../../../Typescript/IComposerConfiguration.ts' />

(() => {
    'use strict';

    var composerContext: Orckestra.Composer.IComposerContext = {
        language: 'en-CA'
    };
    var composerConfiguration: Orckestra.Composer.IComposerConfiguration = {
        controllers: []
    };
    // Visit http://jasmine.github.io for more information on unit testing with Jasmine.
    // For more info on the Karma test runner, visit http://karma-runner.github.io
    var controller: Orckestra.Composer.SortBySearchController,
        eventHub: Orckestra.Composer.IEventHub,
        spy: SinonSpy,
        controllerActionContext: Orckestra.Composer.IControllerActionContext;

    describe('WHEN calling the SortBySearchController.sortingChanged method', () => {
        beforeEach(() => {
            spy = sinon.spy();
            eventHub = Orckestra.Composer.EventHub.instance();
            controller = new Orckestra.Composer.SortBySearchController(
                Orckestra.Composer.Mocks.MockControllerContext,
                eventHub,
                composerContext,
                composerConfiguration);
            controllerActionContext = {
                elementContext: $(''),
                event: Orckestra.Composer.Mocks.MockJqueryEventObject
            };
        });

        it('SHOULD publish the sortingChanged event.', () => {
            eventHub.subscribe('sortingChanged', (eventInformation: Orckestra.Composer.IEventInformation) => {
                spy();
            });

            controller.sortingChanged(controllerActionContext);
            expect(spy.called).toBe(true);
        });
    });
})();
