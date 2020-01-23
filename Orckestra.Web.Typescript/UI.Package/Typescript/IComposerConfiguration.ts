/// <reference path='./Mvc/IControllerConfiguration.ts' />

module Orckestra.Composer {
    export interface IComposerConfiguration {
        plugins?: string[];
        paymentProvider?: {
            name: string;
            origin: string;
            profileId: string;
        };
        controllers: IControllerConfiguration[];
    };
}
