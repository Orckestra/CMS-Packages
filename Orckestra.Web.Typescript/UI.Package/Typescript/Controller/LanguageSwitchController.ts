///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../Mvc/Controller.ts' />

module Orckestra.Composer {
    export class LanguageSwitchController extends Controller {

        private languageSwitchEvent: string = 'languageSwitchEvent';
        private cacheProvider: ICacheProvider;

        public initialize() {
            super.initialize();

            this.cacheProvider = CacheProvider.instance();
        }

        public onLanguageSwitch() {
            this.cacheProvider.defaultCache.set(this.languageSwitchEvent, true);
        }
    }

}