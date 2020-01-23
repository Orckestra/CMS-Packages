///<reference path='../../../Typings/tsd.d.ts' />


module Orckestra.Composer {
    export interface IAddRecurringOrderCartLineItemParam {
        cartName: string;
        productId: string;
        productDisplayName: string;
        variantId: string;
        sku: string;
        quantity: number;
        recurringOrderFrequencyName: string;
        recurringOrderProgramName: string;
    }

    export interface IRecurringOrderCartUpdateShippingMethodParam {
        shippingProviderId: string;
        shippingMethodName: string;
        cartName: string;
    }

    export interface IRecurringOrderGetCartShippingMethods {
        CartName: string;
    }

    export interface IRecurringOrderLineItemDeleteParam {
        lineItemId: string;
        cartName: string;
    }

    export interface IRecurringOrderLineItems {
        RecurringOrderTemplateLineItemId: string;
    }

    export interface IRecurringOrderLineItemsDeleteParam {
        lineItemsIds: string[];
        cartName: string;
    }

    export interface IRecurringOrderLineItemsUpdateDateParam {
        CartName: string;
        NextOccurence: string;
    }

    export interface IRecurringOrderProgramsByNamesParam {
        recurringOrderProgramNames: string[];
    }

    export interface IRecurringOrderTemplateLineItemDeleteParam {
        lineItemId: string;
    }

    export interface IRecurringOrderTemplateLineItemUpdateParam {
        paymentMethodId: string;
        shippingAddressId: string;
        billingAddressId: string;
        lineItemId: string;
        nextOccurence: Date;
        frequencyName: string;
        shippingProviderId: string;
        shippingMethodName: string;
    }

    export interface IRecurringOrderTemplateLineItemsDeleteParam {
        lineItemsIds: string[];
    }

    export interface IRecurringOrderTemplateUpdateLineItemQuantityParam {
        lineItemId: string;
        quantity: number;
    }

    export interface IRecurringOrderUpdateLineItemQuantityParam {
        lineItemId: string;
        quantity: number;
        cartName: string;
        recurringProgramName: string;
        recurringFrequencyName: string;
    }

    export interface IRecurringOrderUpdateTemplateAddressParam {
        shippingAddressId: string;
        billingAddressId: string;
        cartName: string;
        useSameForShippingAndBilling: boolean;
    }

    export interface IRecurringOrderUpdateTemplatePaymentMethodParam {
        paymentMethodId: string;
        cartName: string;
        providerName: string;
    }

    export interface IRecurringOrderCartParam {
        cartName: string;
    }

    export interface IRecurringOrderUpdateCartAddressParam {
        shippingAddressId: string;
        billingAddressId: string;
        cartName: string;
        useSameForShippingAndBilling: boolean;
    }

    export interface IRecurringOrderGetCartPaymentMethods {
        cartName: string;
    }

    export interface IRecurringOrderCartUpdatePaymentMethodParam {
        cartName: string;
        paymentId: string;
        paymentProviderName: string;
        paymentType: string;
        paymentMethodId: string;
    }

    export interface IRecurringOrderGetTemplatePaymentMethods {
        id: string;
    }
}
