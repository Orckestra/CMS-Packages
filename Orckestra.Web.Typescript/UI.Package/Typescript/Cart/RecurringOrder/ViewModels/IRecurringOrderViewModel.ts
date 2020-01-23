/// <reference path='../../../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    export interface IRecurringOrderProgramFrequencyInfoViewModel {
        RecurringOrderProgramFrequencyId: string;
        RecurrenceDefDescription: string;
    }

    export interface IRecurringOrderProgramEligibleItemResultViewModel {
        CategoryId: string;
        ProductId: string;
        Sku: string;
        VariantId: string;
        IsEligible: boolean;
        Frequencies: Array<IRecurringOrderProgramFrequencyInfoViewModel>;
    }

    export interface IRecurringFrequecyDescriptionListViewModel {
        Frequencies: Array<IRecurringOrderProgramFrequencyInfoViewModel>;
    }

    export interface IRecurringOrderProgramEligibleItemListViewModel {
        Items: Array<IRecurringOrderProgramEligibleItemResultViewModel>;
    }
}
