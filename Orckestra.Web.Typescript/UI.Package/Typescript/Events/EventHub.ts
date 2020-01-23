/// <reference path='./ISubscription.ts' />
/// <reference path='./IListener.ts' />
/// <reference path='./IListenerQueue.ts' />
/// <reference path='./IEventInformation.ts' />
/// <reference path='./IEventHub.ts' />
/// <reference path='../Generics/Collections/IHashTable.ts' />

module Orckestra.Composer {
    /**
     * An event hub for subscribing to and publishing events.
    */
    export class EventHub implements Orckestra.Composer.IEventHub {
        private _events: Orckestra.Composer.IHashTable<Orckestra.Composer.IListenerQueue> = {};
        private static _instance: Orckestra.Composer.IEventHub = new EventHub();

        public static instance(): Orckestra.Composer.IEventHub {
            return EventHub._instance;
        }

        constructor() {
            if (EventHub._instance) {
                throw new Error('Error: Instantiation failed: Use EventHub.instance() instead of new.');
            }

            EventHub._instance = this;
        }

        /**
         * Subscribe to an event.
         *
         * @param eventName The name of the event to subscribe to.
         * @param listener The listener subscribing to the event.
        */
        subscribe(eventName: string, listener: IListener): ISubscription {
            var index: number;

            if (!this._events.hasOwnProperty(eventName)) {
                this._events[eventName] = { queue: [] };
            }

            index = this._events[eventName].queue.push(listener) - 1;

            return ((topic, index) => {
                return {
                    remove: () => {
                        delete this._events[eventName].queue[index];
                    }
                };
            })(eventName, index);
        }

        /**
         * Publishes an event.
         *
         * @param eventName The name of the event to subscribe to.
         * @param eventInformation The information to provide all subscribers when the event is published.
        */
        publish(eventName: string, eventInformation: IEventInformation): void {
            if (!this._events.hasOwnProperty(eventName)) {
                return;
            }

            this._events[eventName].queue.forEach(function (listener: IListener) {

                if (listener !== void 0) {
                    listener(eventInformation);
                }
            });
        }
    }
}
