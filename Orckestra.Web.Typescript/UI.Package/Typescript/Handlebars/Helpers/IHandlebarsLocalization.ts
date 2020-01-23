///<reference path='../../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    'use strict';

    export interface IHandlebarsLocalization extends HandlebarsStatic {
        localizationProvider: {
            handleBarsHelper_localizeFormat(categoryName: string, keyName: string, args: any): string;
            handleBarsHelper_localize(categoryName: string, keyName: string): string;
            handleBarsHelper_isLocalized(categoryName: string, keyName: string): boolean;
        };
    }
}
