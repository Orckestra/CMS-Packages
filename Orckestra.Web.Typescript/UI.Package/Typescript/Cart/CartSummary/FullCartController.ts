///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='../../Repositories/CartRepository.ts' />
///<reference path='../../ErrorHandling/ErrorHandler.ts' />
///<reference path='../../Events/EventScheduler.ts' />
///<reference path='../RecurringOrder/Services/RecurringOrderService.ts' />
///<reference path='../RecurringOrder/Repositories/RecurringOrderRepository.ts' />
///<reference path='./CartService.ts' />

module Orckestra.Composer {

    export class FullCartController extends Orckestra.Composer.Controller {
        protected source: string = 'Checkout';
        protected debounceUpdateLineItem: (args: any) => void;

        protected loaded: boolean = false;
        protected cartService: CartService = new CartService(new CartRepository(), this.eventHub);

        public initialize() {

            super.initialize();

            this.registerSubscriptions();
            this.loadCart();
        }

        protected registerSubscriptions() {

            this.eventHub.subscribe('cartUpdated', e => this.onCartUpdated(e.data));
        }

        protected onCartUpdated(cart: any): void {
            this.render('CartContent', cart);
            ErrorHandler.instance().removeErrors();
        }

        protected loadCart() {

            this.cartService.getFreshCart()
                .then(cart => {

                    if (this.loaded) {
                        return cart;
                    }

                    var e: IEventInformation = {
                        data: {
                            Cart: cart,
                            StepNumber: this.context.viewModel.CurrentStep
                        }
                    };

                    this.eventHub.publish('checkoutStepRendered', e);
                    return cart;

                })
                .done(cart => {

                    this.eventHub.publish('cartUpdated', { data: cart });
                    this.loaded = true;

                }, reason => this.loadCartFailed(reason));
        }

        protected loadCartFailed(reason: any): void {
            console.error('Error while loading the cart.', reason);
            this.context.container.find('.js-loading').hide();

            ErrorHandler.instance().outputErrorFromCode('LoadCartFailed');
        }

        public updateLineItem(actionContext: IControllerActionContext): void {
            if (!this.debounceUpdateLineItem) {
                this.debounceUpdateLineItem =
                    _.debounce((args) =>
                        this.executeLineItemUpdate(args), 300);
            }

            var context: JQuery = actionContext.elementContext;
            context.closest('.cart-row').addClass('is-loading');

            var lineItemId: string = <any>context.data('lineitemid');
            var productId: string = context.attr('data-productid');
            var action: string = <any>context.data('action');
            var quantity: number = parseInt(<any>context.data('quantity'), 10);
            var tmpQuantity: number = context.data('tmp-qte') ? parseInt(<any>context.data('tmp-qte'), 10) : null;
            var updatedQuantity: number = this.updateQuantity(action, tmpQuantity ? tmpQuantity : quantity);
            var recurringOrderFrequencyName: string = <any>context.data('recurringorderfrequencyname');
            var recurringOrderProgramName: string = <any>context.data('recurringorderprogramname');

            context.data('tmp-qte', updatedQuantity);

            var args: any = {
                context: context,
                lineItemId: lineItemId,
                originalQuantity: quantity,
                updatedQuantity: updatedQuantity,
                productId: productId,
                recurringOrderFrequencyName: recurringOrderFrequencyName,
                recurringOrderProgramName: recurringOrderProgramName
            };

            if (quantity !== updatedQuantity) {
                //use only debouced function when incrementing/decrementing quantity
                this.debounceUpdateLineItem(args);
            } else {
                this.executeLineItemUpdate(args);
            }
        }

        protected executeLineItemUpdate(args: any): void {
            this.cartService.getCart().then((cart: any) => {
                let lineItem = _.find(cart.LineItemDetailViewModels, (li: any) => li.Id === args.lineItemId);

                if (this.isUpdateRequired(args, lineItem)) {
                    let delta = args.updatedQuantity - lineItem.Quantity;
                    let positiveDelta: number = delta < 0 ? delta * -1 : delta;
                    let data = this.getLineItemDataForAnalytics(lineItem, positiveDelta);

                    if (delta !== 0) {
                        let eventName = (delta > 0) ? 'lineItemAdding' : 'lineItemRemoving';
                        this.eventHub.publish(eventName, { data: data });
                    }

                    args.context.removeData('tmp-qte');
                    args.context.data('quantity', args.updatedQuantity);

                    return this.cartService.updateLineItem(args.lineItemId,
                        args.updatedQuantity,
                        args.productId,
                        args.recurringOrderFrequencyName === '' ? null : args.recurringOrderFrequencyName,
                        args.recurringOrderProgramName);
                } else {
                    this.render('CartContent', cart);
                    return cart;
                }
            }).fail((reason: any) => this.lineItemUpdateFailed(args.context, reason));
        }

        protected isUpdateRequired(updateLineItemArgs, lineItem): boolean {
            if (!lineItem) {
                return false;
            }

            var shouldUpdateQuantity = updateLineItemArgs.updatedQuantity - lineItem.Quantity !== 0;
            var shouldUpdateRecurringFrequency = updateLineItemArgs.recurringOrderFrequencyName !== lineItem.RecurringOrderFrequencyName;
            var shouldUpdateRecurringProgram = updateLineItemArgs.recurringOrderProgramName !== lineItem.RecurringOrderProgramName;

            return shouldUpdateQuantity || shouldUpdateRecurringFrequency || shouldUpdateRecurringProgram;
        }

