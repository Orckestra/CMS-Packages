///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Repositories/MembershipRepository.ts' />
///<reference path='./IMembershipService.ts' />

module Orckestra.Composer {
    'use strict';

    export class MembershipService implements IMembershipService {

        protected memoizeIsAuthenticated: Function;
        protected membershipRepository: IMembershipRepository;

        constructor(membershipRepository: IMembershipRepository) {

            if (!membershipRepository) {
                throw new Error('Error: membershipRepository is required');
            }

            this.membershipRepository = membershipRepository;
        }

        /**
        * Attempt to Log In using Composer API.
        * @param
        */
        public login(formData: any, returnUrl: string): Q.Promise<any> {

            return this.membershipRepository.login(formData, returnUrl);
        }

        /**
        * Logout using Composer API.
        * @param
        */
        public logout(returnUrl: string = '', preserveCustomerInfo: boolean = false): Q.Promise<any> {

            return this.membershipRepository.logout(returnUrl, preserveCustomerInfo);
        }

        /**
        * Attempt to register using Composer API.
        * @param
        */
        public register(formData: any, returnUrl: string): Q.Promise<any> {

            return this.membershipRepository.register(formData, returnUrl);
        }

        /**
        * Attempt to trigger the forgot password email using Composer API.
        * @param
        */
        public forgotPassword(formData: any): Q.Promise<any> {

            return this.membershipRepository.forgotPassword(formData);
        }

        /**
        * Attempt to reset password for the Customer identified by the ticket using Composer API.
        * @param
        */
        public resetPassword(formData: any, ticket: string, returnUrl: string): Q.Promise<any> {

            return this.membershipRepository.resetPassword(formData, ticket, returnUrl);
        }

        /**
        * Attempt to change the password for the connected Customer using Composer API.
        * @param
        */
        public changePassword(formData: any, returnUrl: string): Q.Promise<any> {

            return this.membershipRepository.changePassword(formData, returnUrl);
        }

        /**
         * If the current user is authenticated or not
         */
         public isAuthenticated(): Q.Promise<any> {

            if (_.isUndefined(this.memoizeIsAuthenticated)) {

                this.memoizeIsAuthenticated = _.memoize(arg => this.isAuthenticatedImpl());
            }

            return this.memoizeIsAuthenticated();
        }

        /**
         * If the current user is authenticated or not
         */
         protected isAuthenticatedImpl(): Q.Promise<any> {

            return this.membershipRepository.isAuthenticated();
        }
    }
}
