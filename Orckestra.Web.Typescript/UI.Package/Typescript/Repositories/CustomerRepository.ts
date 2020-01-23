///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Mvc/ComposerClient.ts' />
///<reference path='./ICustomerRepository.ts' />

module Orckestra.Composer {
    'use strict';

    export class CustomerRepository  implements ICustomerRepository {

        /**
        * Attempt to register using Composer API.
        * @param
        */
        public updateAccount(formData: any, returnUrl: string): Q.Promise<any> {

            var data = _.extend({ ReturnUrl: returnUrl }, formData);
            return ComposerClient.post('/api/customer/update', data);
        }

         /**
         * Get the customer addresses.
         */
        public getAddresses(): Q.Promise<any> {

              return ComposerClient.get('/api/customer/addresses');
        }

        /**
         * Get the customer addresses for a recurring cart page.
         */
        public getRecurringCartAddresses(cartName: string): Q.Promise<any> {

            var data = {
                CartName: cartName
            };
            return ComposerClient.post('/api/customer/recurringcartaddresses', data);
        }

        /**
         * Get the customer addresses for a recurring cart page.
         */
        public getRecurringTemplateAddresses(id: string): Q.Promise<any> {

            var data = {
                id: id
            };
            return ComposerClient.post('/api/customer/recurringorderstemplatesaddresses', data);
        }

        /**
        * Create a new customer address
        * @param
        */
        public createAddress(formData: any, returnUrl: string): Q.Promise<any> {

            var data = _.extend({ ReturnUrl: returnUrl }, formData);
            return ComposerClient.post('/api/customer/addresses', data);
        }

        /**
        * Update a customer address
        * @param
        */
        public updateAddress(formData: any, addressId: string, returnUrl: string): Q.Promise<any> {

            var data = _.extend({ ReturnUrl: returnUrl }, formData);
            return ComposerClient.post('/api/customer/addresses/' + addressId, data);
        }

        /**
        * Delete a customer address
        * @param
        */
        public deleteAddress(addressId: JQuery, returnUrl: string): Q.Promise<any> {

            var data = { ReturnUrl: returnUrl };
            return ComposerClient.remove('/api/customer/addresses/' + addressId, data);
        }

        /**
        * Set default address for a customer
        * @param
        */
        public setDefaultAddress(addressId: string, returnUrl: string): Q.Promise<any> {

            var data = { ReturnUrl: returnUrl };
            return ComposerClient.post('/api/customer/setdefaultaddress/' + addressId, data);
        }
    }
}
