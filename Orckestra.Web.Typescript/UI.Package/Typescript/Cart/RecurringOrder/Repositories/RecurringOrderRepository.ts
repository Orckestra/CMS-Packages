/// <reference path='../../../../Typings/tsd.d.ts' />
/// <reference path='../../../Mvc/ComposerClient.ts' />
/// <reference path='./IRecurringOrderRepository.ts' />

module Orckestra.Composer {
    'use strict';

    export class RecurringOrderRepository implements IRecurringOrderRepository {
        public updateLineItemsDate(updateLineItemsParam: IRecurringOrderLineItemsUpdateDateParam): Q.Promise<any> {
            return ComposerClient.put(`/api/recurringcart/reschedule`, updateLineItemsParam);
        }

        public deleteLineItem(deleteLineItemParam: IRecurringOrderLineItemDeleteParam): Q.Promise<any> {
            const {lineItemId, cartName} = deleteLineItemParam;
            if (!lineItemId) {
                throw new Error('lineItemId is required');
            }
            if (!cartName) {
                throw new Error('cartName is required');
            }

            var data = {
                CartName: cartName,
                LineItemId: lineItemId
            };

            return ComposerClient.remove(`/api/recurringcart/lineitem/`, data);
        }

        public deleteLineItems(deleteLineItemsParam: IRecurringOrderLineItemsDeleteParam): Q.Promise<any> {
            const {lineItemsIds, cartName} = deleteLineItemsParam;
            if (!lineItemsIds) {
                throw new Error('lineItemsIds is required');
            }
            if (!cartName) {
                throw new Error('cartName is required');
            }

            const datas = {
                LineItemsIds: lineItemsIds,
                cartName: cartName
            };

            return ComposerClient.remove(`/api/recurringordercart/${cartName}/lineitems/byIds`, datas);
        }

        public getCustomerAddresses() {

            return ComposerClient.get(`/api/recurringordercart/get-customer-addresses`);
        }

        public getCustomerPaymentMethods() {

            return ComposerClient.get(`/api/recurringordercart/get-customer-payment-methods`);
        }

        public getRecurringOrderCartsByUser() {
            return ComposerClient.get(`/api/recurringcart/upcoming-orders`);
        }

        public getRecurringOrderTemplatesByUser() {
            return ComposerClient.get(`/api/recurringordertemplate/getrecurringordertemplates`);
        }

        public updateCartShippingAddress(updateCartAddressParam: IRecurringOrderUpdateTemplateAddressParam): Q.Promise<any> {

            const {billingAddressId, shippingAddressId, cartName, useSameForShippingAndBilling} = updateCartAddressParam;
            if (!billingAddressId && !shippingAddressId) {
                throw new Error('billingAddressId or shippingAddressId is required');
            }
            if (!cartName) {
                throw new Error('cartName is required');
            }

            var data = {
                billingAddressId: billingAddressId,
                shippingAddressId: shippingAddressId,
                cartName: cartName,
                UseSameForShippingAndBilling: useSameForShippingAndBilling
            };

            return ComposerClient.put(`/api/recurringcart/address`, data);
        }

        public updateCartBillingAddress(updateTemplateAddressParam: IRecurringOrderUpdateTemplateAddressParam): Q.Promise<any> {

            const {billingAddressId, shippingAddressId, cartName, useSameForShippingAndBilling} = updateTemplateAddressParam;
            if (!billingAddressId && !shippingAddressId) {
                throw new Error('billingAddressId or shippingAddressId is required');
            }
            if (!cartName) {
                throw new Error('cartName is required');
            }

            var data = {
                billingAddressId: billingAddressId,
                shippingAddressId: shippingAddressId,
                cartName: cartName,
                UseSameForShippingAndBilling: useSameForShippingAndBilling
            };

            return ComposerClient.put(`/api/recurringordercart/${cartName}/billing-address`, data);
        }

        public updateTemplatePaymentMethod(updateTemplatePaymentMethodParam: IRecurringOrderUpdateTemplatePaymentMethodParam): Q.Promise<any> {

            const {paymentMethodId, cartName, providerName} = updateTemplatePaymentMethodParam;
            if (!paymentMethodId) {
                throw new Error('paymentMethodId is required');
            }
            if (!cartName) {
                throw new Error('cartName is required');
            }
            if (!providerName) {
                throw new Error('paymentMethodId is required');
            }

            var data = {
                paymentMethodId: paymentMethodId,
                cartName: cartName,
                providerName: providerName
            };

            return ComposerClient.put(`/api/recurringordercart/${cartName}/paymentmethod`, data);

        }

