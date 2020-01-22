///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Composer.UI/Controller.ts' />
///<reference path='../Composer.UI/IControllerActionContext.ts' />
///<reference path='../Composer.UI/ISerializeObjectJqueryPlugin.ts' />
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
