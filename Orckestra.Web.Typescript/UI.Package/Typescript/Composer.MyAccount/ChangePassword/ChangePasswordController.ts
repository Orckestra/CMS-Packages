///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='../Common/MembershipService.ts' />
///<reference path='../Common/MyAccountEvents.ts' />
///<reference path='../MyAccount/MyAccountController.ts' />

module Orckestra.Composer {

    export class ChangePasswordController extends Orckestra.Composer.MyAccountController {

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

                this.render('ChangePassword', result);
                this.registerFormsForValidation(this.context.container.find('form'), {
                    serverValidationContainer: '[data-templateid="ChangePasswordSuccessful"]'
                });
            }
        }

        /**
         * Event triggered when submitting the change password form.
         * @param {IControllerActionContext} actionContext - Event context.
         */
        public changePassword(actionContext: IControllerActionContext): void {

            actionContext.event.preventDefault();

            var formData: any = this.getFormData(actionContext);
            var returnUrlQueryString: string = 'ReturnUrl=';
            var returnUrl: string = '';

            if (window.location.href.indexOf(returnUrlQueryString) > -1) {

                returnUrl = window.location.href.substring(window.location.href.indexOf(returnUrlQueryString)
                    + returnUrlQueryString.length);
            }

            var busy = this.asyncBusy({elementContext: actionContext.elementContext});

            this.membershipService.changePassword(formData, returnUrl)
                .then(result => this.onChangePasswordFulfilled(result), reason => this.renderFormErrorMessages(reason))
                .fin(() => busy.done())
                .done();
        }

        private onChangePasswordFulfilled(result: any): void {

            this.eventHub.publish(MyAccountEvents[MyAccountEvents.PasswordChanged], { data: result });
        }
    }
}
