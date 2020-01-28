///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='./RecurringScheduleDetailsController.ts' />
///<reference path='../../Composer.Cart/RecurringOrder/Services/RecurringOrderService.ts' />
///<reference path='../../Composer.Cart/RecurringOrder/Services/IRecurringOrderService.ts' />
///<reference path='../../Composer.Cart/RecurringOrder/Repositories/RecurringOrderRepository.ts' />
///<reference path='../../ErrorHandling/ErrorHandler.ts' />
///<reference path='../Common/DatepickerService.ts' />
///<reference path='../../Utils/Utils.ts' />

module Orckestra.Composer {

    export class MyRecurringScheduleDetailsController extends Orckestra.Composer.RecurringScheduleDetailsController {
        protected recurringOrderService: IRecurringOrderService = new RecurringOrderService(new RecurringOrderRepository(), this.eventHub);

        protected viewModelName = '';
        protected id = '';
        protected viewModel;
        protected modalElementSelector: string = '#confirmationModal';
        protected uiModal: UIModal;
        protected busyHandler: UIBusyHandle;
        protected window: Window;

        protected customerService: ICustomerService = new CustomerService(new CustomerRepository());
        protected recurringCartAddressRegisteredService: RecurringCartAddressRegisteredService =
            new RecurringCartAddressRegisteredService(this.customerService);

        public initialize() {

            super.initialize();
            this.viewModelName = 'MyRecurringScheduleDetails';
            this.window = window;

            this.getRecurringTemplateDetail();
            this.uiModal = new UIModal(window, this.modalElementSelector, this.deleteAddress, this);
        }

        public getRecurringTemplateDetail() {

            var nameUrlQueryString: string = 'id=';
            var id: string = '';

            if (window.location.href.indexOf(nameUrlQueryString) > -1) {
                id = window.location.href.substring(window.location.href.indexOf(nameUrlQueryString)
                    + nameUrlQueryString.length);
            }

            this.recurringOrderService.getRecurringTemplateDetail(id)
                .then(result => {
                    //console.log(result);
                    this.viewModel = result;
                    this.id = id;
                    this.reRenderPage(result.RecurringOrderTemplateLineItemViewModels[0]);
                })
                .fail((reason) => {
                    console.error(reason);
                });
        }

        public getAvailableEditList() {
            this.getAddresses();
            this.getShippingMethodsList();
            this.getPaymentMethods();
        }

        public reRenderPage(vm) {
            //this.viewModel = vm;
            ErrorHandler.instance().removeErrors();
            this.render(this.viewModelName, vm);

            //Scroll to success message if visible
            Utils.scrollToElement( $('[data-templateid="RecurringScheduleDetailsUpdateSuccessful"]:has(div)'));
            this.getAvailableEditList();
            DatepickerService.renderDatepicker('.datepicker');
        }


        public renderShippingMethods(vm) {
            this.render('RecurringScheduleDetailsShippingMethods', vm);
        }
        public renderAddresses(vm) {
            this.render('RecurringScheduleDetailsAddresses', vm);
        }
        public renderPayment(vm) {
            this.render('RecurringScheduleDetailsPayments', vm);
        }

        public getAddresses() {
            this.recurringCartAddressRegisteredService.getRecurringTemplateAddresses(this.id)
                .then((addressesVm) => {
                    addressesVm.SelectedBillingAddressId = this.viewModel.RecurringOrderTemplateLineItemViewModels[0].BillingAddressId;
                    addressesVm.SelectedShippingAddressId = this.viewModel.RecurringOrderTemplateLineItemViewModels[0].ShippingAddressId;

                    addressesVm.UseShippingAddress = addressesVm.SelectedBillingAddressId === addressesVm.SelectedShippingAddressId;

                    //console.log(addressesVm);
                    this.renderAddresses(addressesVm);
                });
        }
        public getShippingMethodsList() {

            let shippingMethodName = this.viewModel.RecurringOrderTemplateLineItemViewModels[0].ShippingMethodName;
            this.getShippingMethods()
                .then(shippingMethods => {

                    if (!shippingMethods) {
                        throw new Error('No viewModel received');
                    }

                    if (_.isEmpty(shippingMethods.ShippingMethods)) {
                        throw new Error('No shipping method was found.');
                    }

                    let selectedShippingMethodName = shippingMethodName;
                    shippingMethods.ShippingMethods.forEach(shippingMethod => {

                        if (shippingMethod.Name === selectedShippingMethodName) {
                            shippingMethods.SelectedShippingProviderId = shippingMethod.ShippingProviderId;
                        }
                    });

                    var vm = {
                        ShippingMethods: shippingMethods,
                        SelectedMethod: selectedShippingMethodName
                    };

                    //console.log(vm);

                    this.renderShippingMethods(vm);
                });
        }

