module Orckestra.Composer {
    /**
     * A hook that allows a listener to unsubscribe from an event.
     */
    export interface ISubscription {
        /**
         * Removes a listener from an event.
         */
        remove(): void;
    }
}
