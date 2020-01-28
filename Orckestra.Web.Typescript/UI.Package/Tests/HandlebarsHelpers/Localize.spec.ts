///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../../Typescript/IComposerContext.ts' />
///<reference path='../../Typescript/Mvc/Localization/LocalizationProvider.ts' />

(() => {
    'use strict';

    describe('Localize Handlebars helper', () => {
        //Bootstrap handlebars
        (<any>Handlebars).localizationProvider = Orckestra.Composer.LocalizationProvider.instance();
        var composerContext: Orckestra.Composer.IComposerContext = {
            language: 'en-CA'
        };

        beforeAll(function(done) {
            jasmine.Ajax.install();

            jasmine.Ajax
                   .stubRequest('/api/localization/en-CA')
                   .andReturn({
                       status: 200,
                       statusText: 'HTTP/1.1 200 OK',
                       contentType: 'application/json; charset=utf-8',
                       responseText: JSON.stringify({
                           CultureName: 'en-CA',
                           LocalizedCategories: {
                               'somecategory': {
                                   CategoryName: 'SomeCategory',
                                   LocalizedValues: {
                                       'SomeKey' : 'SomeValue',
                                       'EmptyValue' : '',
                                       'WhitespaceValue' : '    ',
                                       'HtmlValue' : 'This value is <strong>Strong</strong> and  <em>emphasized</em>'
                                   }
                               }
                           }
                       })
                    });
            Orckestra.Composer.LocalizationProvider.instance().initialize(composerContext).then(done);
        });

        afterAll(function() {
            jasmine.Ajax.uninstall();
        });

        describe('WHEN Localized category is not found', () => {
            it('SHOULD render formatted key hint', () => {
                var template: HandlebarsTemplateDelegate = Handlebars.compile('{{localize CategoryName KeyName}}');
                var result = template({ 'CategoryName': 'NotFoundCategory', 'KeyName': 'NotFoundKeyname' });

                expect(result).toEqual('[NotFoundCategory.NotFoundKeyname]');
            });
        });

        describe('WHEN Localized keyname is not found', () => {
            it('SHOULD render formatted key hint', () => {
                var template: HandlebarsTemplateDelegate = Handlebars.compile('{{localize CategoryName KeyName}}');
                var result = template({ 'CategoryName': 'SomeCategory', 'KeyName': 'NotFoundKeyname' });

                expect(result).toEqual('[SomeCategory.NotFoundKeyname]');
            });
        });

        describe('WHEN Localized value is found', () => {
            it('SHOULD render LocalizedValue', () => {
                var template: HandlebarsTemplateDelegate = Handlebars.compile('{{localize CategoryName KeyName}}');
                var result = template({ 'CategoryName': 'SomeCategory', 'KeyName': 'SomeKey' });

                expect(result).toEqual('SomeValue');
            });
        });

        describe('WHEN Localized value is found and empty', () => {
            it('SHOULD render empty', () => {
                var template: HandlebarsTemplateDelegate = Handlebars.compile('{{localize CategoryName KeyName}}');
                var result = template({ 'CategoryName': 'SomeCategory', 'KeyName': 'EmptyValue' });

                expect(result).toEqual('');
            });
        });

        describe('WHEN Localized value is found and whitespace', () => {
            it('SHOULD render whitespaces', () => {
                var template: HandlebarsTemplateDelegate = Handlebars.compile('{{localize CategoryName KeyName}}');
                var result = template({ 'CategoryName': 'SomeCategory', 'KeyName': 'WhitespaceValue' });

                expect(result).toEqual('    ');
            });
        });

        describe('WHEN ValueContains XHTML', () => {
            it('SHOULD use SafeString to render XHTML as is', () => {
                var template: HandlebarsTemplateDelegate = Handlebars.compile('{{localize CategoryName KeyName}}');
                var result = template({ 'CategoryName': 'SomeCategory', 'KeyName': 'HtmlValue' });

                expect(result).toEqual('This value is <strong>Strong</strong> and  <em>emphasized</em>');
            });
        });
    });
})();