        public updateLineItemQuantity(updateLineItemQuantityParam: IRecurringOrderUpdateLineItemQuantityParam): Q.Promise<any> {

            const {lineItemId, quantity, cartName, recurringProgramName, recurringFrequencyName} = updateLineItemQuantityParam;

            if (!lineItemId) {
                throw new Error('lineItemId is required');
            }
            if (!quantity) {
                throw new Error('quantity is required');
            }
            if (!cartName) {
                throw new Error('cartName is required');
            }

            const data = {
                LineItemId: lineItemId,
                Quantity: quantity,
                CartName: cartName,
                RecurringOrderProgramName: recurringProgramName,
                RecurringOrderFrequencyName : recurringFrequencyName
            };

            return ComposerClient.put(`/api/recurringcart/lineitem/`, data);
        }

        public updateTemplateLineItemQuantity(updateLineItemQuantityParam: IRecurringOrderTemplateUpdateLineItemQuantityParam): Q.Promise<any> {

            const {lineItemId, quantity} = updateLineItemQuantityParam;

            if (!lineItemId) {
                throw new Error('lineItemId is required');
            }
            if (!quantity) {
                throw new Error('quantity is required');
            }

            const data = {
                RecurringLineItemId: lineItemId,
                Quantity: quantity
            };

            return ComposerClient.put(`/api/recurringordertemplate/lineitemquantity/`, data);
        }

        public getRecurringOrderProgramsByUser(): Q.Promise<any> {
            return ComposerClient.get(`/api/recurringordertemplate/get-recurring-order-programs-by-user/`);
        }

        public getRecurringOrderProgramsByNames(programsByNamesParam: IRecurringOrderProgramsByNamesParam): Q.Promise<any> {

            const {recurringOrderProgramNames} = programsByNamesParam;

            if (!recurringOrderProgramNames) {
                throw new Error('recurringOrderProgramNames is required');
            }

            const data = {
                RecurringOrderProgramNames: recurringOrderProgramNames
            };

            return ComposerClient.post(`/api/recurringordercart/get-recurring-order-programs-by-names/`, data);
        }

        public updateTemplateLineItem(templateLineItemUpdateParam: IRecurringOrderTemplateLineItemUpdateParam): Q.Promise<any> {
            const {
                lineItemId,
                paymentMethodId,
                shippingAddressId,
                billingAddressId,
                nextOccurence,
                frequencyName,
                shippingProviderId,
                shippingMethodName
            } = templateLineItemUpdateParam;

            if (!lineItemId) {
                throw new Error('lineItemId is required');
            }
            if (!paymentMethodId) {
                throw new Error('paymentMethodId is required');
            }
            if (!billingAddressId) {
                throw new Error('billingAddressId is required');
            }
            if (!shippingAddressId) {
                throw new Error('shippingAddressId is required');
            }
            if (!nextOccurence) {
                throw new Error('nextOccurence is required');
            }
            if (!frequencyName) {
                throw new Error('frequencyName is required');
            }
            if (!shippingProviderId) {
                throw new Error('shippingProviderId is required');
            }
            if (!shippingMethodName) {
                throw new Error('shippingMethodName is required');
            }

            const data = {
                LineItemId: lineItemId,
                PaymentMethodId: paymentMethodId,
                ShippingAddressId: shippingAddressId,
                BillingAddressId: billingAddressId,
                NextOccurence: nextOccurence,
                RecurringOrderFrequencyName: frequencyName,
                ShippingProviderId: shippingProviderId,
                ShippingMethodName: shippingMethodName
            };

            return ComposerClient.put(`/api/recurringordertemplate/lineitem/`, data);
        }

        public deleteTemplateLineItem(deleteTemplateLineItemParam: IRecurringOrderTemplateLineItemDeleteParam): Q.Promise<any> {
            const {lineItemId} = deleteTemplateLineItemParam;
            if (!lineItemId) {
                throw new Error('lineItemId is required');
            }

            const data = {
                LineItemId: lineItemId
            };
            return ComposerClient.remove(`/api/recurringordertemplate/lineitem/`, data);
        }

        public deleteTemplateLineItems(deleteTemplateLineItemsParam: IRecurringOrderTemplateLineItemsDeleteParam): Q.Promise<any> {
            const {lineItemsIds} = deleteTemplateLineItemsParam;
            if (!lineItemsIds) {
                throw new Error('lineItemsIds is required');
            }

            const datas = {
                LineItemsIds: lineItemsIds,
            };

            return ComposerClient.remove(`/api/recurringordertemplate/lineitems/byIds`, datas);
        }

        public getCartContainsRecurrence(): Q.Promise<any> {
            return ComposerClient.get(`/api/recurringordercart/get-cart-contains-recurrence`);
        }

        public getRecurrenceConfigIsActive(): Q.Promise<any> {
            return ComposerClient.get(`/api/recurringordercart/get-recurrence-config-is-active`);
        }

        public getCanRemovePaymentMethod(paymentMethodId): Q.Promise<any> {

            const datas = {
                PaymentMethodId: paymentMethodId,
            };
            return ComposerClient.post(`/api/recurringordertemplate/get-can-remove-payment-method`, datas);
        }

        public getRecurringOrderCartSummaries(): Q.Promise<any> {
            return ComposerClient.get(`/api/recurringordercart/customer-cart-summaries`);
        }

