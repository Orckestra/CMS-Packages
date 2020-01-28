///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='./RecurringCartDetailsController.ts' />
///<reference path='../../Composer.Cart/RecurringOrder/Services/RecurringOrderService.ts' />
///<reference path='../../Composer.Cart/RecurringOrder/Services/IRecurringOrderService.ts' />
///<reference path='../../Composer.Cart/RecurringOrder/Repositories/RecurringOrderRepository.ts' />
///<reference path='./RecurringCartAddressRegisteredService.ts' />
///<reference path='../../Composer.Store/Store/IStoreService.ts' />
///<reference path='../../Composer.Store/Store/StoreService.ts' />
///<reference path='../../UI/UIModal.ts' />
///<reference path='../../Composer.Cart/MonerisPaymentProvider/MonerisPaymentService.ts' />
///<reference path='../Common/MyAccountEvents.ts' />
///<reference path='../Common/DatepickerService.ts' />
///<reference path='../../Utils/Utils.ts' />

module Orckestra.Composer {

    export enum EditSection {
        NextOccurence = 0,
        ShippingMethod = 1,
        Address = 2,
        Payment = 3
    };

    //From Composer
    export enum ShippingMethodType {
        Unspecified = 0,
        PickUp = 1,
        Delivery = 2,
        Shipping = 3,
        ShipToStore = 4
    }

    export class MyRecurringCartDetailsController extends Orckestra.Composer.RecurringCartDetailsController {
        protected recurringOrderService: IRecurringOrderService = new RecurringOrderService(new RecurringOrderRepository(), this.eventHub);
        protected storeService: IStoreService = new StoreService();
        protected paymentService: MonerisPaymentService = new MonerisPaymentService();

        protected editNextOcurrence = false;
        protected editShippingMethod = false;
        protected editAddress = false;
        protected editPayment = false;
        protected originalShippingMethodType = '';
        protected hasShippingMethodTypeChanged = false;
        protected newShippingMethodType = undefined;
        protected viewModelName = '';
        protected viewModel;
        protected updateWaitTime = 300;
        protected modalElementSelector: string = '#confirmationModal';
        protected uiModal: UIModal;
        protected busyHandler: UIBusyHandle;
        protected window: Window;

        protected debounceUpdateLineItem: (args: any) => void;

        protected customerService: ICustomerService = new CustomerService(new CustomerRepository());
        protected recurringCartAddressRegisteredService: RecurringCartAddressRegisteredService =
            new RecurringCartAddressRegisteredService(this.customerService);

        public initialize() {

            super.initialize();

            this.viewModelName = 'MyRecurringCartDetails';

            //console.log(this.context.viewModel);

            this.getRecurringCart();
            this.uiModal = new UIModal(window, this.modalElementSelector, this.deleteAddress, this);
            this.registerSubscriptions();
            this.window = window;
        }

        protected registerSubscriptions() {
            this.eventHub.subscribe(MyAccountEvents[MyAccountEvents.AddressDeleted], e => this.onAddressDeleted(e));
        }

        public getRecurringCart() {

            var nameUrlQueryString: string = 'name=';
            var cartName: string = '';

            if (window.location.href.indexOf(nameUrlQueryString) > -1) {
                cartName = window.location.href.substring(window.location.href.indexOf(nameUrlQueryString)
                    + nameUrlQueryString.length);
            }

            var data = {
                cartName: cartName
            };

            this.recurringOrderService.getRecurringCart(data)
                .then(result => {
                    //console.log(result);
                    this.viewModel = result;

                    this.reRenderCartPage(result);
                })
                .fail((reason) => {
                    console.error(reason);
                });
        }

        public toggleEditNextOccurence(actionContext: IControllerActionContext) {
            var context: JQuery = $('#btntoggleEditNextOccurence');

            this.editNextOcurrence = !this.editNextOcurrence;

            if (this.editNextOcurrence) {
                this.closeOtherEditSections(actionContext, EditSection.NextOccurence);
            }

            let nextOccurence = context.data('next-occurence');
            let formatedNextOccurence = context.data('formated-next-occurence');
            let nextOccurenceValue = context.data('next-occurence-value');
            let total = context.data('total');

            let vm = {
                EditMode: this.editNextOcurrence,
                NextOccurence: nextOccurence,
                FormatedNextOccurence: formatedNextOccurence,
                NextOccurenceValue: nextOccurenceValue,
                OrderSummary: {
                    Total: total
                }
            };
            this.render('RecurringCartDetailsSummary', vm);

            DatepickerService.renderDatepicker('.datepicker');
        }