        public getShippingMethods() : Q.Promise<any> {
            return this.recurringOrderService.getOrderTemplateShippingMethods()
                    .fail((reason) => {
                        console.error('Error while retrieving shipping methods', reason);
                    });
        }

        public getPaymentMethods() {

            let busy = this.asyncBusy();
            this.renderPayment({ IsLoading: true });

            let data: IRecurringOrderGetTemplatePaymentMethods = {
                id: this.id
            };
            this.recurringOrderService.getTemplatePaymentMethods(data)
                .then(result => {

                    let selected = this.viewModel.RecurringOrderTemplateLineItemViewModels[0].PaymentMethodId;
                    result.SavedCreditCards.forEach(payment => {
                        payment.IsSelected = payment.Id === selected;
                    });

                    //console.log(result);

                    this.renderPayment(result);
                });


            busy.done();
        }

        public useShippingAddress() : Boolean {
            var useShippingAddress = $(this.context.container).find('input[name=UseShippingAddress]:checked').val() === 'true';
            return useShippingAddress;
        }


        public changeUseShippingAddress() {

            this.setBillingAddressFormVisibility();
            this.setSelectedBillingAddress();
            //TODO: form validation?
        }

        public setBillingAddressFormVisibility() {

            var useShippingAddress: Boolean = this.useShippingAddress();
            if (useShippingAddress) {
                $('#BillingAddressContent').addClass('hide');
            } else {
                $('#BillingAddressContent').removeClass('hide');
            }
        }

        protected setSelectedBillingAddress() {

            var selectedBillingAddressId: string = $(this.context.container).find('input[name=BillingAddressId]:checked').val();

            if (!selectedBillingAddressId) {
                return;
            }
        }

        public deleteAddressConfirm(actionContext: IControllerActionContext) {
            this.uiModal.openModal(actionContext.event);
        }

          /**
         * Requires the element in action context to have a data-address-id.
         */
        protected deleteAddress(event: JQueryEventObject): Q.Promise<void> {

            let element = $(event.target);
            var $addressListItem = element.closest('[data-address-id]');
            var addressId = $addressListItem.data('address-id');

            var busy = this.asyncBusy({elementContext: element, containerContext: $addressListItem });

            return this.customerService.deleteAddress(addressId, '')
                .then(result => {
                    this.reRenderPage(this.viewModel);
                })
                .fin(() => busy.done());
        }

        protected releaseBusyHandler(): void {
            if (this.busyHandler) {
                this.busyHandler.done();
                this.busyHandler = null;
            }
        }

