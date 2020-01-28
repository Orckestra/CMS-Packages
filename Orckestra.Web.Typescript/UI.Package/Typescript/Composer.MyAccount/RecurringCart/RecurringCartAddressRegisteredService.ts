///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/ComposerClient.ts' />
///<reference path='../Common/CustomerService.ts' />
///<reference path='../../Composer.Cart/CheckoutCommon/AddressDto.ts' />

module Orckestra.Composer {
    'use strict';

    export class RecurringCartAddressRegisteredService {

        protected customerService: ICustomerService;

        public constructor(customerService: ICustomerService) {
            this.customerService = customerService;
        }

         /**
         * Get the customer addresses. The selected billing/shipping address is taken from the cart by default.
         * If no address has been set in the cart, the selected billing/shipping address corresponds to the preferred address.
         */
        public getRecurringCartAddresses(cart: any): Q.Promise<any> {

            if (!cart) {
                throw new Error('The cart is required');
            }

            return this.customerService.getRecurringCartAddresses(cart.Name)
                .then(addresses => {
                    addresses.AddressesLoaded = true;
                    addresses.SelectedBillingAddressId = this.getSelectedBillingAddressId(cart, addresses);
                    addresses.SelectedShippingAddressId = this.getSelectedShippingAddressId(cart, addresses);

                    return addresses;
                });
        }

        public getRecurringTemplateAddresses(id: any): Q.Promise<any> {

            if (!id) {
                throw new Error('The recurring schedule id is required');
            }

            return this.customerService.getRecurringTemplateAddresses(id)
                .then(addresses => {
                    addresses.AddressesLoaded = true;

                    return addresses;
                });
        }

        public getSelectedBillingAddressId(cart: any, addressList: any) : string {

            if (this.isBillingAddressFromCartValid(cart, addressList)) {
                return cart.Payment.BillingAddress.Id;
            }

            return this.getPreferredBillingAddressId(addressList);
        }

        public isBillingAddressFromCartValid(cart: any, addressList: any) : boolean {

            if (cart.Payment === undefined) {
                return false;
            }

            if (cart.Payment.BillingAddress === undefined) {
                return false;
            }

            return _.any(addressList.Addresses, (address: AddressDto) => address.Id === cart.Payment.BillingAddress.Id);
        }

        public getPreferredBillingAddressId(addressList: any) : string {

            var preferredBillingAddress = _.find(addressList.Addresses, (address: AddressDto) => address.IsPreferredBilling);

            return preferredBillingAddress === undefined ? undefined : preferredBillingAddress.Id;
        }

        public getSelectedShippingAddressId(cart: any, addressList: any) : string {

            if (this.isShippingAddressFromCartValid(cart, addressList)) {
                return cart.ShippingAddress.Id;
            }

            return this.getPreferredShippingAddressId(addressList);
        }

        public isShippingAddressFromCartValid(cart: any, addressList: any) : boolean {

            if (cart.ShippingAddress === undefined) {
                return false;
            }

            return _.any(addressList.Addresses, (address: AddressDto) => address.Id === cart.ShippingAddress.Id);
        }

        public getPreferredShippingAddressId(addressList: any) : string {

            var preferredShippingAddress = _.find(addressList.Addresses, (address: AddressDto) => address.IsPreferredShipping);

            return preferredShippingAddress === undefined ? undefined : preferredShippingAddress.Id;
        }
    }
}
