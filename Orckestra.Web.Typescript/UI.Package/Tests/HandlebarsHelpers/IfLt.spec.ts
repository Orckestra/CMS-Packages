///<reference path='../../Typings/tsd.d.ts' />

(() => {
    'use strict';

    describe('if_lt Handlebars helper', () => {

        var template: HandlebarsTemplateDelegate;

        beforeEach(() => {
            template = Handlebars.compile('{{#if_lt V1 V2}}Primary{{else}}Inverse{{/if_lt}}');
        });

        describe('WHEN left value is less than right value', () => {
            it('SHOULD render primary template', () => {
                var result = template({ V1: 1, V2: 2 });
                expect(result).toBe('Primary');
            });
        });

        describe('WHEN left value is not less than right value', () => {
            it('SHOULD render inverse template', () => {
                var result = template({ V1: 2, V2: 1 });
                expect(result).toBe('Inverse');
            });
        });
    });
})();
