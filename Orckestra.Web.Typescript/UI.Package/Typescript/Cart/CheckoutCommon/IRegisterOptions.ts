///<reference path='../../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    export interface IValidationCallback {
        (): Q.Promise<boolean>;
    }

    export interface IUpdateCallback {
        (): Q.Promise<any>;
    }

    export interface IRegisterOptions {
        validationCallback: Function;
        updateCallback: Function;
    }

    export interface IRegisterControlOptions extends IRegisterOptions {
        isReady: boolean;
    }
}