        public saveEditNextOccurence(actionContext: IControllerActionContext) {
            var context: JQuery = actionContext.elementContext;

            let element = <HTMLInputElement>$('#NextOcurrence')[0];
            let newDate = element.value;
            let isValid = this.nextOcurrenceIsValid(newDate);

            if (isValid) {
                let cartName = this.viewModel.Name;
                let data: IRecurringOrderLineItemsUpdateDateParam = {
                    CartName: cartName,
                    NextOccurence: newDate
                };

                var busyHandle = this.asyncBusy();

                this.recurringOrderService.updateLineItemsDate(data)
                    .then((viewModel) => {

                        let hasMerged = viewModel.RescheduledCartHasMerged;

                        if (hasMerged) {
                            //Redirect to my orders
                            let url = viewModel.RecurringCartsUrl;
                            if (!_.isUndefined(url) && url.length > 0) {
                                this.window.location.href = url;
                            }
                        } else if (!_.isEmpty(viewModel)) {

                            //TODO refresh cart cache
                            // let currentCart;
                            // viewModel.RecurringOrderCartsViewModel.RecurringOrderCartViewModelList.forEach(cart => {
                            //     if (cart.Name === this.viewModel.Name) {
                            //         currentCart = cart;
                            //     }
                            // });

                            // if (currentCart) {
                            //     this.viewModel = currentCart;
                            //     console.log(currentCart);
                            //     this.reRenderCartPage(currentCart);
                            // }
                            this.getRecurringCart();
                        }
                        busyHandle.done();
                    })
                    .fail((reason) => {
                        console.error(reason);
                        busyHandle.done();
                    });
            } else {
                console.error('Error: Invalid date while saving cart');
                this.showError('InvalidDateSelected', `[data-templateid="RecurringCartDetailsSummary"]`);
            }
        }