        protected lineItemUpdateFailed(context: JQuery, reason: any) {
            console.error('Error while updating line item quantity.', reason);
            context.closest('.cart-row').removeClass('is-loading');

            ErrorHandler.instance().outputErrorFromCode('LineItemUpdateFailed');
        }

        protected getLineItemDataForAnalytics(lineItem: any, quantity: number) : any {
            var data = {
                List: this.source,
                DisplayName: lineItem.ProductSummary.DisplayName,
                ProductId: lineItem.ProductId,
                ListPrice: lineItem.ListPrice,
                Brand: lineItem.ProductSummary.Brand,
                CategoryId: lineItem.ProductSummary.CategoryId,
                Variant: undefined,
                Quantity: quantity
            };

            if (lineItem.VariantId && lineItem.KeyVariantAttributesList) {
                data.Variant = this.buildVariantName(lineItem.KeyVariantAttributesList);
        }

            return data;
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

            this.cartService.getCart()
                .then((cart: any) => {
                    var lineItem = _.find(cart.LineItemDetailViewModels, (li: any) => li.Id === lineItemId);

                    var data = this.getLineItemDataForAnalytics(lineItem, lineItem.Quantity);

                    this.eventHub.publish('lineItemRemoving', { data: data });

                    return this.cartService.deleteLineItem(lineItemId, productId);
                }).fail((reason: any) => this.onLineItemDeleteFailed(context, reason));
        }

        protected onLineItemDeleteFailed(context: JQuery, reason: any): void {
            console.error('Error while deleting line item.', reason);
            context.closest('.cart-row').removeClass('is-loading');

            ErrorHandler.instance().outputErrorFromCode('LineItemDeleteFailed');
        }

        protected buildVariantName(kvas: any[]): string {
            var nameParts: string[] = [];

            for (var i: number = 0; i < kvas.length; i++) {
                var value: any = kvas[i].OriginalValue;

                nameParts.push(value);
            }

            return nameParts.join(' ');
        }

        /**
         * Grab the requested frequency change information from the form then update the lineitem
         * @param {Orckestra.Composer.IControllerActionContext} actionContext
         */
        public updateLineItemRecurringFrequency(actionContext: IControllerActionContext): void {
            let applyButton$: JQuery = actionContext.elementContext,
                recurringModesContainer$: JQuery = applyButton$.closest('.js-recurringModes'), //
                selectedRecurringMode: string = (recurringModesContainer$.find('input[name^="recurringMode"]:checked').val()).toUpperCase(),
                selectedFrequency: string = (recurringModesContainer$.find('select').val()),
                optionValue: string;

            if (applyButton$) {
                if (selectedRecurringMode === 'SINGLE') {
                    optionValue = '';
                } else {
                    optionValue = selectedFrequency;
                }

                applyButton$.attr('data-recurringorderfrequencyname', optionValue);
                this.updateLineItem(actionContext);
            }
        }

        /**
         * Reset the lineitem frequency to its previous state (without re-rendering)
         * @param {Orckestra.Composer.IControllerActionContext} actionContext
         */
        public resetLineItemRecurringFrequency(actionContext: IControllerActionContext): void {
            let cancelButton$: JQuery = actionContext.elementContext,
                recurringModesContainer$: JQuery = cancelButton$.closest('.js-recurringModes'),
                recurringModeRadioGroup$: JQuery = recurringModesContainer$.find('input[name^="recurringMode"]'),
                selectedRecurringModeRadio$: JQuery = recurringModeRadioGroup$.filter((index, node) => {
                    return $(node).prop('checked');
                }),
                frequencySelect$: JQuery = recurringModesContainer$.find('select'),
                previouslySelectedFrequency: string = cancelButton$.attr('data-recurringorderfrequencyname');

            frequencySelect$.val(previouslySelectedFrequency).trigger('change');

            if (previouslySelectedFrequency === '' && selectedRecurringModeRadio$.val() !== 'single') {
                let singleRadio$: JQuery = recurringModeRadioGroup$.filter((index, node) => {
                    return (<HTMLInputElement>node).value === 'single';
                });
                recurringModeRadioGroup$.val(['single']);
                singleRadio$.trigger('change');
            } else if (previouslySelectedFrequency !== '' && selectedRecurringModeRadio$.val() !== 'recurring') {
                let recurringRadio$: JQuery = recurringModeRadioGroup$.filter((index, node) => {
                    return (<HTMLInputElement>node).value === 'recurring';
                });
                recurringModeRadioGroup$.val(['recurring']);
                recurringRadio$.trigger('change');
            }

            recurringModesContainer$.closest('.js-cartRecurring').find('.js-cartRecurringCta').show();
        }

        /**
         * Update UI of recurring modes radio buttons when switching between single and recurring
         * @param {Orckestra.Composer.IControllerActionContext} actionContext
         */
        public changeRecurringMode(actionContext: IControllerActionContext) {
            let clickedRadioButton$: JQuery = actionContext.elementContext,
                recurringModesContainer$: JQuery = clickedRadioButton$.closest('.js-recurringModes');

            recurringModesContainer$.find('.js-recurringModeRow.selected').removeClass('selected');
            clickedRadioButton$.closest('.js-recurringModeRow').addClass('selected');
            recurringModesContainer$.find('.modeSelection').collapse('toggle');
        }

        /**
         * Toggle the visibility of the recurring modes selector
         * @param {Orckestra.Composer.IControllerActionContext} actionContext
         */
        public expandRecurringModes(actionContext: IControllerActionContext) {
            actionContext.elementContext.closest('.js-cartRecurringCta').hide();
        }
    }
}
