///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='../Common/MembershipService.ts' />
///<reference path='../Common/MyAccountEvents.ts' />
///<reference path='../Common/MyAccountStatus.ts' />
///<reference path='../MyAccount/MyAccountController.ts' />

module Orckestra.Composer {

    export class ForgotPasswordController extends Orckestra.Composer.MyAccountController {

        protected membershipService: IMembershipService = new MembershipService(new MembershipRepository());

        public initialize() {

            super.initialize();
            this.registerSubscriptions();
        }

        protected registerSubscriptions() {

           this.registerFormsForValidation(this.context.container.find('form'));
           this.eventHub.subscribe(MyAccountEvents[MyAccountEvents.ForgotPasswordInstructionSent], e => this.onForgotPasswordInstructionSent(e));
        }

        private onForgotPasswordInstructionSent(e: IEventInformation) {

            var result = e.data;

            if (result.ReturnUrl) {
                window.location.replace(decodeURIComponent(result.ReturnUrl));
            } else {
                this.render('ForgotPassword', result);
            }
        }

        /**
         * Event triggered when submitting the forgot password form.
         * @param {IControllerActionContext} actionContext - Event context.
         */
        public forgotPassword(actionContext: IControllerActionContext): void {

            actionContext.event.preventDefault();

            var formData: any = this.getFormData(actionContext);
            var busy = this.asyncBusy({elementContext: actionContext.elementContext});

            this.membershipService.forgotPassword(formData)
                .then(result => this.onForgotPasswordFulfilled(result), reason => this.renderFormErrorMessages(reason))
                .fin(() => busy.done())
                .done();
        }

        private onForgotPasswordFulfilled(result: any): void {

            this.eventHub.publish(MyAccountEvents[MyAccountEvents.ForgotPasswordInstructionSent], { data: result });
        }
    }
}
