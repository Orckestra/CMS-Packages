module Orckestra.Composer {
    'use strict';

    /**
     * Signature for error handler of the checkout get cart promise.
     */
    export interface ICheckoutGetCartPromiseFailureHandler {
        (error: any): void;
    }
}
