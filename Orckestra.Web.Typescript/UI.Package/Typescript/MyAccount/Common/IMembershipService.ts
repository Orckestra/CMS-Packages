///<reference path='../../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    export interface IMembershipService {

        /**
        * Attempt to Log In using Composer API.
        * @param
        */
        login(formData: any, returnUrl: string): Q.Promise<any>;

        /**
        * Logout using Composer API.
        * @param
        */
        logout(returnUrl: string, preserveCustomerInfo: boolean): Q.Promise<any>;

        /**
        * Attempt to register using Composer API.
        * @param
        */
        register(formData: any, returnUrl: string): Q.Promise<any>;

        /**
        * Attempt to trigger the forgot password email using Composer API.
        * @param
        */
        forgotPassword(formData: any): Q.Promise<any>;

        /**
        * Attempt to reset password for the Customer identified by the ticket using Composer API.
        * @param
        */
        resetPassword(formData: any, ticket: string, returnUrl: string): Q.Promise<any>;

        /**
        * Attempt to change the password for the connected Customer using Composer API.
        * @param
        */
        changePassword(formData: any, returnUrl: string): Q.Promise<any>;

        /**
         * If the current user is authenticated or not
         */
        isAuthenticated(): Q.Promise<any>;
    }
}