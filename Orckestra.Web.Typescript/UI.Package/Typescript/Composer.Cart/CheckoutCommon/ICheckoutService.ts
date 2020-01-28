///<reference path='../../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    export interface ICheckoutService {

        registerController(controller: IController);

        unregisterController(controllerName: string);

        getCart(): Q.Promise<any>;

        updateCart(): Q.Promise<IUpdateCartResult>;

        completeCheckout(): Q.Promise<ICompleteCheckoutResult>;

        updatePostalCode(postalCode: string): Q.Promise<void>;

        invalidateCache(): Q.Promise<void>;

        setOrderConfirmationToCache(orderConfirmationviewModel : any) : void;

        getOrderConfirmationFromCache(): Q.Promise<any>;

        clearOrderConfirmationFromCache(): void;

        setOrderToCache(orderConfirmationviewModel : any) : void;
    }
}
