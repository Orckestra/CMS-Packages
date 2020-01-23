///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Events/EventScheduler.ts' />
///<reference path='../../Cache/CacheProvider.ts' />
///<reference path='../Common/MembershipService.ts' />
///<reference path='../Common/MyAccountEvents.ts' />
///<reference path='../Common/MyAccountStatus.ts' />

module Orckestra.Composer {
    /**
     * Controller for the Coupons section.
     */
    export class ReturningCustomerController extends Orckestra.Composer.Controller {

        protected membershipService: IMembershipService = new MembershipService(new MembershipRepository());
        protected cacheProvider: ICacheProvider = CacheProvider.instance();

        public initialize() {

            super.initialize();
            this.registerSubscriptions();
        }

        protected registerSubscriptions() {

           this.registerFormsForValidation(this.context.container.find('form'));

           var scheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedIn]);
            scheduler.setPostEventCallback((data: any) => this.onLoggedIn(data));
        }

        private onLoggedIn(data: any): Q.Promise<any> {

            var promise: Q.Promise<any> = Q.fcall(() => {
                if (data.ReturnUrl) {
                    window.location.replace(decodeURIComponent(data.ReturnUrl));
                } else {
                    this.render('ReturningCustomer', data);

                    this.registerFormsForValidation(this.context.container.find('form'), {
                        serverValidationContainer: '[data-templateid="ReturningCustomerFormsServerValidations"]'
                    });
                }
            });

            return promise;
        }

        /**
         * Event triggered when submitting the login form.
         * @param {IControllerActionContext} actionContext - Event context.
         */
        public login(actionContext: IControllerActionContext) {

            actionContext.event.preventDefault();

            var busy: UIBusyHandle = this.asyncBusy();

            this.loginImpl(actionContext)
                .then(result => this.onLoginFulfilled(result, busy))
                .fail(reason => this.onLoginRejected(reason, busy))
                .done();
        }

        private loginImpl(actionContext: IControllerActionContext): Q.Promise<any> {

            var formData: any = (<ISerializeObjectJqueryPlugin>actionContext.elementContext).serializeObject();
            var href: string = window.location.href;
            var returnUrlKey: string = 'ReturnUrl=';
            var returnUrl: string = href.indexOf(returnUrlKey) > -1 ? href.substring(href.indexOf(returnUrlKey) + returnUrlKey.length) : '';

            return this.membershipService.login(formData, returnUrl);
        }

        private onLoginFulfilled(result: any, busy: UIBusyHandle) {

            if (result.Status === MyAccountStatus[MyAccountStatus.Success]) {
                this.eventHub.publish(MyAccountEvents[MyAccountEvents.LoggedIn], { data: result });
                this.cacheProvider.defaultCache.set('customerId', null).done();
            } else {
                this.renderFailedForm(result.Status);
                busy.done();
            }
        }

        private onLoginRejected(reason: any, busy: UIBusyHandle) {
            let errorCode = MyAccountStatus[MyAccountStatus.AjaxFailed];
            if (reason && reason.Errors && reason.Errors[0] && reason.Errors[0].ErrorCode) {
                errorCode = reason.Errors[0].ErrorCode;
            }
            this.renderFailedForm(errorCode);
            busy.done();
        }

        /**
         * Render the template for message failures
         * Register Format validation to hide those server message on client interaction
         * Reset potentially unsafe fields
         */
        private renderFailedForm(status: string) {

            this.render('ReturningCustomerFormsServerValidations', { Status: status });

            this.context.container.find('input[type="password"]').val('');

            this.registerFormsForValidation(this.context.container.find('form'), {
                serverValidationContainer: '[data-templateid="ReturningCustomerFormsServerValidations"]'
            });
        }
    }
}
