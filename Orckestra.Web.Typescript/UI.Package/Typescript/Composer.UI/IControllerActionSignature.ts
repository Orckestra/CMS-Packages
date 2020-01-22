///<reference path='./IControllerActionContext.ts' />

module Orckestra.Composer {
    'use strict';

    /**
    * Signature for executing a function that requires a controller action context.
    */
    export interface IControllerActionSignature {
        (options: Composer.IControllerActionContext): void;
    }
}
