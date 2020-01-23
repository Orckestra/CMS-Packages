///<reference path='../../../Typings/tsd.d.ts' />

Handlebars.registerHelper('escape', function(options) {
    var innerContent = options.fn(this);

    var escapedContent = _.escape(innerContent);

    return new Handlebars.SafeString(escapedContent);
});