///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../../../Events/IEventHub.ts' />
///<reference path='./IRecurringOrderService.ts' />

module Orckestra.Composer {
    'use strict';

    export class RecurringOrderService implements IRecurringOrderService {
        protected repository: IRecurringOrderRepository;
        protected eventHub: Orckestra.Composer.IEventHub;

        public constructor(repository: IRecurringOrderRepository, eventHub: Orckestra.Composer.IEventHub) {
            if (!eventHub) {
                throw new Error('Error: eventHub is required');
            }
            this.repository = repository;
            this.eventHub = eventHub;
        }

        public updateLineItemsDate(updateLineItemsParam: IRecurringOrderLineItemsUpdateDateParam): Q.Promise<any> {
            return this.repository.updateLineItemsDate(updateLineItemsParam);
        }

        public deleteLineItem(deleteLineItemParam: IRecurringOrderLineItemDeleteParam): Q.Promise<any> {
            return this.repository.deleteLineItem(deleteLineItemParam);
        }

        public deleteLineItems(deleteLineItemsParam: IRecurringOrderLineItemsDeleteParam): Q.Promise<any> {
            return this.repository.deleteLineItems(deleteLineItemsParam);
        }

        public getCustomerAddresses(): Q.Promise<any> {
            return this.repository.getCustomerAddresses();
        }

        public getCustomerPaymentMethods(): Q.Promise<any> {
            return this.repository.getCustomerPaymentMethods();
        }

        public updateCartShippingAddress(updateCartAddressParam: IRecurringOrderUpdateCartAddressParam): Q.Promise<any> {
            return this.repository.updateCartShippingAddress(updateCartAddressParam);
        }

        public updateCartBillingAddress(updateTemplateAddressParam: IRecurringOrderUpdateTemplateAddressParam): Q.Promise<any> {
            return this.repository.updateCartBillingAddress(updateTemplateAddressParam);
        }


        public updateTemplatePaymentMethod(updateTemplatePaymentMethodParam: IRecurringOrderUpdateTemplatePaymentMethodParam): Q.Promise<any> {
            return this.repository.updateTemplatePaymentMethod(updateTemplatePaymentMethodParam);
        }

        public updateLineItemQuantity(updateLineItemQuantityParam: IRecurringOrderUpdateLineItemQuantityParam): Q.Promise<any> {
            return this.repository.updateLineItemQuantity(updateLineItemQuantityParam);
        }

        public getRecurringOrderCartsByUser() {
            return this.repository.getRecurringOrderCartsByUser();
        }

        public getRecurringOrderTemplatesByUser() {
            return this.repository.getRecurringOrderTemplatesByUser();
        }

        public updateTemplateLineItemQuantity(updateLineItemQuantityParam: IRecurringOrderTemplateUpdateLineItemQuantityParam): Q.Promise<any> {
            return this.repository.updateTemplateLineItemQuantity(updateLineItemQuantityParam);
        }

        public getRecurringOrderProgramsByUser(): Q.Promise<any> {
            return this.repository.getRecurringOrderProgramsByUser();
        }

        public getRecurringOrderProgramsByNames(programsByNamesParam: IRecurringOrderProgramsByNamesParam): Q.Promise<any> {
            return this.repository.getRecurringOrderProgramsByNames(programsByNamesParam);
        }

        public updateTemplateLineItem(templateLineItemUpdateParam: IRecurringOrderTemplateLineItemUpdateParam): Q.Promise<any> {
            return this.repository.updateTemplateLineItem(templateLineItemUpdateParam);
        }

        public deleteTemplateLineItem(deleteTemplateLineItemParam: IRecurringOrderTemplateLineItemDeleteParam): Q.Promise<any> {
            return this.repository.deleteTemplateLineItem(deleteTemplateLineItemParam);
        }

        public deleteTemplateLineItems(deleteTemplateLineItemsParam: IRecurringOrderTemplateLineItemsDeleteParam): Q.Promise<any> {
            return this.repository.deleteTemplateLineItems(deleteTemplateLineItemsParam);
        }

        public getCartContainsRecurrence(): Q.Promise<any> {
            return this.repository.getCartContainsRecurrence();
        }

        public getRecurrenceConfigIsActive(): Q.Promise<any> {
            return this.repository.getRecurrenceConfigIsActive();
        }

        public getCanRemovePaymentMethod(paymentMethodId: string): Q.Promise<any> {
            return this.repository.getCanRemovePaymentMethod(paymentMethodId);
        }

        public getRecurringOrderCartSummaries(): Q.Promise<any> {
            return this.repository.getRecurringOrderCartSummaries();
        }

        public addRecurringOrderCartLineItem(addLineItemQuantityParam: IAddRecurringOrderCartLineItemParam): Q.Promise<any> {
            return this.repository.addRecurringOrderCartLineItem(addLineItemQuantityParam);
        }

        public getAnonymousCartSignInUrl(): Q.Promise<any> {
            return this.repository.getAnonymousCartSignInUrl();
        }

        public updateCartShippingMethod(updateCartShippingMethodParam: IRecurringOrderCartUpdateShippingMethodParam): Q.Promise<any> {
            return this.repository.updateCartShippingMethod(updateCartShippingMethodParam);
        }

        public getCartShippingMethods(getCartShippingMethodsParam: IRecurringOrderGetCartShippingMethods): Q.Promise<any> {
            return this.repository.getCartShippingMethods(getCartShippingMethodsParam);
        }

        public getOrderTemplateShippingMethods(): Q.Promise<any> {
            return this.repository.getOrderTemplateShippingMethods();
        }

        public getInactifProductsFromCustomer(): Q.Promise<any> {
            return this.repository.getInactifProductsFromCustomer();
        }

        public clearCustomerInactifItems(): Q.Promise<any> {
            return this.repository.clearCustomerInactifItems();
        }

        public getRecurringCart(getRecurringCartParam: IRecurringOrderCartParam): Q.Promise<any> {
            return this.repository.getRecurringCart(getRecurringCartParam);
        }

        public getCartPaymentMethods(getCartPaymentMethodsParam: IRecurringOrderGetCartPaymentMethods): Q.Promise<any> {
            return this.repository.getCartPaymentMethods(getCartPaymentMethodsParam);
        }

        public updateCartPaymentMethod(updateCartPaymentMethodParam: IRecurringOrderCartUpdatePaymentMethodParam): Q.Promise<any> {
            return this.repository.updateCartPaymentMethod(updateCartPaymentMethodParam);
        }

        public getRecurringTemplateDetail(recurringOrderTemplateId: string): Q.Promise<any> {
            return this.repository.getRecurringTemplateDetail(recurringOrderTemplateId);
        }

        public getTemplatePaymentMethods(getTemplatePaymentMethodsParam: IRecurringOrderGetTemplatePaymentMethods): Q.Promise<any> {
            return this.repository.getTemplatePaymentMethods(getTemplatePaymentMethodsParam);
        }

        private _mapLineItemToRequest(lineItem: any): any {
            return {
                CategoryId: lineItem.ProductSummary.CategoryId,
                ProductInfo: {
                    ProductId: lineItem.ProductId,
                    Sku: lineItem.Sku,
                    VariantId: lineItem.VariantId
                }
            };
        }

        private _mapRecurringLineItemToRequest(lineItem: any): any {
            return lineItem.SelectedRecurringOrderFrequencyId;
        }
    }
}
