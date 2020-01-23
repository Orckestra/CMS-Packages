///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='./IHandlebarsLocalization.ts' />

Handlebars.registerHelper('localizeFormat', function(categoryName, keyName) {
    var args = [];
    if (arguments.length > 2) {
        args = Array.prototype.slice.call(arguments, 2);
    }

    var value = (<Orckestra.Composer.IHandlebarsLocalization>Handlebars).localizationProvider
        .handleBarsHelper_localizeFormat(categoryName, keyName, args);

    return new Handlebars.SafeString(value);
});
