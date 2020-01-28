///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='./RecurringCartsController.ts' />
///<reference path='../../Composer.Cart/RecurringOrder/Services/RecurringOrderService.ts' />
///<reference path='../../Composer.Cart/RecurringOrder/Repositories/RecurringOrderRepository.ts' />

module Orckestra.Composer {

    export class MyRecurringCartsController extends Orckestra.Composer.RecurringCartsController {
        protected recurringOrderService: IRecurringOrderService = new RecurringOrderService(new RecurringOrderRepository(), this.eventHub);

        public initialize() {
            super.initialize();

            this.getUpcomingOrders();
        }

        public getUpcomingOrders() {
            var busyHandle = this.asyncBusy();

            this.recurringOrderService.getRecurringOrderCartsByUser()
                .done((viewModel) => {

                    if (!_.isEmpty(viewModel)) {
                        this.render('MyRecurringCarts', viewModel);
                    }

                    busyHandle.done();
                },
                    (reason) => {
                        console.error(reason);
                        busyHandle.done();
                    });
        }
    }
}
