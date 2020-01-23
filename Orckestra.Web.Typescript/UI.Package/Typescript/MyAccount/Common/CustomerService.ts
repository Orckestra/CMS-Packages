///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Repositories/CustomerRepository.ts' />
///<reference path='./ICustomerService.ts' />

module Orckestra.Composer {
    'use strict';

    export class CustomerService  implements ICustomerService {

        private memoizeGetAdresses: Function;

        protected customerRepository: ICustomerRepository;

        public constructor(customerRepository: ICustomerRepository) {

            if (!customerRepository) {
                throw new Error('Error: customerRepository is required');
            }

            this.customerRepository = customerRepository;
        }

        /**
        * Attempt to register using Composer API.
        * @param
        */
        public updateAccount(formData: any, returnUrl: string): Q.Promise<any> {

            return this.customerRepository.updateAccount(formData, returnUrl);
        }

         /**
         * Get the customer addresses.
         */
        public getAddresses(): Q.Promise<any> {

             if (_.isUndefined(this.memoizeGetAdresses)) {
                this.memoizeGetAdresses = _.memoize(arg => this.getAddressesImpl());
            }

            return this.memoizeGetAdresses();
        }

        private getAddressesImpl(): Q.Promise<any> {

             return this.customerRepository.getAddresses();
        }

        /**
         * Get the customer addresses for a recurring cart page.
         */
        public getRecurringCartAddresses(cartName: string): Q.Promise<any> {

            if (_.isUndefined(this.memoizeGetAdresses)) {
               this.memoizeGetAdresses = _.memoize(arg => this.getRecurringCartAddressesImpl(cartName));
           }

           return this.memoizeGetAdresses();
        }

        private getRecurringCartAddressesImpl(cartName: string): Q.Promise<any> {
            return this.customerRepository.getRecurringCartAddresses(cartName);
        }

        /**
         * Get the customer addresses for a recurring cart page.
         */
        public getRecurringTemplateAddresses(id: string): Q.Promise<any> {

            if (_.isUndefined(this.memoizeGetAdresses)) {
               this.memoizeGetAdresses = _.memoize(arg => this.getRecurringTemplateAddressesImpl(id));
           }

           return this.memoizeGetAdresses();
        }

        private getRecurringTemplateAddressesImpl(id: string): Q.Promise<any> {

            return this.customerRepository.getRecurringTemplateAddresses(id);
        }

        /**
        * Create a new customer address
        * @param
        */
        public createAddress(formData: any, returnUrl: string): Q.Promise<any> {

            return this.customerRepository.createAddress(formData, returnUrl);
        }

        /**
        * Update a customer address
        * @param
        */
        public updateAddress(formData: any, addressId: string, returnUrl: string): Q.Promise<any> {

            return this.customerRepository.updateAddress(formData, addressId, returnUrl);
        }

        /**
        * Delete a customer address
        * @param
        */
        public deleteAddress(addressId: JQuery, returnUrl: string): Q.Promise<any> {
            this.memoizeGetAdresses = undefined;
            return this.customerRepository.deleteAddress(addressId, returnUrl);
        }

        /**
        * Set default address for a customer
        * @param
        */
        public setDefaultAddress(addressId: string, returnUrl: string): Q.Promise<any> {

            return this.customerRepository.setDefaultAddress(addressId, returnUrl);
        }
    }
}
