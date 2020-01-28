///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../../Typescript/Events/EventHub.ts' />

(() => {

    'use strict';

    // Visit http://jasmine.github.io for more information on unit testing with Jasmine.
    // For more info on the Karma test runner, visit http://karma-runner.github.io
    var someEventName: string = 'someEvent',
        dummyData: string = 'dummy data';

    describe('When instantiating the EventHub with a constructor', () => {
        var eventHub: Orckestra.Composer.IEventHub;

        it('should throw an error', () => {
            expect(() => {
                eventHub = new Orckestra.Composer.EventHub();
            }).toThrowError(Error);
        });
    });

    describe('When a listener subscribes to an event', () => {
        var eventHub: Orckestra.Composer.IEventHub,
            listener: Orckestra.Composer.IListener,
            spy: SinonSpy;

        beforeEach(() => {
            eventHub = Orckestra.Composer.EventHub.instance();
            spy = sinon.spy();
        });

        it('should be notified when an event publishes', () => {
            listener = (eventInformation: Orckestra.Composer.IEventInformation) => {
                spy();
            };

            eventHub.subscribe(someEventName, listener);

            eventHub.publish(someEventName, {
                source: someEventName,
                data: {}
            });

            expect(spy.called).toBe(true);
        });

        it('should be notified when an event publishes and receive the data of the event in the event information', () => {
            var data: any;

            listener = (eventInformation: Orckestra.Composer.IEventInformation) => {
                data = eventInformation.data;
            };

            eventHub.subscribe(someEventName, listener);

            eventHub.publish(someEventName, {
                source: someEventName,
                data: dummyData
            });

            expect(data).toBe(dummyData);
        });
    });

    describe('When a listener does not subscribe to an event', () => {
        var eventHub: Orckestra.Composer.IEventHub,
            listener: Orckestra.Composer.IListener,
            spy: SinonSpy;

        beforeEach(() => {
            eventHub = Orckestra.Composer.EventHub.instance();
            spy = sinon.spy();
            listener = (eventInformation: Orckestra.Composer.IEventInformation) => {
                spy();
            };
        });

        it('should not be notified when an event publishes', () => {
            eventHub.publish(someEventName, {
                source: someEventName,
                data: {}
            });

            expect(spy.called).toBe(false);
        });
    });

    describe('When a listener subscribes to an event and then unsubscribes', () => {
        var eventHub: Orckestra.Composer.IEventHub,
            listener: Orckestra.Composer.IListener,
            spy: SinonSpy,
            subscription: Orckestra.Composer.ISubscription;

        beforeEach(() => {
            eventHub = Orckestra.Composer.EventHub.instance();
            spy = sinon.spy();
            listener = (eventInformation: Orckestra.Composer.IEventInformation) => {
                spy();
            };

            subscription = eventHub.subscribe(someEventName, listener);
        });

        it('should not be notified when an event publishes if it unsubscribed from the event before it published', () => {
            subscription.remove();
            eventHub.publish(someEventName, {
                source: someEventName,
                data: {}
            });

            expect(spy.called).toBe(false);
        });

        it('should be notified when an event publishes if it unsubscribed after the event published', () => {
            subscription.remove();
            eventHub.publish(someEventName, {
                source: someEventName,
                data: {}
            });

            expect(spy.called).toBe(false);
        });
    });
})();
