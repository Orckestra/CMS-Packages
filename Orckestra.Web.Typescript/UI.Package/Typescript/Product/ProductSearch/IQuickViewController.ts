///<reference path='../../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    export interface IQuickViewController {
        selectImage(actionContext: IControllerActionContext): void;
        addLineItem(actionContext: IControllerActionContext): void;
        selectKva(actionContext: IControllerActionContext): void;
    }
}
