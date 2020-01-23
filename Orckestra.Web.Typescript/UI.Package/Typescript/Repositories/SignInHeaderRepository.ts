/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='../Mvc/ComposerClient.ts' />
/// <reference path='./ISignInHeaderRepository.ts' />

module Orckestra.Composer {

    export class SignInHeaderRepository implements Orckestra.Composer.ISignInHeaderRepository {

        public getSignInHeader(param: any): Q.Promise<any> {

            return ComposerClient.get('/api/membership/signin');
        }    }
}
