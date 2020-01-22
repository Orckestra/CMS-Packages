/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='./IError.ts' />

module Orckestra.Composer {
    export interface IErrorHandler {

        /**
         * Will output the error to the user.
         * @param {IError} error Error to display.
         */
        outputError(error: IError): void;

        /**
         * Will localize an error based on its code and will display it
         * to the user.
         * @param {string} errorCode Error code to localize.
         */
        outputErrorFromCode(errorCode: string): void;

        /**
         * Removes all errors from the current page.
         */
        removeErrors(): void;
    }
}
