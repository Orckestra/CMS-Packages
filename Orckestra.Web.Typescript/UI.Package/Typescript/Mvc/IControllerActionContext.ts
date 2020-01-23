/// <reference path="../../Typings/tsd.d.ts" />

module Orckestra.Composer {
    'use strict';

    /**
    * All the contextual information required for an action to execute.
    */
    export interface IControllerActionContext {
        /**
        * A DOM element requesting the action to execute.
        */
        elementContext: JQuery;

        /**
        * A DOM event that triggers the action to execute.
        */
        event: JQueryEventObject;
    }
}

