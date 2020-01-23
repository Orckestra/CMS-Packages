///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
///<reference path='./IFindOrderService.ts' />
///<reference path='./FindOrderService.ts' />
///<reference path='./IGetOrderDetailsUrlRequest.ts' />
///<reference path='./IGuestOrderDetailsViewModel.ts' />
///<reference path='./IFindMyOrderViewModel.ts' />

module Orckestra.Composer {
    'use strict';

    export class FindMyOrderController extends Orckestra.Composer.Controller {
        private findOrderService: IFindOrderService;

        public initialize() {

           super.initialize();

           this.registerFormsForValidation(this.context.container.find('form'));

           this.findOrderService = new FindOrderService(this.eventHub);
        }

        public getWindow(): Window {
            return window;
        }

        public onFindMyOrder(actionContext: IControllerActionContext): void {
            actionContext.event.preventDefault();

            var busy: UIBusyHandle = this.asyncBusy();

            var request = <IGetOrderDetailsUrlRequest>(<ISerializeObjectJqueryPlugin>actionContext.elementContext).serializeObject();

            this.findOrderAsync(request)
                .then((vm: IGuestOrderDetailsViewModel) => {
                    this.getWindow().location.href = vm.Url;
                })
                .fail(reason => {
                    busy.done();

                    if (reason.status && reason.status === 404) {
                        this.handleOrderNotFound(reason, request);
                    } else {
                        console.error(reason);
                    }
                });
        }

        private findOrderAsync(request: IGetOrderDetailsUrlRequest): Q.Promise<IGuestOrderDetailsViewModel> {
            return this.findOrderService.getOrderDetailsUrl(request);
        }

        private handleOrderNotFound(reason: any, request: IGetOrderDetailsUrlRequest): void {
            var vm: IFindMyOrderViewModel = {
                Email: request.Email,
                OrderNumber: request.OrderNumber,
                OrderNotFound: true
            };

            this.render('FindMyOrder', vm);
        }
    }
}
