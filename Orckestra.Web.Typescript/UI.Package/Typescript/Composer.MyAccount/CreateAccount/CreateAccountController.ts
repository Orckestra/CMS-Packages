///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='../../Utils/UrlHelper.ts' />
///<reference path='../Common/MembershipService.ts' />
///<reference path='../Common/MyAccountEvents.ts' />
///<reference path='../Common/MyAccountStatus.ts' />
///<reference path='../MyAccount/MyAccountController.ts' />

module Orckestra.Composer {

    export class CreateAccountController extends Orckestra.Composer.MyAccountController {

        protected membershipService: IMembershipService = new MembershipService(new MembershipRepository());

        public initialize() {

            super.initialize();
            this.registerSubscriptions();
        }

        protected registerSubscriptions() {

            this.registerFormsForValidation(this.context.container.find('form'));
            this.eventHub.subscribe(MyAccountEvents[MyAccountEvents.AccountCreated], e => this.onAccountCreated(e));
        }

        private onAccountCreated(e: IEventInformation): void {

            var result = e.data;

            if (result.ReturnUrl) {
                window.location.replace(decodeURIComponent(result.ReturnUrl));
            } else {
                this.render('CreateAccount', result);
            }
        }

        public createAccount(actionContext: IControllerActionContext) {

            actionContext.event.preventDefault();

            var formData: any = this.getFormData(actionContext);
            var returnUrlQueryString: string = 'ReturnUrl=';
            var returnUrl: string = '';

            if (window.location.href.indexOf(returnUrlQueryString) > -1) {
                returnUrl = urlHelper.getURLParameter(location.search, 'ReturnUrl');
            }

            var busy = this.asyncBusy({ elementContext: actionContext.elementContext });

            this.membershipService.register(formData, returnUrl)
                .then(result => this.onRegisterFulfilled(result), reason => this.renderFormErrorMessages(reason))
                .fin(() => busy.done())
                .done();
        }

        private onRegisterFulfilled(result: any): void {

            this.eventHub.publish(MyAccountEvents[MyAccountEvents.AccountCreated], { data: result });
            if (result.Status === MyAccountStatus[MyAccountStatus.Success]) {
                this.eventHub.publish(MyAccountEvents[MyAccountEvents.LoggedIn], { data: result });
            }
        }
    }
}
