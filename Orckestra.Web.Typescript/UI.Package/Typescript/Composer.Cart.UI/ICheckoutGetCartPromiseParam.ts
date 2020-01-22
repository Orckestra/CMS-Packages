///<reference path='./ICheckoutGetCartPromiseFailureHandler.ts' />

module Orckestra.Composer {
    'use strict';

    /**
     * @param {number} targetedStep The current checkout step.
     * @param {ICheckoutGetCartPromiseFailureHandler} failureHandler Handles failure of the AJAX request. This handler will only
     * @param {boolean} forceGet If true, forces the get of a new promise instead of the cached one.
     *                          be called if this is the first call.
     */
    export interface ICheckoutGetCartPromiseParam {
        targetedStep: number;
        forceGet?: boolean;
    }
}
