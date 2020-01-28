///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
///<reference path='./MembershipService.ts' />

module Orckestra.Composer {

    export enum MyAccountEvents {
        AccountCreated,
        AccountUpdated,
        AddressCreated,
        AddressUpdated,
        AddressDeleted,
        LoggedIn,
        LoggedOut,
        PasswordChanged,
        ForgotPasswordInstructionSent
    };
}