        public addRecurringOrderCartLineItem(addLineItemQuantityParam: IAddRecurringOrderCartLineItemParam): Q.Promise<any> {
            if (!addLineItemQuantityParam.cartName) {
                throw new Error('billingAddressId is required');
            }
            if (!addLineItemQuantityParam.productId) {
                throw new Error('productId is required');
            }
            if (!addLineItemQuantityParam.productDisplayName) {
                throw new Error('productDisplayName is required');
            }
            if (!addLineItemQuantityParam.sku) {
                throw new Error('sku is required');
            }
            if (!addLineItemQuantityParam.quantity) {
                throw new Error('quantity is required');
            }

            return ComposerClient.post(`/api/recurringordercart/lineitem`, addLineItemQuantityParam);
        }

        public updateCartShippingMethod(updateCartShippingMethodParam: IRecurringOrderCartUpdateShippingMethodParam): Q.Promise<any> {
            if (!updateCartShippingMethodParam.shippingProviderId) {
                throw new Error('shippingProviderId is required');
            }
            if (!updateCartShippingMethodParam.shippingMethodName) {
                throw new Error('shippingMethodName is required');
            }
            if (!updateCartShippingMethodParam.cartName) {
                throw new Error('cartName is required');
            }

            return ComposerClient.put(`/api/recurringcart/shippingmethod`,
                updateCartShippingMethodParam);
        }

        public getAnonymousCartSignInUrl(): Q.Promise<any> {
            return ComposerClient.get(`/api/recurringcart/getanonymouscartsigninurl`);
        }

        public getCartShippingMethods(getCartShippingMethodsParam: IRecurringOrderGetCartShippingMethods): Q.Promise<any> {
            if (!getCartShippingMethodsParam.CartName) {
                throw new Error('CartName is required');
            }

            return ComposerClient.post(`/api/cart/shippingmethodsbycartname`,
                getCartShippingMethodsParam);
        }

        public getOrderTemplateShippingMethods(): Q.Promise<any> {
            return ComposerClient.get(`/api/cart/shippingmethodsscope`);
        }

        public getInactifProductsFromCustomer(): Q.Promise<any> {
            return ComposerClient.get(`/api/recurringordertemplate/inactifProducts`);
        }

        public clearCustomerInactifItems(): Q.Promise<any> {
            return ComposerClient.get(`/api/recurringordertemplate/clear-customer-inactif-items`);
        }

        public getRecurringCart(getRecurringCartParam: IRecurringOrderCartParam): Q.Promise<any> {
            const {cartName} = getRecurringCartParam;
            if (!cartName) {
                throw new Error('cartName is required');
            }

            const data = {
                Name: cartName,
            };

            return ComposerClient.post(`/api/recurringcart/getrecurringcart`, data);
        }

        public getCartPaymentMethods(getCartPaymentMethodsParam: IRecurringOrderGetCartPaymentMethods): Q.Promise<any> {
            const {cartName} = getCartPaymentMethodsParam;
            if (!cartName) {
                throw new Error('cartName is required');
            }

            const data = {
                CartName: cartName,
            };

            return ComposerClient.post(`/api/payment/recurringcartspaymentmethods`, data);
        }


        public updateCartPaymentMethod(updateCartPaymentMethodParam: IRecurringOrderCartUpdatePaymentMethodParam): Q.Promise<any> {
            if (!updateCartPaymentMethodParam.paymentId) {
                throw new Error('paymentId is required');
            }
            if (!updateCartPaymentMethodParam.paymentProviderName) {
                throw new Error('paymentProviderName is required');
            }
            if (!updateCartPaymentMethodParam.cartName) {
                throw new Error('cartName is required');
            }
            if (!updateCartPaymentMethodParam.paymentMethodId) {
                throw new Error('paymentMethodId is required');
            }
            if (!updateCartPaymentMethodParam.paymentType) {
                throw new Error('paymentType is required');
            }

            return ComposerClient.put(`/api/recurringcart/paymentmethod`,
                updateCartPaymentMethodParam);
        }

        public getRecurringTemplateDetail(recurringOrderTemplateId: string): Q.Promise<any> {
            if (!recurringOrderTemplateId) {
                throw new Error('recurringOrderTemplateId is required');
            }

            const data = {
                RecurringOrderTemplateId: recurringOrderTemplateId,
            };

            return ComposerClient.post(`/api/recurringordertemplate/getrecurringordertemplatedetails`, data);
        }

        public getTemplatePaymentMethods(getTemplatePaymentMethodsParam: IRecurringOrderGetTemplatePaymentMethods): Q.Promise<any> {
            const {id} = getTemplatePaymentMethodsParam;
            if (!id) {
                throw new Error('id is required');
            }

            const data = {
                Id: id,
            };

            return ComposerClient.post(`/api/payment/recurringorderstemplatespaymentmethods`, data);
        }
    }
}