        public saveRecurringOrderTemplate(actionContext: IControllerActionContext): Q.Promise<void> {

            let lineItemId = this.id;
            let paymentMethodId;
            let shippingAddressId;
            let billingAddressId;
            let nextOccurence;
            let frequencyName;
            let shippingProviderId;
            let shippingMethodName;
            let isAllValid = true;

            //If save successful is still showing
            $('[data-templateid="RecurringScheduleDetailsUpdateSuccessful"]').hide();

            paymentMethodId = $(this.context.container).find('input[name=PaymentMethod]:checked').val();
            shippingAddressId = $(this.context.container).find('input[name=ShippingAddressId]:checked').val();
            billingAddressId = $(this.context.container).find('input[name=BillingAddressId]:checked').val();

            if (this.useShippingAddress()) {
                billingAddressId = shippingAddressId;
            }

            let element = <HTMLInputElement>$('#NextOcurrence')[0];
            let newDate = element.value;
            let isValid = this.nextOcurrenceIsValid(newDate);

            if (isValid) {
                nextOccurence = newDate;
            } else {
                isAllValid = false;
                console.error('Error: Invalid date while saving template');
                ErrorHandler.instance().outputErrorFromCode('InvalidDateSelected');
            }

            let frequency: any = $('#modifyFrequency').find(':selected')[0];
            frequencyName = frequency.value;
            if (frequencyName === '' || frequencyName === undefined) {
                isAllValid = false;
                console.error('Error: Invalid frequency while saving template');
                ErrorHandler.instance().outputErrorFromCode('InvalidFrequencySelected');
            }

            let elementShipping = $('#ShippingMethod').find('input[name=ShippingMethod]:checked');
            shippingMethodName = elementShipping.val();
            shippingProviderId = elementShipping.data('shipping-provider-id');

            if (_.isUndefined(paymentMethodId)) {
                isAllValid = false;
                console.error('Error: Missing payment method');
                ErrorHandler.instance().outputErrorFromCode('RecurringSchedulePaymentMissing');
            }
            if (!this.useShippingAddress() && _.isUndefined(shippingAddressId)) {
                isAllValid = false;
                console.error('Error: Missing shipping address');
                ErrorHandler.instance().outputErrorFromCode('RecurringScheduleShippingAddressMissing');
            }
            if (_.isUndefined(billingAddressId)) {
                isAllValid = false;
                console.error('Error: Missing billing address');
                ErrorHandler.instance().outputErrorFromCode('RecurringScheduleBillingAddressMissing');
            }
            if (_.isUndefined(shippingProviderId) || _.isUndefined(shippingMethodName)) {
                isAllValid = false;
                console.error('Error: Missing shipping provider or shipping method name');
                ErrorHandler.instance().outputErrorFromCode('RecurringScheduleBillingAddressMissing');
            }

            if (isAllValid) {

                this.busyHandler = this.asyncBusy({ elementContext: actionContext.elementContext });
                let updateTemplateLineItemParam: IRecurringOrderTemplateLineItemUpdateParam = {

                    nextOccurence: nextOccurence,
                    lineItemId: lineItemId,
                    billingAddressId: billingAddressId,
                    shippingAddressId: shippingAddressId,
                    paymentMethodId: paymentMethodId,
                    frequencyName: frequencyName,
                    shippingProviderId: shippingProviderId,
                    shippingMethodName: shippingMethodName
                };

                this.recurringOrderService.updateTemplateLineItem(updateTemplateLineItemParam)
                    .then(result => {

                        var templateLineItems = _.map(result.RecurringOrderTemplateViewModelList, (x: any) => {
                        return x.RecurringOrderTemplateLineItemViewModels;
                        });

                        var templateLineItemsList = _.reduce(templateLineItems,  function(a, b){ return a.concat(b); }, []);
                        var item = templateLineItemsList.filter(u => u.Id === this.id);

                        var vm = {
                            RecurringOrderTemplateLineItemViewModels: item
                        };

                        this.viewModel = vm;
                        item[0].UpdateStatus = 'Success';
                        this.reRenderPage(item[0]);
                    })
                    .fail((reason) => {
                        console.error(reason);
                        ErrorHandler.instance().outputErrorFromCode('RecurringScheduleUpdateFailed');
                    })
                    .fin(() => this.releaseBusyHandler());
            }
            return null;
        }

        public nextOcurrenceIsValid(value) {
            let newDate = this.convertDateToUTC(new Date(value));
            let today = this.convertDateToUTC(new Date(new Date().setHours(0, 0, 0, 0)));

            if (newDate > today) {
                return true;
            }
            return false;
        }

        public convertDateToUTC(date) {
            return new Date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(),
            date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds());
        }
    }
}
