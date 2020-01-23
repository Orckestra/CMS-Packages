///<reference path='../../Typings/tsd.d.ts' />
/// <reference path='../Validation/IParsleyValidator.ts' />
/// <reference path='../ComposerContext.ts' />
/// <reference path='./IPlugin.ts' />

module Orckestra.Composer {
    export class ComposerValidationLocalizationPlugin implements IPlugin {
        public initialize(window: Window, document: HTMLDocument) {
            var locale: string,
                parlseyConfig = (<IParsleyValidator>window).ParsleyConfig,
                parsleyLocaleMessages =
                    JSON.parse((<any>Orckestra.Composer).Templates['GlobalValidation']()),
                composerContext = new ComposerContext();

            locale = composerContext.language;

            if (_.isEmpty(locale)) {
                throw new Error('The locale has not been set');
            }

            (<IParsleyValidator>window).ParsleyConfig = (<IParsleyValidator>window).ParsleyConfig || {};
            (<IParsleyValidator>window).ParsleyConfig.i18n = (<IParsleyValidator>window).ParsleyConfig.i18n || {};

            (<IParsleyValidator>window).ParsleyConfig.i18n[locale] =
                jQuery.extend((<IParsleyValidator>window).ParsleyConfig.i18n[locale] || {}, parsleyLocaleMessages);

            // If file is loaded after Parsley main file, auto-load locale
            if ((<IParsleyValidator>window).ParsleyValidator !== void 0) {
              (<IParsleyValidator>window).ParsleyValidator.addCatalog(locale, (<IParsleyValidator>window).ParsleyConfig.i18n[locale], true);
              (<IParsleyValidator>window).ParsleyValidator.setLocale(locale);
            }

            this.defineValidators((<IParsleyValidator>window).ParsleyValidator);
        }

        protected defineValidators(parsleyValidator: any) {
            var regex: RegExp =  /^(?!(.|\n)*<[a-z!\/?])(?!(.|\n)*&#)(.|\n)*$/i;

            parsleyValidator.addValidator('antixss', (value, requirement) => {
                var isReq: boolean;

                if (_.isString(requirement)) {
                    isReq = (<string>requirement).toLowerCase() === 'true';
                } else {
                    isReq = !!requirement;
                }

                var isValid: boolean = !isReq || regex.test(value);
                return isValid;
            });

            //en: 'This field contains invalid characters.',
            //fr: 'Ce champ contient des caractères invalides.'
        }
    }
}
