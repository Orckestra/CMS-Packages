module Orckestra.Composer {
    'use strict';

    export interface ICreateVaultTokenOptions {
        CardHolderName: string;
        VaultTokenId: string;
        PaymentId: string;
        CreatePaymentProfile: boolean;
        PaymentProviderName: string;
    }
}
