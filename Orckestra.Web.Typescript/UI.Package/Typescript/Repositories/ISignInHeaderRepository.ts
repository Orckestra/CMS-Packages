/// <reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    /**
     * Separates the logic that retrieves the data and maps it to the entity model from the application services that acts on the model.
    */
    export interface ISignInHeaderRepository {

        /**
         * Get the SignInHeader
         */
        getSignInHeader(param: any): Q.Promise<any>;
    }
}