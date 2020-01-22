///<reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    export class UIModal {

        private modalContext: JQuery;
        private confirmDeferred: Q.Deferred<any>;
        private confirmAction: any;
        private window: Window;
        private sender: any;
        private modalContextSelector: string;
        private container: JQuery;

        public constructor(window: Window, modalContextSelector: string, confirmAction, sender, container = undefined) {

            this.confirmAction = confirmAction;
            this.modalContextSelector = modalContextSelector;
            this.window = window;
            this.sender = sender;
            this.container = container;
            this.registerDomEvents(container);
        }

        private registerDomEvents(container) : void {
            if ( container === undefined ) {
                $(this.window.document).on('click', '.modal--confirm',  this.confirmModal.bind(this));
                $(this.window.document).on('click', '.modal--cancel', this.cancelModal.bind(this));
            } else {
                container.on('click', '.modal--confirm',  this.confirmModal.bind(this));
                container.on('click', '.modal--cancel', this.cancelModal.bind(this));
            }
        }

        private unregisterDomEvents() : void {
            $(this.window.document).off('click', '.modal--confirm', this.confirmModal);
            $(this.window.document).off('click', '.modal--cancel', this.cancelModal);
        }

        public openModal = (event: JQueryEventObject) => {

            this.modalContext = $(this.modalContextSelector);
            this.confirmDeferred = Q.defer();

            this.modalContext.on('shown.bs.modal', (event: JQueryEventObject) => {
                $('[data-dismiss]', event.target).focus();
            });

            this.modalContext.on('hide.bs.modal', (event: JQueryEventObject) => {
                $(event.target).off('shown.bs.modal hide.bs.modal');

                if (this.confirmDeferred.promise.isPending()) {
                    this.confirmDeferred.resolve(false);
                }
            });
            this.modalContext.modal('show');

            this.confirmDeferred.promise
                .then((value) => {
                    this.modalContext.modal('hide');

                    if (value) {
                        return this.confirmAction.call(this.sender, event);
                    }
                })
                .done(null, (error) => {
                    console.log(error);
                });
        };

        public confirmModal() {
            this.confirmDeferred.resolve(true);
        }

        public cancelModal() {
            this.confirmDeferred.resolve(false);
        }

        public dispose(): void {
            this.unregisterDomEvents();
        }
    }
}
