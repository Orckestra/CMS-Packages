///<reference path='../../Typings/tsd.d.ts' />

(() => {
    'use strict';

    describe('if_eq Handlebars helper', () => {

        describe('WHEN more than two arguents are passed', () => {
            it('SHOULD throw error', () => {

                expect(() => {
                    var template: HandlebarsTemplateDelegate = Handlebars.compile('{{#if_eq prop1 prop2 prop3 }}equals{{/if_eq}}'),
                        result = template({ prop1: 1, prop2: 1, prop3: 1});
                }).toThrowError(Error);
            });
        });

        describe('WHEN less than than two arguents are passed', () => {
            it('SHOULD throw error', () => {

                expect(() => {
                    var template: HandlebarsTemplateDelegate = Handlebars.compile('{{#if_eq prop1 }}equals{{/if_eq}}'),
                        result = template({});
                }).toThrowError(Error);
            });
        });

        describe('WHEN no arguents are passed', () => {
            it('SHOULD throw error', () => {

                expect(() => {
                    var template: HandlebarsTemplateDelegate = Handlebars.compile('{{#if_eq }}equals{{/if_eq}}'),
                        result = template({});
                }).toThrowError(Error);
            });
        });

        describe('WHEN two arguents are passed that are equal', () => {
            it('SHOULD return true', () => {
                var template: HandlebarsTemplateDelegate = Handlebars.compile('{{#if_eq prop1 prop2 }}equals{{/if_eq}}'),
                    actual = template({ prop1: 2, prop2: 2});

                expect(actual).toEqual('equals');
            });
        });

        describe('WHEN two arguents are passed that are not equal', () => {
            it('SHOULD return false', () => {

                var template: HandlebarsTemplateDelegate = Handlebars.compile('{{#if_eq prop1 prop2 }}equals{{else}}not equals{{/if_eq}}'),
                    actual = template({ prop1: 2, prop2: 1});

                expect(actual).toEqual('not equals');
            });
        });
    });
})();
