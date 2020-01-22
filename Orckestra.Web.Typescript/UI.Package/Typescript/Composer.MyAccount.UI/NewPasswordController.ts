///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Composer.UI/Controller.ts' />
///<reference path='../Composer.UI/IControllerActionContext.ts' />
///<reference path='../Composer.UI/ISerializeObjectJqueryPlugin.ts' />
///<reference path='./MembershipService.ts' />
///<reference path='./MyAccountEvents.ts' />
///<reference path='./MyAccountController.ts' />

module Orckestra.Composer {

    export class NewPasswordController extends Orckestra.Composer.MyAccountController {

        protected membershipService: IMembershipService = new MembershipService(new MembershipRepository());

        public initialize() {

            super.initialize();
            this.registerSubscriptions();
        }

        protected registerSubscriptions() {

           this.registerFormsForValidation(this.context.container.find('form'));
           this.eventHub.subscribe(MyAccountEvents[MyAccountEvents.PasswordChanged], e => this.onPasswordChanged(e));
        }

        private onPasswordChanged(e: IEventInformation): void {

            var result = e.data;

            if (result.ReturnUrl) {
                window.location.replace(decodeURIComponent(result.ReturnUrl));
            } else {
                this.render('NewPassword', result);
            }
        }

        /**
         * Event triggered when submitting the new password form.
         * @param {IControllerActionContext} actionContext - Event context.
         */
        public newPassword(actionContext: IControllerActionContext): void {

            actionContext.event.preventDefault();

            var formData: any = this.getFormData(actionContext);
            var returnUrlQueryString: string = 'ReturnUrl=';
            var ticketQueryString: string = 'ticket=';
            var returnUrl: string = '';
            var ticket: string = '';

            if (window.location.href.indexOf(returnUrlQueryString) > -1) {
                returnUrl = window.location.href.substring(window.location.href.indexOf(returnUrlQueryString)
                    + returnUrlQueryString.length);
            }

            if (window.location.href.indexOf(ticketQueryString) > -1) {
                ticket = window.location.href.substring(window.location.href.indexOf(ticketQueryString)
                    + ticketQueryString.length);
            }

            var busy = this.asyncBusy({elementContext: actionContext.elementContext});

            this.membershipService.resetPassword(formData, ticket, returnUrl)
                .then(result => this.onResetPasswordFulfilled(result), reason => this.renderFormErrorMessages(reason))
                .fin(() => busy.done())
                .done();
        }

        private onResetPasswordFulfilled(result: any): void {

            this.eventHub.publish(MyAccountEvents[MyAccountEvents.PasswordChanged], { data: result });
        }
    }
}
