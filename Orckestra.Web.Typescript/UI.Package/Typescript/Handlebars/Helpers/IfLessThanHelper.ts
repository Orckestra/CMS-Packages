///<reference path='../../../Typings/tsd.d.ts' />

Handlebars.registerHelper('if_lt', function(left, right, options) {
    if (left < right) {
        return options.fn(this);
    } else {
        return options.inverse(this);
    }
});
