///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
///<reference path='../../Utils/UrlHelper.ts' />
///<reference path='../../JQueryPlugins/IParsleyJqueryPlugin.ts' />
///<reference path='../../Validation/IParsley.ts' />
///<reference path='../Common/CustomerService.ts' />
///<reference path='../Common/MyAccountEvents.ts' />
///<reference path='../MyAccount/MyAccountController.ts' />

module Orckestra.Composer {

    export class EditAddressController extends Orckestra.Composer.MyAccountController {

        protected _formInstances: IParsley[];
        protected customerService: ICustomerService = new CustomerService(new CustomerRepository());

        public initialize() {

            super.initialize();
            this.registerSubscriptions();

            var busy = this.asyncBusy({ msDelay: 300, loadingIndicatorSelector: '.loading-indicator-regions' });

            ComposerClient.get('/api/address/regions')
                .then(regions => this.rebuildRegionSelector(regions))
                .done(() => busy.done());
        }

        protected registerSubscriptions() {

            this.registerFormsForValidation(this.context.container.find('form'));
            this.eventHub.subscribe(MyAccountEvents[MyAccountEvents.AddressCreated], e => this.onAddressCreatedOrUpdated(e));
            this.eventHub.subscribe(MyAccountEvents[MyAccountEvents.AddressUpdated], e => this.onAddressCreatedOrUpdated(e));
        }

        private onAddressCreatedOrUpdated(e: IEventInformation) {

            var result = e.data;

            if (result.ReturnUrl) {
                window.location.replace(decodeURIComponent(result.ReturnUrl));
            } else {

                this.render('EditAddress', result);
                this.registerFormsForValidation(this.context.container.find('form'), {
                    serverValidationContainer: '[data-templateid="FormErrorMessages"]'
                });
            }
        }

        /**
         * Rerender the region selector, keeping the currently selected value
         */
        private rebuildRegionSelector(regions: any) {

            var selectedRegion: string = this.context.container.find('[data-templateid="AddressRegionPicker"]').val();
            this.render('AddressRegionPicker', { Regions: regions, SelectedRegion : selectedRegion});
        }

        public adjustPostalCode(actionContext: IControllerActionContext): void {

            actionContext.elementContext.val(actionContext.elementContext.val().toUpperCase());
            _.all(this._formInstances, formInstance => formInstance.validate('shipping-based-on', true));
        }

        public createAddress(actionContext: IControllerActionContext): void {

            actionContext.event.preventDefault();

            var formData: any = this.getFormData(actionContext);
            var returnUrlQueryString: string = 'ReturnUrl=';
            var returnUrl: string = '';

            if (window.location.href.indexOf(returnUrlQueryString) > -1) {
                returnUrl = urlHelper.getURLParameter(location.search, 'ReturnUrl');
            }

            var busy = this.asyncBusy({elementContext: actionContext.elementContext});

            this.customerService.createAddress(formData, returnUrl)
                .then(result => this.onCreateAddressFulfilled(result), reason => this.renderFormErrorMessages(reason))
                .fin(() => busy.done())
                .done();
        }

        private onCreateAddressFulfilled(result: any): void {

            this.eventHub.publish(MyAccountEvents[MyAccountEvents.AddressCreated], { data: result });
        }

        public updateAddress(actionContext: IControllerActionContext): void {

            actionContext.event.preventDefault();

            var formData: any = this.getFormData(actionContext);
            var addressId: string = this.context.container.find('[data-address-id]').data('address-id').toString();
            var returnUrlQueryString: string = 'ReturnUrl=';
            var returnUrl: string = '';

            if (window.location.href.indexOf(returnUrlQueryString) > -1) {
                returnUrl = urlHelper.getURLParameter(location.search, 'ReturnUrl');
            }

            var busy = this.asyncBusy({elementContext: actionContext.elementContext});

            this.customerService.updateAddress(formData, addressId, returnUrl)
                .then(result => this.onUpdateAddressFulfilled(result), reason => this.renderFormErrorMessages(reason))
                .fin(() => busy.done())
                .done();
        }

        private onUpdateAddressFulfilled(result: any): void {

            this.eventHub.publish(MyAccountEvents[MyAccountEvents.AddressUpdated], { data: result });
        }
    }
}
