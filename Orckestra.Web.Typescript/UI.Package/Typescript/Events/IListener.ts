module Orckestra.Composer {
    /**
     * A listener that can subscribe to an event.
    */
    export interface IListener {
        /**
         * Signature for a listener that is subscribed to an event.
        */
        (eventInformation: IEventInformation): void;
    }
}
