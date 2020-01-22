/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../Composer.UI/ISerializeObjectJqueryPlugin.ts' />
/// <reference path='../Composer.UI/Controller.ts' />
/// <reference path='../Composer.UI/IControllerContext.ts' />
/// <reference path='../Composer.UI/IControllerActionContext.ts' />
///<reference path='./OrderService.ts' />

module Orckestra.Composer {
    'use strict';

    export class PastOrdersController extends Controller {
        protected orderService = new OrderService();

        public initialize() {
            super.initialize();

            this.getPastOrders();
        }

        public getOrders(context: IControllerActionContext) {
            var page: number = context.elementContext.data('page');

            context.event.preventDefault();
            this.getPastOrders({ page: page });
        }

        private getPastOrders(param? : IGetOrderParameters) {
            var busyHandle = this.asyncBusy();

            this.orderService.getPastOrders(param)
                .done((viewModel) => {
                    this.render('PastOrders', viewModel);

                    if (!_.isEmpty(viewModel)) {
                        this.render('OrderHistoryPagination', viewModel.Pagination);
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
