module Orckestra.Composer {
    /**
     * Information that an event publishes to its subscribers.
    */
    export interface IEventInformation {
        /**
         * The data that an event is publishing to its subscribers.
        */
        data: any; // no choice but using. Events that publish may contain different data to provide to their subscribers

        /**
         * Source from which the data originates. (Optional)
         * @type {string}
         */
        source?: string;
    }
}
