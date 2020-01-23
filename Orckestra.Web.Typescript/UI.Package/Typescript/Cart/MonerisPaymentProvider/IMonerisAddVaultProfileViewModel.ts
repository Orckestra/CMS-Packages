/// <reference path="../CheckoutPayment/ViewModels/IActivePaymentViewModel.ts" />

module Orckestra.Composer {
    export interface IMonerisAddVaultProfileViewModel {
        /**
         * Determines if the request was successful.
         */
        Success: boolean;

        /**
         * Code of the error.
         */
        ErrorCode: string;

        /**
         * Message given with the error.
         */
        ErrorMessage: string;

        /** 
         * Active Payment ViewModel.
         */
        ActivePaymentViewModel: IActivePaymentViewModel;
    }
}
