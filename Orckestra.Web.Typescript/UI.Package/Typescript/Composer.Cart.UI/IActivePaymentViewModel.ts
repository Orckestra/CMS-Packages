module Orckestra.Composer {
    export interface IActivePaymentViewModel {
        Id: string;
        PaymentMethodType: string;
        ShouldCapturePayment: boolean;
        CapturePaymentUrl: string;
        PaymentStatus: string;
        ProviderType: string;
        ProviderName: string;
        CanSavePaymentMethod: boolean;
    }
}
