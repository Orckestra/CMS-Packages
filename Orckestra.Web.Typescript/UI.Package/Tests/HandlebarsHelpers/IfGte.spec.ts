///<reference path='../../Typings/tsd.d.ts' />

(() => {
    'use strict';

    describe('if_gte Handlebars helper', () => {

        var template: HandlebarsTemplateDelegate;

        beforeEach(() => {
            template = Handlebars.compile('{{#if_gte V1 V2}}Primary{{else}}Inverse{{/if_gte}}');
        });

        describe('WHEN left value is greater than right value', () => {
            it('SHOULD render primary template', () => {
                var result = template({ V1: 2, V2: 1 });
                expect(result).toBe('Primary');
            });
        });

        describe('WHEN both values of if_gte are equal', () => {
            it('SHOULD render primary template', () => {
                var result = template({ V1: 1, V2: 1 });
                expect(result).toBe('Primary');
            });
        });

        describe('WHEN left value of if_gte is not greater than right value', () => {
            it('SHOULD render inverse template', () => {
                var result = template({ V1: 1, V2: 2 });
                expect(result).toBe('Inverse');
            });
        });
    });
})();
