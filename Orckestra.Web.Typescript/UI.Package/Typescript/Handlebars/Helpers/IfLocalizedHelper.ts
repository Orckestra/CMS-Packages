///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='./IHandlebarsLocalization.ts' />

Handlebars.registerHelper('if_localized', function(categoryName, keyName, options) {
    if ((<Orckestra.Composer.IHandlebarsLocalization>Handlebars).localizationProvider.handleBarsHelper_isLocalized(categoryName, keyName)) {
        return options.fn(this);
    } else {
        return options.inverse(this);
    }
});
