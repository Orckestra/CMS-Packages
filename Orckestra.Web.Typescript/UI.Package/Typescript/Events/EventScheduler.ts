///<reference path='../../Typings/tsd.d.ts' />
///<reference path='./IScheduledCallback.ts' />
///<reference path='./EventHub.ts' />

module Orckestra.Composer {
    /**
     * Class in charge of scheduling multiple calls to an event.
     * This class is a multiton. Please use the instance() method to get an instance.
     */
    export class EventScheduler {
        private static instances: IHashTable<EventScheduler> = {};

        private eventName: string;
        private onEventCallbacks: IScheduledCallback[];
        private postEventCallback: IScheduledCallback;

        /**
         * Get an instance of the EventScheduler for a specific event.
         * @param eventName The name of the event to listen to.
         */
        public static instance(eventName: string): EventScheduler {
            var instance: EventScheduler = EventScheduler.instances[eventName];

            if (!instance) {
                instance = new EventScheduler(eventName);
                EventScheduler.instances[eventName] = instance;
            }

            return instance;
        }

        /**
         * Constructor. Should be used for testing purposes and inside the multiton only.
         * @param eventName The name of the event to subscribe to.
         */
        constructor(eventName: string) {
            var instance: EventScheduler = EventScheduler.instances[eventName];

            if (instance) {
                throw new Error('Error: Instantiation failed: Use EventScheduler.instance(eventName: string) instead of new.');
            }

            this.eventName = eventName;
            this.onEventCallbacks = [];

            EventHub.instance().subscribe(this.eventName, (e: IEventInformation) => this.trigger(e.data));
        }

        /**
         * Subscribes a callback to the EventScheduler.
         * @param callback Function to call when the event arises. Must return a promise.
         */
        public subscribe(callback: IScheduledCallback): void {
            this.onEventCallbacks.push(callback);
        }

        /**
         * Sets the callback method that will be invoked after all the others are done executing.
         * @param postEventCallback Function to call when all other callbacks have been executed.
         */
        public setPostEventCallback(postEventCallback: IScheduledCallback): void {
            this.postEventCallback = postEventCallback;
        }

        private trigger(data: any): void {
            var promise: Q.Promise<any> = this.triggerCallbacks(data);

            promise
                .then((data: any) => this.triggerPostEvent(data))
                .done(() => console.log(`Event '${this.eventName}' fulfilled by the Event Schedule successfully.`),
                    (reason: any) => this.onError(reason));
        }

        private triggerCallbacks(data: any): Q.Promise<any> {
            var promise: Q.Promise<any>;

            if (_.isEmpty(this.onEventCallbacks)) {
                promise = Q(data);
            } else {
               var promises: Q.Promise<any>[] = _.map(this.onEventCallbacks, (callback: IScheduledCallback) => callback(data));

               promise = Q.all(promises)
                .then((values: any[]) => {
                    return data;
                });
            }

            return promise;
        }

        private triggerPostEvent(data: any): Q.Promise<any> {
            var promise: Q.Promise<any>;

            if (this.postEventCallback) {
                promise = this.postEventCallback(data);
            } else {
                promise = Q(data);
            }

            return promise;
        }

        /**
         * Gets invoked when an error occurs while executing the promises chain.
         */
        protected onError(reason: any): void {
            console.error(`An error occured while processing the event '${this.eventName}' with the EventScheduler.`, reason);
        }
    }
}
