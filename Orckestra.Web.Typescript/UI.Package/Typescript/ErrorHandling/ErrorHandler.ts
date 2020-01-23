/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='./IErrorCollection.ts' />
/// <reference path='./IError.ts' />
/// <reference path='./IErrorHandler.ts' />
/// <reference path='../Mvc/Localization/LocalizationProvider.ts' />
/// <reference path='../Events/EventHub.ts' />

module Orckestra.Composer {
    export class ErrorHandler implements IErrorHandler {

        public static instance(): IErrorHandler {
            var instance: IErrorHandler = new ErrorHandler();

            var memoize = () => instance;
            ErrorHandler.instance = memoize;
            return memoize();
        }

        /**
         * Will output the error to the user.
         * @param {IError} error Error to display.
         */
        public outputError(error: IError): void {
            this.publishGenericErrorEvent(error);
        }

        /**
         * Will localize an error based on its code and will display it
         * to the user.
         * @param {string} errorCode Error code to localize.
         */
        public outputErrorFromCode(errorCode: string): void {
            var error = this.createErrorFromCode(errorCode);

            this.publishGenericErrorEvent(error);
        }

        protected createErrorFromCode(errorCode: string): IError {
            var localization: string = LocalizationProvider.instance().getLocalizedString('Errors', `L_${errorCode}`);

            var error: IError = {
                ErrorCode: errorCode,
                LocalizedErrorMessage: localization
            };

            return error;
        }

        /**
         * Removes all errors from the current page.
         */
        public removeErrors(): void {
            this.publishGenericErrorEvent();
        }

        protected publishGenericErrorEvent(error?: IError): void {
            var errorColl = this.createErrorCollection(error);

            EventHub.instance().publish('GeneralErrorOccured', {
                data: errorColl
            });
        }

        protected createErrorCollection(error?: IError): IErrorCollection {
            var errorCollection = {
                Errors: []
            };

            if (error) {
                errorCollection.Errors.push(error);
            }

            return errorCollection;
        }
    }
}