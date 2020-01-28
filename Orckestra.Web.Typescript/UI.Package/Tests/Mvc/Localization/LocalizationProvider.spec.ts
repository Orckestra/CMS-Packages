///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../../Typescript/IComposerContext.ts' />
///<reference path='../../../Typescript/Events/EventHub.ts' />
///<reference path='../../../Typescript/Mvc/Localization/LocalizationProvider.ts' />

(() => {

    'use strict';


    describe('WHEN instantiating the LocalizationProvider with a constructor', () => {
        var provider: Orckestra.Composer.ILocalizationProvider;

        it('SHOULD throw an error', () => {
            expect(() => {
                provider = new Orckestra.Composer.LocalizationProvider();
            }).toThrowError(Error);
        });
    });

    describe('WHEN LocalizationProvider.getLocalizedString', () => {
        var provider: Orckestra.Composer.ILocalizationProvider;
        var composerContext: Orckestra.Composer.IComposerContext = {
            language: 'en-CA'
        };

        provider = Orckestra.Composer.LocalizationProvider.instance();

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

            provider.initialize(composerContext).then(done);
        });

        afterAll(function() {
            jasmine.Ajax.uninstall();
        });

        describe('USING a known category and a known key', () => {
            it('SHOULD return the localized value', () => {
                var value = provider.getLocalizedString('SomeCategory', 'SomeKey');
                expect(value).toBe('SomeValue');
            });
        });

        describe('USING a known category and a unknown key', () => {
            it('SHOULD return null', () => {
                var value = provider.getLocalizedString('SomeCategory', 'UnknownKey');
                expect(value).toBeUndefined();
            });
        });

        describe('USING a unknown category', () => {
            it('SHOULD return null', () => {
                var value = provider.getLocalizedString('UnknownCategory', 'SomeKey');
                expect(value).toBeUndefined();
            });
        });

        describe('USING a known category with wrong Case and a known key', () => {
            it('SHOULD return the localized value', () => {
                var value = provider.getLocalizedString('sOmEcAtEgOrY', 'SomeKey');
                expect(value).toBe('SomeValue');
            });
        });

        describe('USING a known category and a known key with wrong Case', () => {
            it('SHOULD return the null', () => {
                var value = provider.getLocalizedString('SomeCategory', 'sOmEkEy');
                expect(value).toBeUndefined();
            });
        });

        describe('USING Category Key resolving to Empty Localized String', () => {
            it('SHOULD return the Localized String', () => {
                var value = provider.getLocalizedString('SomeCategory', 'EmptyValue');
                expect(value).toBe('');
            });
        });

        describe('USING Category Key resolving to Whitespace Localized String', () => {
            it('SHOULD return the Localized String', () => {
                var value = provider.getLocalizedString('SomeCategory', 'WhitespaceValue');
                expect(value).toBe('    ');
            });
        });

        describe('USING Category Key resolving to Html Localized String', () => {
            it('SHOULD return the Localized String', () => {
                var value = provider.getLocalizedString('SomeCategory', 'HtmlValue');
                expect(value).toBe('This value is <strong>Strong</strong> and  <em>emphasized</em>');
            });
        });
    });
})();
