///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Events/EventScheduler.ts' />
///<reference path='../../Mvc/IControllerActionContext.ts' />
///<reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />
///<reference path='../Common/MyAccountEvents.ts' />
///<reference path='../Common/MembershipService.ts' />

module Orckestra.Composer {

    export class AccountHeaderController extends Orckestra.Composer.Controller {

        protected membershipService: IMembershipService = new MembershipService(new MembershipRepository());

        public initialize() {

            super.initialize();
            this.registerSubscriptions();
        }

        protected registerSubscriptions() {

            var scheduler = EventScheduler.instance(MyAccountEvents[MyAccountEvents.LoggedOut]);
            scheduler.setPostEventCallback((data: any) => this.onLoggedOut(data));
        }

        private onLoggedOut(data: any): Q.Promise<any> {
            var promise: Q.Promise<any> = Q.fcall(() => {
                var newLocation = decodeURIComponent(data.ReturnUrl) || window.location.href;
                window.location.replace(newLocation);
            });

            return promise;
        }

        public fullLogout(actionContext: IControllerActionContext) {

            var returnUrlQueryString: string = 'ReturnUrl=';
            var returnUrl: string = '';

            actionContext.event.preventDefault();

            if (window.location.href.indexOf(returnUrlQueryString) > -1) {
                returnUrl = window.location.href.substring(window.location.href.indexOf(returnUrlQueryString)
                    + returnUrlQueryString.length);
            }

            var busy = this.asyncBusy({elementContext: actionContext.elementContext});

            this.membershipService.logout(returnUrl, false)
                .then(result => this.eventHub.publish(MyAccountEvents[MyAccountEvents.LoggedOut], { data: result }))
                .fin(() => busy.done())
                .done();
        }
    }
}
