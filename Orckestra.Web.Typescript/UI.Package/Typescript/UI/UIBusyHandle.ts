///<reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    export class UIBusyHandle {

        protected loadingIndicatorContext: JQuery;
        protected containerContext: JQuery;
        protected timeoutHandle;

        /*
         * Start the busy state, to be called before the async call
         * @param loadingIndicatorContext JQuery element of what the handler will remove/add class "hidden".
         * @param containerContext JQuery element containing inputs that will be disabled while busy.
         * @param msDelay Number of ms before activating async. If done is called before the end
         *        of the delay, the loadingIndicatorContext will not be shown.
         */
        public constructor(loadingIndicatorContext: JQuery, containerContext: JQuery, msDelay: number) {

            this.loadingIndicatorContext = loadingIndicatorContext;
            this.containerContext = containerContext;

            this.startBusy(msDelay);
        }

        /*
         * Ends the busy state, to be called in the then of the async call
         */
        public done() {
            this.endBusy();
        }

        private startBusy(msDelay: number) {

            this.timeoutHandle = setTimeout(() => {

                this.containerContext.find(':input:enabled').addClass('async-busy').prop('disabled', true);
                this.loadingIndicatorContext.removeClass('hidden');

            }, msDelay);
        }

        private endBusy() {

            clearTimeout(this.timeoutHandle);

            this.loadingIndicatorContext.addClass('hidden');
            this.containerContext.find(':input.async-busy').removeClass('async-busy').prop('disabled', false);
        }
    }
}
