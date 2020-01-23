///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
///<reference path='../Common/CustomerService.ts' />
///<reference path='../Common/MyAccountEvents.ts' />
///<reference path='../Common/MyAccountStatus.ts' />
///<reference path='../MyAccount/MyAccountController.ts' />
///<reference path='../../UI/UIModal.ts' />

module Orckestra.Composer {

    //TODO refactor modal : create a generic modal service
    export class AddressListController extends Orckestra.Composer.MyAccountController {

        private deleteModalElementSelector: string= '#confirmationModal';
        private uiModal: UIModal;

        protected customerService: ICustomerService = new CustomerService(new CustomerRepository());

        public initialize() {

            super.initialize();

            this.uiModal = new UIModal(window, this.deleteModalElementSelector, this.deleteAddress, this);

            this.registerSubscriptions();
        }

        protected registerSubscriptions() {

            this.eventHub.subscribe(MyAccountEvents[MyAccountEvents.AddressDeleted], e => this.onAddressDeleted(e));
        }

        private onAddressDeleted(e: IEventInformation) {

            var result = e.data;
            var $container = result.$container;
            $container.remove();
        }

        /**
        * Requires the element in action context to have a data-address-id.
        */
        public setDefaultAddress(actionContext: IControllerActionContext): void {

            var $addressListItem: JQuery = $(actionContext.elementContext).closest('[data-address-id]');
            var addressId = $addressListItem.data('address-id');

            var busy = this.asyncBusy({ elementContext: actionContext.elementContext, containerContext: $addressListItem });

            this.customerService.setDefaultAddress(addressId.toString(), '')
                .then(result => location.reload(), reason => console.error(reason))
                .fin(() => busy.done())
                .done();
        }

        /**
        * Requires the element in action context to have a data-address-id.
        */
        public deleteAddress(event: JQueryEventObject): void {
            let element = $(event.target);
            var $addressListItem: JQuery = element.closest('[data-address-id]');
            var addressId = $addressListItem.data('address-id');

            var busy = this.asyncBusy({ elementContext: element, containerContext: $addressListItem });

            this.customerService.deleteAddress(addressId, '')
                .then(result => this.onDeleteAddressFulfilled(result, $addressListItem), reason => console.error(reason))
                .fin(() => busy.done())
                .done();
        }

        private onDeleteAddressFulfilled(result: any, $addressListItem: JQuery): void {

            var data = {
                result: result,
                $container: $addressListItem
            };

            this.eventHub.publish(MyAccountEvents[MyAccountEvents.AddressDeleted], { data: data });
        }

        public deleteAddressConfirm(actionContext: IControllerActionContext) {
             this.uiModal.openModal(actionContext.event);
        }
    }
}
