///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/ComposerClient.ts' />
///<reference path='../../Composer.MyAccount/Common/CustomerService.ts' />
///<reference path='../CheckoutCommon/AddressDto.ts' />

module Orckestra.Composer {
    'use strict';

    export class BillingAddressRegisteredCheckoutService {

        protected customerService: ICustomerService;

        public constructor(customerService: ICustomerService) {
            this.customerService = customerService;
        }

         /**
         * Get the customer addresses. The selected billing address is taken from the cart by default.
         * If no address has been set in the cart, the selected billing address corresponds to the preferred address.
         */
        public getBillingAddresses(cart: any): Q.Promise<any> {

            if (!cart) {
                throw new Error('The cart is required');
            }

            return this.customerService.getAddresses()
                .then(addresses => {
                    addresses.AddressesLoaded = true;
                    addresses.SelectedBillingAddressId = this.getSelectedBillingAddressId(cart, addresses);

                    return addresses;
                });
        }

        public getSelectedBillingAddressId(cart: any, addressList: any) : string {

            if (this.isBillingAddressFromCartValid(cart, addressList)) {
                return cart.Payment.BillingAddress.AddressBookId;
            }

            return this.getPreferredBillingAddressId(addressList);
        }

        private isBillingAddressFromCartValid(cart: any, addressList: any) : boolean {

            if (cart.Payment === undefined) {
                return false;
            }

            if (cart.Payment.BillingAddress === undefined) {
                return false;
            }

            return _.any(addressList.Addresses, (address: AddressDto) => address.Id === cart.Payment.BillingAddress.AddressBookId);
        }

        private getPreferredBillingAddressId(addressList: any) : string {

            var preferredBillingAddress = _.find(addressList.Addresses, (address: AddressDto) => address.IsPreferredBilling);

            return preferredBillingAddress === undefined ? undefined : preferredBillingAddress.Id;
        }
    }
}
