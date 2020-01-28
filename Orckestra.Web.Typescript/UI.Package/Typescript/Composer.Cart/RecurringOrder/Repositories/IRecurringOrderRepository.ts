///<reference path='../../../../Typings/tsd.d.ts' />
///<reference path='../IRecurringOrderParameters.ts' />
///<reference path='../ViewModels/IRecurringOrderViewModel.ts' />


module Orckestra.Composer {
    export interface IRecurringOrderRepository {
        updateLineItemsDate(updateLineItemParam: IRecurringOrderLineItemsUpdateDateParam) : Q.Promise<any>;
        deleteLineItem(deleteLineItemParam: IRecurringOrderLineItemDeleteParam) : Q.Promise<any>;
        deleteLineItems(deleteLineItemsParam: IRecurringOrderLineItemsDeleteParam) : Q.Promise<any>;
        getCustomerAddresses(): Q.Promise<any>;
        getCustomerPaymentMethods(): Q.Promise<any>;
        updateCartShippingAddress(updateCartAddressParam: IRecurringOrderUpdateCartAddressParam): Q.Promise<any>;
        updateCartBillingAddress(updateTemplateAddressParam: IRecurringOrderUpdateTemplateAddressParam): Q.Promise<any>;
        updateTemplatePaymentMethod(updateTemplatePaymentMethodParam: IRecurringOrderUpdateTemplatePaymentMethodParam): Q.Promise<any>;
        updateLineItemQuantity(updateLineItemQuantityParam: IRecurringOrderUpdateLineItemQuantityParam) : Q.Promise<any>;
        getRecurringOrderCartsByUser(): Q.Promise<any>;
        getRecurringOrderTemplatesByUser(): Q.Promise<any>;
        updateTemplateLineItemQuantity(updateLineItemQuantityParam: IRecurringOrderTemplateUpdateLineItemQuantityParam) : Q.Promise<any>;
        getRecurringOrderProgramsByUser(): Q.Promise<any>;
        getRecurringOrderProgramsByNames(programsByNamesParam : IRecurringOrderProgramsByNamesParam): Q.Promise<any>;
        updateTemplateLineItem(templateLineItemUpdateParam: IRecurringOrderTemplateLineItemUpdateParam): Q.Promise<any>;
        deleteTemplateLineItem(deleteTemplateLineItemParam: IRecurringOrderTemplateLineItemDeleteParam): Q.Promise<any>;
        deleteTemplateLineItems(deleteTemplateLineItemsParam: IRecurringOrderTemplateLineItemsDeleteParam): Q.Promise<any>;
        getCartContainsRecurrence(): Q.Promise<any>;
        getRecurrenceConfigIsActive(): Q.Promise<any>;
        getCanRemovePaymentMethod(paymentMethodId: string): Q.Promise<any>;
        getRecurringOrderCartSummaries(): Q.Promise<any>;
        addRecurringOrderCartLineItem(addRecurringOrderCartLineItemParam: IAddRecurringOrderCartLineItemParam): Q.Promise<any>;
        getAnonymousCartSignInUrl(): Q.Promise<any>;
        updateCartShippingMethod(updateCartShippingMethodParam: IRecurringOrderCartUpdateShippingMethodParam): Q.Promise<any>;
        getCartShippingMethods(getCartShippingMethodsParam: IRecurringOrderGetCartShippingMethods): Q.Promise<any>;
        getOrderTemplateShippingMethods(): Q.Promise<any>;
        getInactifProductsFromCustomer(): Q.Promise<any>;
        clearCustomerInactifItems(): Q.Promise<any>;
        getRecurringCart(getRecurringCartParam: IRecurringOrderCartParam): Q.Promise<any>;
        getCartPaymentMethods(getCartPaymentMethodsParam: IRecurringOrderGetCartPaymentMethods): Q.Promise<any>;
        updateCartPaymentMethod(updateCartPaymentMethodParam: IRecurringOrderCartUpdatePaymentMethodParam): Q.Promise<any>;
        getRecurringTemplateDetail(recurringOrderTemplateId: string): Q.Promise<any>;
        getTemplatePaymentMethods(getTemplatePaymentMethodsParam: IRecurringOrderGetTemplatePaymentMethods): Q.Promise<any>;
    }
}
