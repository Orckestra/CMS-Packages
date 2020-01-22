///<reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    export interface UIBusyParam {
        /*
         * Custom UI busy function (optional). This overides the out of the box UI busy effect.
         */
        callback?: Function;

        /*
         * Element responsible for the busy state (optional)
         * Usually the button clicked
         */
        elementContext?: JQuery;

        /*
         * The root container to disable (optional)
         * Usually the blade containing the button
         */
        containerContext?: JQuery;

        /*
         * dom selector to find the loading indicator relative to theelementContext
         */
        loadingIndicatorSelector?: string;

        /*
         * delay in millisecond before displaying the loading indicator
         */
        msDelay?: number;
    }
}
