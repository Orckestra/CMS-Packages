///<reference path='../../Typings/tsd.d.ts' />

(() => {
    'use strict';

    describe('if_lte Handlebars helper', () => {

        var template: HandlebarsTemplateDelegate;

        beforeEach(() => {
            template = Handlebars.compile('{{#if_lte V1 V2}}Primary{{else}}Inverse{{/if_lte}}');
        });

        describe('WHEN left value of if_lte is less than right value', () => {
            it('SHOULD render primary template', () => {
                var result = template({ V1: 1, V2: 2 });
                expect(result).toBe('Primary');
            });
        });

        describe('WHEN both values of if_lte are equal', () => {
            it('SHOULD render primary template', () => {
                var result = template({ V1: 1, V2: 1 });
                expect(result).toBe('Primary');
            });
        });

        describe('WHEN left value of if_lte is not less than left value', () => {
            it('SHOULD render inverse template', () => {
                var result = template({ V1: 2, V2: 1 });
                expect(result).toBe('Inverse');
            });
        });
    });
})();
