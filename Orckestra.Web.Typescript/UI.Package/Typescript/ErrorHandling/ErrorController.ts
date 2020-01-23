/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../Mvc/Controller.ts' />
/// <reference path='./IErrorCollection.ts' />

module Orckestra.Composer {
    export class ErrorController extends Controller {
        private lastErrorCodes: string[];

        public initialize() {
            this.subscribeToEvents();
        }

        protected subscribeToEvents(): void {
            this.eventHub.subscribe('GeneralErrorOccured',
                (eventInfo: IEventInformation) => this.handleGeneralError(eventInfo.data, eventInfo.source));
        }

        protected handleGeneralError(errors: IErrorCollection, source: string) {
            var errorCodes: string[] = <string[]>_.map(errors.Errors, 'ErrorCode').sort();
            var lastErrorCodes = this.lastErrorCodes ? this.lastErrorCodes : [];
            var isMatch = _.isEqual(errorCodes, lastErrorCodes);

            if (!isMatch) {
                this.lastErrorCodes = errorCodes;
                this.render('FormErrorMessages', errors);
            }

            //Scroll to the error message if there's one
            if (errors && errors.Errors && errors.Errors.length > 0) {
                this.scrollToElement($('[data-templateid="FormErrorMessages"]'));
            }
        }

        protected scrollToElement(element: JQuery, offsetDiff: number = 100) {
            $('html, body').animate({
                scrollTop: $(element).offset().top - offsetDiff
            }, 10);
        }
    }
}