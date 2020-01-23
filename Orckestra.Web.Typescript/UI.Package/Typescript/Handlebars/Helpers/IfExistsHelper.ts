///<reference path='../../../Typings/tsd.d.ts' />

Handlebars.registerHelper('if_exists', function(value, options) {
    if (typeof value !== 'undefined') {
        return options.fn(this);
    } else {
        return options.inverse(this);
    }
});
