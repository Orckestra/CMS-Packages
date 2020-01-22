///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Composer.UI/IEventHub.ts' />
///<reference path='../Composer.UI/ComposerClient.ts' />
///<reference path='./IFindOrderService.ts' />
///<reference path='./IGetOrderDetailsUrlRequest.ts' />
///<reference path='./IGuestOrderDetailsViewModel.ts' />

module Orckestra.Composer {
    export class FindOrderService implements IFindOrderService {
        private eventHub: IEventHub;

        public constructor(eventHub: IEventHub) {
            this.eventHub = eventHub;
        }

        public getOrderDetailsUrl(req: IGetOrderDetailsUrlRequest): Q.Promise<IGuestOrderDetailsViewModel> {
            var promise = ComposerClient.post('/api/order/url', req)
                .then((vm: IGuestOrderDetailsViewModel) => {
                    this.eventHub.publish('orderDetailsUrlUpdated', {
                        data: vm
                    });

                    return vm;
                });

            return promise;
        }
    }
}
