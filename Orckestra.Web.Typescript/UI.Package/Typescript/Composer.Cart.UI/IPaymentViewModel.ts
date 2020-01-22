///<reference path='./IPaymentMethodViewModel.ts' />
///<reference path='./IActivePaymentViewModel.ts' />

module Orckestra.Composer {
    'use strict';

    export interface IPaymentViewModel {
        PaymentId: string;
        PaymentMethods: Array<IPaymentMethodViewModel>;
        UponReceptionPaymentMethodViewModels: Array<IPaymentMethodViewModel>;
        OnlinePaymentMethodViewModels: Array<IPaymentMethodViewModel>;
        CreditCardPaymentMethod: IPaymentMethodViewModel;
        SavedCreditCards: Array<IPaymentMethodViewModel>;
        IsSavedCreditCardSelected: boolean;
        IsLoading: boolean;
        IsProviderLoading: boolean;
        ActivePaymentViewModel: IActivePaymentViewModel;
    }
}
