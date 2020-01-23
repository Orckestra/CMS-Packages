///<reference path='../../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    /**
     * Localization Provider for accessing localized string
    */
    export interface ILocalizationProvider {
        /**
         * Boostrap the localization.
         */
        initialize(composerContext: IComposerContext): Q.Promise<any>;

        /**
         * Get a  localized string
         *
         * @param categoryName The category used to bundle this localization
         * @param keyName The exact key to localize
         * @return the localizedString or null if none found
        */
        getLocalizedString(categoryName: string, keyName: string): string;
    }
}
