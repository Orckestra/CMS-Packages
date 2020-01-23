///<reference path='../../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    export interface ICustomerService {

        /**
        * Attempt to register using Composer API.
        * @param
        */
        updateAccount(formData: any, returnUrl: string): Q.Promise<any>;

        /**
         * Get the customer addresses.
         */
        getAddresses(): Q.Promise<any>;

        /**
         * Get the customer addresses for a recurring cart page.
         */
        getRecurringCartAddresses(cartName: string): Q.Promise<any>;

        /**
         * Get the customer addresses for a recurring template page.
         */
        getRecurringTemplateAddresses(id: string): Q.Promise<any>;

        /**
        * Create a new customer address
        * @param
        */
        createAddress(formData: any, returnUrl: string): Q.Promise<any>;

        /**
        * Update a customer address
        * @param
        */
        updateAddress(formData: any, addressId: string, returnUrl: string): Q.Promise<any>;

        /**
        * Delete a customer address
        * @param
        */
        deleteAddress(addressId: JQuery, returnUrl: string): Q.Promise<any>;

        /**
        * Set default address for a customer
        * @param
        */
        setDefaultAddress(addressId: string, returnUrl: string): Q.Promise<any>;
    }
}