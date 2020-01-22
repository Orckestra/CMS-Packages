///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Composer.UI/Controller.ts' />
///<reference path='../Composer.UI/IControllerActionContext.ts' />
///<reference path='../Composer.UI/ISerializeObjectJqueryPlugin.ts' />
///<reference path='./MembershipService.ts' />

module Orckestra.Composer {

    export enum MyAccountStatus {
        Success,
        InvalidTicket,
        DuplicateEmail,
        DuplicateUserName,
        InvalidQuestion,
        InvalidPassword,
        InvalidPasswordAnswer,
        InvalidEmail,
        Failed,
        UserRejected,
        RequiresApproval,
        AjaxFailed
    };
}
