///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Events/EventHub.ts' />
///<reference path='../../Mvc/Controller.ts' />

module Orckestra.Composer {
    export class AddToCartNotificationController extends Controller {
        public initialize() {
            super.initialize();
            this.registerSubscriptions();
        }

        private registerSubscriptions(): void {
            this.eventHub.subscribe('lineItemAddedToCart', (e: IEventInformation) => {
                this.displayNotification(e);
            });
        }

        public displayNotification(e: IEventInformation): void {
            let notificationContainer = $(this.context.container),
                notificationTime = notificationContainer.data('notificationTime'),
                cart = e.data.Cart;

            if (notificationTime > 0) {
                this.render('AddToCartNotificationModal', cart);

                notificationContainer.removeClass('hidden');

                setTimeout(() => {
                    this.closeNotification();
                }, parseInt(notificationTime, 10));
            }
        }

        public onClose(e: IControllerActionContext): void {
            e.event.preventDefault();
            this.closeNotification();
        }

        private closeNotification(): void {
            $(this.context.container).addClass('hidden');
        }
    }
}
