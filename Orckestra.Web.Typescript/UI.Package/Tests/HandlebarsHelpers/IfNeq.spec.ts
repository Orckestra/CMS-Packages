///<reference path='../../Typings/tsd.d.ts' />

(() => {
    'use strict';

    describe('if_neq Handlebars helper', () => {

        describe('WHEN more than two arguents are passed', () => {
            it('SHOULD throw error', () => {

                expect(() => {
                    var template: HandlebarsTemplateDelegate = Handlebars.compile('{{#if_neq prop1 prop2 prop3 }}equals{{/if_neq}}'),
                        result = template({ prop1: 1, prop2: 1, prop3: 1 });
                }).toThrowError(Error);
            });
        });

        describe('WHEN less than than two arguents are passed', () => {
            it('SHOULD throw error', () => {

                expect(() => {
                    var template: HandlebarsTemplateDelegate = Handlebars.compile('{{#if_neq prop1 }}equals{{/if_neq}}'),
                        result = template({});
                }).toThrowError(Error);
            });
        });

        describe('WHEN no arguents are passed', () => {
            it('SHOULD throw error', () => {

                expect(() => {
                    var template: HandlebarsTemplateDelegate = Handlebars.compile('{{#if_neq }}equals{{/if_neq}}'),
                        result = template({});
                }).toThrowError(Error);
            });
        });

        describe('WHEN two arguents are passed that are equal', () => {
            it('SHOULD return false', () => {
                var template: HandlebarsTemplateDelegate =
                        Handlebars.compile('{{#if_neq prop1 prop2 }}not equals{{else}}equals{{/if_neq}}'),
                    actual = template({ prop1: 2, prop2: 2 });

                expect(actual).toEqual('equals');
            });
        });

        describe('WHEN two arguents are passed that are not equal', () => {
            it('SHOULD return true', () => {

                var template: HandlebarsTemplateDelegate =
                        Handlebars.compile('{{#if_neq prop1 prop2 }}not equals{{else}}equals{{/if_neq}}'),
                    actual = template({ prop1: 2, prop2: 1 });

                expect(actual).toEqual('not equals');
            });
        });
    });
})();
