module Orckestra.Composer {
    'use strict';

    export interface IPaymentMethodViewModel {
        Id: string;
        PaymentProviderName: string;
        DisplayName: string;
        IsSelected: boolean;
        JsonContext: any;
        PaymentType: string;
        Default: boolean;
        IsValid: boolean;
    }
}
