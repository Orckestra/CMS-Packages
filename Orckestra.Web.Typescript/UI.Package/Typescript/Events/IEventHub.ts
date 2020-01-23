/// <reference path='./ISubscription.ts' />
/// <reference path='./IListener.ts' />
/// <reference path='./IListenerQueue.ts' />
/// <reference path='./IEventInformation.ts' />
/// <reference path='../Generics/Collections/IHashTable.ts' />

module Orckestra.Composer {
    /**
     * An event hub for subscribing to and publishing events.
    */
    export interface IEventHub {
        /**
         * Subscribe to an event.
         *
         * @param eventName The name of the event to subscribe to.
         * @param listener The listener subscribing to the event.
        */
        subscribe(eventName: string, listener: IListener): ISubscription;

        /**
         * Publishes an event.
         *
         * @param eventName The name of the event to subscribe to.
         * @param eventInformation The information to provide all subscribers when the event is published.
        */
        publish(eventName: string, eventInformation: IEventInformation): void;
    }
}
