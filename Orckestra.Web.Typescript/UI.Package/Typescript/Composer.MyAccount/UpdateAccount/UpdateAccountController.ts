///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='../../Utils/UrlHelper.ts' />
///<reference path='../Common/CustomerService.ts' />
///<reference path='../SignInHeader/SignInHeaderService.ts' />
///<reference path='../Common/MyAccountEvents.ts' />
///<reference path='../MyAccount/MyAccountController.ts' />

module Orckestra.Composer {

    export class UpdateAccountController extends Orckestra.Composer.MyAccountController {

        protected customerService: ICustomerService = new CustomerService(new CustomerRepository());
        protected signInHeaderService: SignInHeaderService = new SignInHeaderService(new SignInHeaderRepository());

        public initialize() {

            super.initialize();
            this.registerSubscriptions();
        }

        protected registerSubscriptions() {

            this.registerFormsForValidation(this.context.container.find('form'));
            this.eventHub.subscribe(MyAccountEvents[MyAccountEvents.AccountUpdated], e => this.onAccountUpdated(e));
        }

        private onAccountUpdated(e: IEventInformation): void {

            var result = e.data;

            if (result.ReturnUrl) {
                window.location.replace(decodeURIComponent(result.ReturnUrl));
            } else {

                this.render('UpdateAccount', result);
                this.registerFormsForValidation(this.context.container.find('form'), {
                    serverValidationContainer: '[data-templateid="UpdateAccountSuccessful"]'
                });
            }
        }

        public enableSubmitButton(actionContext: IControllerActionContext): void {

            $('#UpdateAccountSubmit').prop('disabled', false);
        }

        public updateAccount(actionContext: IControllerActionContext): void {

            actionContext.event.preventDefault();

            var formData: any = this.getFormData(actionContext);
            var returnUrlQueryString: string = 'ReturnUrl=';
            var returnUrl: string = '';

            if (window.location.href.indexOf(returnUrlQueryString) > -1) {
                returnUrl = urlHelper.getURLParameter(location.search, 'ReturnUrl');
            }

            var busy = this.asyncBusy({elementContext: actionContext.elementContext});

            this.customerService.updateAccount(formData, returnUrl)
                .then((result) => {
                    this.signInHeaderService.invalidateCache();
                    return result;
                })
                .then(result => this.onUpdateAccountFulfilled(result))
                .fail((reason) => {
                    console.error('Error updating the account.', reason);
                    this.renderFormErrorMessages(reason);

                })
                .fin(() => busy.done());
        }

        protected onUpdateAccountFulfilled(result: any): Q.Promise<any> {

            this.eventHub.publish(MyAccountEvents[MyAccountEvents.AccountUpdated], { data: result });

            return Q(result);
        }
    }
}
