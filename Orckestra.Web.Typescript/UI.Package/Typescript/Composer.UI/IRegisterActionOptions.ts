///<reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    export interface IRegisterActionOptions {
        actionName: string;
        actionDelegate: Function;
        overwrite?: boolean;
    }
}
