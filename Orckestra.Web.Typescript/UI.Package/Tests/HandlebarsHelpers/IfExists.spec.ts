///<reference path='../../Typings/tsd.d.ts' />

(() => {
    'use strict';

    describe('if_exists Handlebars helper', () => {

        var templateString: string,
            template: HandlebarsTemplateDelegate;

        beforeEach(() => {
            templateString = '{{#if_exists Value}}yolo{{else}}nolo{{/if_exists}}';
            template = Handlebars.compile(templateString);
        });

        describe('WHEN Parameter is not null', () => {
            it('SHOULD render primary template', () => {
                var result = template({ Value: 'Test' });
                expect(result).toBe('yolo');
            });
        });

        describe('WHEN Parameter is undefined', () => {
            it('SHOULD render inverse template', () => {
                var result = template({ NotValue: true });
                expect(result).toBe('nolo');
            });
        });
    });
})();