        public nextOcurrenceIsValid(value) {
            let newDate = this.convertDateToUTC(new Date(Date.parse(value)));
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

        public toggleEditShippingMethod (actionContext: IControllerActionContext) {
            var context: JQuery = $('#btntoggleEditShippingMethod');

            this.editShippingMethod = !this.editShippingMethod;

            let busy = this.asyncBusy({elementContext: actionContext.elementContext});
            this.render('RecurringCartDetailsShippingMethod', { IsLoading: true });

            let shippingMethodDisplayName = context.data('shipping-method-display-name');
            let shippingMethodCost = context.data('shipping-method-cost');
            let shippingMethodName = context.data('selected-shipping-method-name');
            let shippingMethodFulfillmentType = context.data('selected-shipping-method-fulfillment-type');
            let originalShippingMethodName = context.data('original-selected-shipping-method-name');
            let originalShippingMethodFulfillmentType = context.data('original-selected-shipping-method-fulfillment-type');

            //TODO : manage changing type
            //
            this.originalShippingMethodType = originalShippingMethodFulfillmentType;

            if (this.editShippingMethod) {
                this.closeOtherEditSections(actionContext, EditSection.ShippingMethod);

                this.getShippingMethods(this.viewModel.Name)
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
                            EditMode: this.editShippingMethod,
                            ShippingMethods: shippingMethods,
                            SelectedMethod: selectedShippingMethodName,
                            ShippingMethod: {
                                DisplayName: shippingMethodDisplayName,
                                Cost: shippingMethodCost,
                                Name: shippingMethodName,
                                FulfillmentMethodTypeString: shippingMethodFulfillmentType
                            }
                        };

                        this.render('RecurringCartDetailsShippingMethod', vm);
                        busy.done();
                    });
            } else {
                if (this.hasShippingMethodTypeChanged) {

                    shippingMethodDisplayName = context.data('shipping-method-display-name-tmp');
                    shippingMethodCost = context.data('shipping-method-cost-tmp');
                    shippingMethodName = context.data('selected-shipping-method-name-tmp');
                    shippingMethodFulfillmentType = context.data('selected-shipping-method-fulfillment-type-tmp');

                    var vm = {
                        EditMode: this.editShippingMethod,
                        ShippingMethod: {
                            DisplayName: shippingMethodDisplayName,
                            Cost: shippingMethodCost,
                            Name: shippingMethodName,
                            FulfillmentMethodTypeString: shippingMethodFulfillmentType
                        }
                    };

                } else {
                    var vm = {
                        EditMode: this.editShippingMethod,
                        ShippingMethod: {
                            DisplayName: shippingMethodDisplayName,
                            Cost: shippingMethodCost,
                            Name: shippingMethodName,
                            FulfillmentMethodTypeString: shippingMethodFulfillmentType
                        }
                    };
                }
                this.render('RecurringCartDetailsShippingMethod', vm);
                busy.done();
            }
        }

        public closeOtherEditSections(actionContext: IControllerActionContext, type: EditSection) {

            if (this.editNextOcurrence && type !== EditSection.NextOccurence) {
                this.toggleEditNextOccurence(actionContext);
            }
            if (this.editShippingMethod && type !== EditSection.ShippingMethod) {
                this.toggleEditShippingMethod(actionContext);
            }
            if (this.editAddress && type !== EditSection.Address) {
                this.toggleEditAddress(actionContext);
            }
            if (this.editPayment && type !== EditSection.Payment) {
                this.toggleEditPayment(actionContext);
            }
        }

        public resetEditToggleFlags() {
            this.editNextOcurrence = false;
            this.editShippingMethod = false;
            this.editAddress = false;
            this.editPayment = false;
        }

        public getShippingMethods(cartName) : Q.Promise<any> {
            let param: IRecurringOrderGetCartShippingMethods = {
                CartName: cartName
            };
            return this.recurringOrderService.getCartShippingMethods(param)
                    .fail((reason) => {
                        console.error('Error while retrieving shipping methods', reason);
                    });
        }

        public saveEditShippingMethod(actionContext: IControllerActionContext) {
            let element = $('#ShippingMethod').find('input[name=ShippingMethod]:checked')[0];

            if (_.isUndefined(element)) {
                console.error('Error: Missing shipping method');
                this.showError('RecurringCartShippingMethodMissing', `[data-templateid="RecurringCartDetailsShippingMethod"]`);
                return;
            }

            var newType = element.dataset['fulfillmentMethodType'];
            this.manageSaveShippingMethod(newType, actionContext);
        }

        public methodSelected(actionContext: IControllerActionContext) {
            var shippingProviderId = actionContext.elementContext.data('shipping-provider-id');
             $('#ShippingProviderId').val(shippingProviderId.toString());
        }

        public manageSaveShippingMethod(newType, actionContext) {
            //When shipping method is changed from ship to store and ship to home, address must correspond to 
            //store adress/home address.
            //When the type change, we wait to save shipping method and open adresse section. Then, when saving valid address,
            //also save the shipping method.
            //When cancel in one of the two steps, revert to original values.
            //If saving shipping method and the method type doesn't change, save immediatly.

            //TODO: This fulfillment type management is ON HOLD. Directly save shipping method.
            //this.hasShippingMethodTypeChanged = this.originalShippingMethodType !== newType;
            this.hasShippingMethodTypeChanged = false;

            let shippingProviderId = $('#ShippingProviderId').val();

            let element = $('#ShippingMethod').find('input[name=ShippingMethod]:checked');
            let shippingMethodName = element.val();
            let shippingMethodCost = element.data('shipping-method-cost');
            let shippingMethodDisplayName = element.data('shipping-method-display-name');
            let shippingMethodFulfillmentType = element.data('selected-shipping-method-fulfillment-type');

            if (this.hasShippingMethodTypeChanged) {

                var btnEditShippingMethod: JQuery = $('#btntoggleEditShippingMethod');
                btnEditShippingMethod.data('shipping-method-display-name-tmp', shippingMethodDisplayName);
                btnEditShippingMethod.data('shipping-method-cost-tmp', shippingMethodCost);
                btnEditShippingMethod.data('selected-shipping-method-name-tmp', shippingMethodName);
                btnEditShippingMethod.data('selected-shipping-method-fulfillment-type-tmp', shippingMethodFulfillmentType);

                if (newType === ShippingMethodType.Shipping) {
                    //TODO
                    //Toggle addresses with Get customer Addresses

                    this.newShippingMethodType = ShippingMethodType.Shipping;

                } else if (newType === 'ShipToStore') {
                    //TODO
                    //Toggle addresses with Get store Addresses

                    this.newShippingMethodType = ShippingMethodType.ShipToStore;

                }
                this.toggleEditAddress(actionContext);
            } else {
                //Do the save
                let cartName = this.viewModel.Name;

                let data: IRecurringOrderCartUpdateShippingMethodParam = {
                    shippingProviderId: shippingProviderId,
                    shippingMethodName: shippingMethodName,
                    cartName: cartName
                };

                if (_.isUndefined(shippingProviderId) || _.isUndefined(shippingMethodName)) {
                    console.error('Error: Missing shipping method');
                    this.showError('RecurringCartShippingMethodMissing', `[data-templateid="RecurringCartDetailsShippingMethod"]`);
                    return;
                }

                var busy = this.asyncBusy({ elementContext: actionContext.elementContext });
                this.recurringOrderService.updateCartShippingMethod(data)
                    .then(result => {

                        this.reRenderCartPage(result);
                    })
                    .fail((reason) => {
                        console.error('Error: Error while saving shipping method', reason);
                        this.showError('RecurringCartShippingMethodUpdateFailed', `[data-templateid="RecurringCartDetailsShippingMethod"]`);
                    })
                    .fin(() => busy.done());
            }
        }

        public reRenderCartPage(vm) {
            this.resetEditToggleFlags();
            this.viewModel = vm;
            this.render(this.viewModelName, vm);
        }

        public toggleEditAddress (actionContext: IControllerActionContext) {

            this.editAddress = !this.editAddress;

            let busy = this.asyncBusy({elementContext: actionContext.elementContext});
            this.render('RecurringCartDetailsAddress', { IsLoading: true });

            if (this.editAddress) {
                this.closeOtherEditSections(actionContext, EditSection.Address);

                if (!this.newShippingMethodType || (this.newShippingMethodType === ShippingMethodType.Shipping)) {
                    this.recurringCartAddressRegisteredService.getRecurringCartAddresses(this.viewModel)
                        .then((addressesVm) => {
                            addressesVm.EditMode = this.editAddress;
                            addressesVm.Payment = {
                                BillingAddress: {
                                    UseShippingAddress: this.viewModel.Payment.BillingAddress.UseShippingAddress
                                }
                            };

                            this.render('RecurringCartDetailsAddress', addressesVm);
                        })
                        .fin(() => busy.done());
                } else {
                    this.storeService.getStores()
                        .then((storesVm) => {
                            //console.log(storesVm);
                            //TODO: Open adresse with list of stores
                        })
                        .fin(() => busy.done());
                }

            } else {
                this.render('RecurringCartDetailsAddress', this.viewModel);

                if (this.hasShippingMethodTypeChanged) {
                    this.hasShippingMethodTypeChanged = false;
                    this.newShippingMethodType = '';
                    this.render('RecurringCartDetailsShippingMethod', this.viewModel);
                }
                busy.done();
            }
        }

        public saveEditAddress (actionContext: IControllerActionContext) {
            //If shipping method type has changed, save address and shipping method

            let shippingAddressId = $(this.context.container).find('input[name=ShippingAddressId]:checked').val();
            let billingAddressId = $(this.context.container).find('input[name=BillingAddressId]:checked').val();
            let useSameForShippingAndBilling = $(this.context.container).find('input[name=UseShippingAddress]:checked').val();
            let cartName = this.viewModel.Name;

            if (_.isUndefined(shippingAddressId)) {
                console.error('Error: Missing shipping address');
                this.showError('RecurringCartShippingAddressMissing', `[data-templateid="RecurringCartDetailsAddress"]`);
                return;
            }

            let data: IRecurringOrderUpdateTemplateAddressParam = {
                shippingAddressId: shippingAddressId,
                billingAddressId: null,
                cartName: cartName,
                useSameForShippingAndBilling: useSameForShippingAndBilling
            };

            let useSameBool : boolean = Boolean(JSON.parse(useSameForShippingAndBilling));

            if (!useSameBool) {
                data.billingAddressId = billingAddressId;
            }

            if (!useSameBool && _.isUndefined(billingAddressId)) {
                console.error('Error: Missing billing address');
                this.showError('RecurringCartBillingAddressMissing', `[data-templateid="RecurringCartDetailsAddress"]`);
                return;
            }

            var busy = this.asyncBusy({ elementContext: actionContext.elementContext });

            this.recurringOrderService.updateCartShippingAddress(data)
                .then(result => {

                    //console.log(result);

                    if (this.hasShippingMethodTypeChanged) {
                        this.hasShippingMethodTypeChanged = false;
                    }

                    this.viewModel = result;

                    this.reRenderCartPage(result);
                })
                .fail((reason) => {
                    console.error('Error: Error while saving addresses', reason);
                    this.showError('RecurringCartAddressesUpdateFailed', `[data-templateid="RecurringCartDetailsAddress"]`);
                })
                .fin(() => busy.done());
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

        public toggleEditPayment (actionContext: IControllerActionContext) {

            this.editPayment = !this.editPayment;

            let busy = this.asyncBusy({elementContext: actionContext.elementContext});
            this.render('RecurringCartDetailsPayment', { IsLoading: true });

            if (this.editPayment) {
                this.closeOtherEditSections(actionContext, EditSection.Payment);

                let data: IRecurringOrderGetCartPaymentMethods = {
                    cartName: this.viewModel.Name
                };
                this.recurringOrderService.getCartPaymentMethods(data)
                    .then(result => {

                        result.EditMode = this.editPayment;
                        this.render('RecurringCartDetailsPayment', result);
                    })
                    .fin(() => busy.done());

            } else {
                this.render('RecurringCartDetailsPayment', this.viewModel);
                busy.done();
            }

        }

        public updateLineItem(actionContext: IControllerActionContext): void {

            if (!this.debounceUpdateLineItem) {
                this.debounceUpdateLineItem =
                    _.debounce((args) =>
                        this.applyUpdateLineItemQuantity(args), this.updateWaitTime);
            }

            let context: JQuery = actionContext.elementContext;
            let cartQuantityElement = actionContext.elementContext
                .parents('.cart-item')
                .find('.cart-quantity');

            let incrementButtonElement = actionContext.elementContext
                .parents('.cart-item')
                .find('.increment-quantity');

            let decrementButtonElement = actionContext.elementContext
                .parents('.cart-item')
                .find('.decrement-quantity');

            const action: string = <any>context.data('action');
            const currentQuantity: number = parseInt(cartQuantityElement.text(), 10);

            let frequencyName = context.data('recurringorderfrequencyname');
            let programName = context.data('recurringorderprogramname');

            const updatedQuantity = this.updateQuantity(action, currentQuantity);
            var quantity: number = parseInt(<any>context.data('quantity'), 10);

            updatedQuantity === 1 ? decrementButtonElement.attr('disabled', 'disabled') : decrementButtonElement.removeAttr('disabled');
            //updatedQuantity === 99 ? incrementButtonElement.attr('disabled', 'disabled') : incrementButtonElement.removeAttr('disabled');

            cartQuantityElement.text(updatedQuantity);
            let cartName = this.viewModel.Name;

            var args: any = {
                actionContext: actionContext,
                context: context,
                cartQuantityElement: cartQuantityElement,
                cartName: cartName,
                frequencyName: frequencyName,
                programName: programName
            };

            if (quantity !== updatedQuantity) {
                //use only debounced function when incrementing/decrementing quantity
                this.debounceUpdateLineItem(args);
            }
        }

        public applyUpdateLineItemQuantity(args: any) {

            var context: JQuery = args.actionContext.elementContext;
            var busy = this.asyncBusy({ elementContext: args.actionContext.elementContext });
            let actionElementSpan =  args.context.find('span.fa').not('.loading-indicator');

            const updateLineItemQuantityParam: IRecurringOrderUpdateLineItemQuantityParam = {
                lineItemId:  args.context.data('lineitemid'),
                quantity: Number( args.cartQuantityElement.text()),
                cartName:  args.cartName,
                recurringProgramName:  args.programName,
                recurringFrequencyName:  args.frequencyName
            };
            args.cartQuantityElement.parents('.cart-item').addClass('is-loading');
            actionElementSpan.hide();
            this.recurringOrderService.updateLineItemQuantity(updateLineItemQuantityParam)
                .then(result => {
                    args.cartQuantityElement.parents('.cart-item').removeClass('is-loading');
                    actionElementSpan.show();

                    this.reRenderCartPage(result);
                })
                .fail((reason: any) => this.onLineItemQuantityFailed(context, reason))
                .fin(() => busy.done());
        }

        protected onLineItemQuantityFailed(context: JQuery, reason: any): void {
            console.error('Error while updating line item quantity.', reason);
            this.showError('LineItemQuantityFailed', `[data-templateid="RecurringCartContent"]`);
        }

        public updateQuantity(action: string, quantity: number): number {
            if (!action) {
                return quantity;
            }

            switch (action.toUpperCase()) {
                case 'INCREMENT':
                    quantity++;
                    break;

                case 'DECREMENT':
                    quantity--;
                    if (quantity < 1) {
                        quantity = 1;
                    }
                    break;
            }

            return quantity;
        }

        public deleteLineItem(actionContext: IControllerActionContext) {
            var context: JQuery = actionContext.elementContext;
            var lineItemId: string = <any>context.data('lineitemid');
            var productId: string = context.attr('data-productid');

            context.closest('.cart-row').addClass('is-loading');
            var cartName = this.viewModel.Name;

            const deleteLineItemParam: IRecurringOrderLineItemDeleteParam = {
                lineItemId: lineItemId,
                cartName: cartName
            };

            var busy = this.asyncBusy({ elementContext: actionContext.elementContext });

            this.recurringOrderService.deleteLineItem(deleteLineItemParam)
                .then(result => {
                    this.reRenderCartPage(result);

                    //TODO: Manage if last item?
                    //Deleting the last recurring item will reschedule the cartName to the line item next occurence
                })
                .fail((reason: any) => this.onLineItemDeleteFailed(context, reason))
                .fin(() => busy.done());
        }

        protected onLineItemDeleteFailed(context: JQuery, reason: any): void {
            console.error('Error while deleting line item.', reason);
            context.closest('.cart-row').removeClass('is-loading');

            this.showError('LineItemDeleteFailed', `[data-templateid="RecurringCartContent"]`);
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
                    this.eventHub.publish(MyAccountEvents[MyAccountEvents.AddressDeleted], { data: addressId });
                    //this.reRenderCartPage(this.viewModel);
                })
                .fin(() => busy.done());
        }

        public onAddressDeleted(e: IEventInformation) {

            var addressId = e.data;
            var $addressListItem = $(this.context.container).find('[data-address-id=' + addressId + ']');

            $addressListItem.remove();
        }

        public saveEditPayment(actionContext: IControllerActionContext) {
            let paymentMethodId = $(this.context.container).find('input[name=PaymentMethod]:checked').val();
            let cartName = this.viewModel.Name;
            let paymentProviderName = $(this.context.container).find('input[name=PaymentMethod]:checked').data('payment-provider');
            let paymentId = $(this.context.container).find('input[name=PaymentId]:checked').data('payment-id');
            let paymentType = $(this.context.container).find('input[name=PaymentMethod]:checked').data('payment-type');

            let data: IRecurringOrderCartUpdatePaymentMethodParam = {
                paymentMethodId: paymentMethodId,
                paymentProviderName: paymentProviderName,
                cartName: cartName,
                paymentId: paymentId,
                paymentType: paymentType
            };

            if (_.isUndefined(paymentId)) {
                console.error('Error: Missing payment method');
                this.showError('RecurringCartPaymentMissing', `[data-templateid="RecurringCartDetailsPayment"]`);
                return;
            }

            var busy = this.asyncBusy({ elementContext: actionContext.elementContext });
            this.recurringOrderService.updateCartPaymentMethod(data)
                .then(result => {

                    //console.log(result);

                    this.viewModel = result;
                    this.reRenderCartPage(result);
                })
                .fail((reason) => {
                    console.error('Error: Error while saving payment', reason);
                    this.showError('RecurringCartPaymentUpdateFailed', `[data-templateid="RecurringCartDetailsPayment"]`);

                })
                .fin(() => busy.done());
        }

        protected releaseBusyHandler(): void {
            if (this.busyHandler) {
                this.busyHandler.done();
                this.busyHandler = null;
            }
        }

        public showError(errorCode: string, parentSelector: string) {
            var localization: string = LocalizationProvider.instance().getLocalizedString('Errors', `L_${errorCode}`);

            var error: IError = {
                ErrorCode: errorCode,
                LocalizedErrorMessage: localization
            };

            var errorCollection = {
                Errors: []
            };

            if (error) {
                errorCollection.Errors.push(error);
            }

            this.render('FormErrorMessages', errorCollection, parentSelector);

            //Scroll to the error message if there's one
            if (errorCollection && errorCollection.Errors && errorCollection.Errors.length > 0) {
                Utils.scrollToElement( $('[data-templateid="FormErrorMessages"]:has(div)'));
            }
        }
    }
}
