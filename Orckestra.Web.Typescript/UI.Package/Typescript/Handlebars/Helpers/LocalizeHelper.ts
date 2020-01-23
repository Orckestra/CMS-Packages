///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='./IHandlebarsLocalization.ts' />

Handlebars.registerHelper('localize', function(categoryName, keyName) {
    var value = (<Orckestra.Composer.IHandlebarsLocalization>Handlebars).localizationProvider.handleBarsHelper_localize(categoryName, keyName);
    return new Handlebars.SafeString(value);
});
