///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Repositories/SignInHeaderRepository.ts' />
///<reference path='../../Cache/CacheProvider.ts' />

module Orckestra.Composer {
    'use strict';

    export class SignInHeaderService {

        private cacheKey: string = 'SignInHeaderViewModel';
        private cachePolicy: ICachePolicy = { slidingExpiration: 300 }; // 5min
        private cacheProvider: ICacheProvider;
        private signInHeaderRepository: ISignInHeaderRepository;

        constructor(signInHeaderRepository: ISignInHeaderRepository) {

            if (!signInHeaderRepository) {
                throw new Error('Error: signInHeaderRepository is required');
            }

            this.cacheProvider = CacheProvider.instance();
            this.signInHeaderRepository = signInHeaderRepository;
        }

        public getSignInHeader(param: any): Q.Promise<any> {

            return this.getSignInHeaderFromCache(param)
                .fail(reason => {

                    if (this.canHandle(reason)) {
                        return this.getFreshSignInHeader(param);
                    }

                    throw reason;
                });
        }

        private canHandle(reason: any): boolean {

            return reason === CacheError.Expired || reason === CacheError.NotFound;
        }

        public getFreshSignInHeader(param: any): Q.Promise<any> {

            return this.signInHeaderRepository.getSignInHeader(param)
                .then(cart => this.setSignInHeaderToCache(param, cart));
        }

        public buildSignedInCacheKey(param: any): string {

            return this.cacheKey + '.' + param.cultureInfo + '.' + param.isAuthenticated + '.' + param.encryptedCustomerId + '.' + param.websiteId;
        }

        public invalidateCache(): Q.Promise<void> {

            return this.cacheProvider.customCache.fullClear();
        }

        private getSignInHeaderFromCache(param: any): Q.Promise<any> {

            var composedKey = this.buildSignedInCacheKey(param);

            return this.cacheProvider.customCache.get<any>(composedKey);
        }

        private setSignInHeaderToCache(param: any, cart: any): Q.Promise<any> {

            var composedKey = this.buildSignedInCacheKey(param);

            return this.cacheProvider.customCache.set(composedKey, cart, this.cachePolicy);
        }
    }
}