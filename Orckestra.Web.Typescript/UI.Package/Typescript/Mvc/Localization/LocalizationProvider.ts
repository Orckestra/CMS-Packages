///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../Controller.ts' />
///<reference path='../ComposerClient.ts' />
///<reference path='../IControllerActionContext.ts' />
///<reference path='../../Events/IEventInformation.ts' />
///<reference path='./ILocalizationProvider.ts' />

module Orckestra.Composer {
    export class LocalizationProvider implements Orckestra.Composer.ILocalizationProvider {

        private static _instance: Orckestra.Composer.ILocalizationProvider = new LocalizationProvider();
        private _localizationTree: any = {};
        private _composerContext: IComposerContext;

        public static instance(): Orckestra.Composer.ILocalizationProvider {
            return LocalizationProvider._instance;
        }

        constructor() {
            if (LocalizationProvider._instance) {
                throw new Error('Error: Instantiation failed: Use LocalizationProvider.instance() instead of new.');
            }

            LocalizationProvider._instance = this;
        }

        /**
         * Boostrap the localization.
         *
         * This call is responsibly for refreshing all localization
         * to the current page culture
         */
        public initialize(composerContext: IComposerContext): Q.Promise<any> {

            this._composerContext = composerContext;

            var provider = this;
            var language = this._composerContext.language;

            return ComposerClient.get('/api/localization/' + language)
                .then((result) => {
                    var tree = result;

                    provider._localizationTree = tree;
                });
        }

        /**
         * Get a  localized string
         *
         * @param categoryName The category used to bundle this localization
         * @param keyName The exact key to localize
         * @return the localizedString or null if none found
        */
        public getLocalizedString(categoryName: string, keyName: string) {
            categoryName   = (categoryName || '').toLowerCase();

            var tree       = (this._localizationTree || {});
            var categories = (tree.LocalizedCategories || {});
            var category   = (categories[categoryName] || {});
            var values     = (category.LocalizedValues || {});

            var value = values[keyName];
            return value;
        }

        /*
         * Note: not really protected, this is a Typescript scoped method
         * called from the javascript scoped handlebars helper.
         */
        protected handleBarsHelper_localize(categoryName: string, keyName: string) {
            var value = this.getLocalizedString(categoryName, keyName);

            if (_.isUndefined(value)) {
                value = '[' + categoryName + '.' + keyName + ']';
            }

            return value;
        }

        /*
         * Note: not really protected, this is a Typescript scoped method
         * called from the javascript scoped handlebars helper.
         */
        protected handleBarsHelper_localizeFormat(categoryName: string, keyName: string, options: any[]) {
            var value;
            var format = this.getLocalizedString(categoryName, keyName);

            if (_.isUndefined(format)) {
                value = '[' + categoryName + '.' + keyName + ']';
            } else {
                value = this.stringFormat(format, options);
            }

            return value;
        }

        /*
         * Note: not really protected, this is a Typescript scoped method
         * called from the javascript scoped handlebars helper.
         */
        protected handleBarsHelper_isLocalized(categoryName: string, keyName: string) {
            var value = this.getLocalizedString(categoryName, keyName);

            if (_.isEmpty(value) || _.isUndefined(value)) {
                return false;
            }

            return true;
        }

        /*
         * Substitute to String.Format in C#
         * This is a limited version to support numeric placeholders {0} without formatting
         */
        private stringFormat(format: string, options: any[]) {
            return format.replace(/\{\s*([^}\s]+)\s*\}/g, (m, p1, offset, string) => {
                return options[p1] !== void 0 ? options[p1] : p1;
            });
        }
    }
}
