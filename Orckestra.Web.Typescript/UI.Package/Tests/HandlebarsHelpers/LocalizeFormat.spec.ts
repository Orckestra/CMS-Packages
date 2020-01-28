///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../../Typescript/IComposerContext.ts' />
///<reference path='../../Typescript/Mvc/Localization/LocalizationProvider.ts' />

(() => {
    'use strict';

    describe('LocalizeFormat Handlebars helper', () => {
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
                var template: HandlebarsTemplateDelegate = Handlebars.compile('{{localizeFormat CategoryName KeyName}}');
                var result = template({ 'CategoryName': 'NotFoundCategory', 'KeyName': 'NotFoundKeyname' });

                expect(result).toEqual('[NotFoundCategory.NotFoundKeyname]');
            });
        });

        describe('WHEN Localized keyname is not found', () => {
            it('SHOULD render formatted key hint', () => {
                var template: HandlebarsTemplateDelegate = Handlebars.compile('{{localizeFormat CategoryName KeyName}}');
                var result = template({ 'CategoryName': 'SomeCategory', 'KeyName': 'NotFoundKeyname' });

                expect(result).toEqual('[SomeCategory.NotFoundKeyname]');
            });
        });

        describe('WHEN Localized value is found', () => {
            it('SHOULD render LocalizedValue', () => {
                var template: HandlebarsTemplateDelegate = Handlebars.compile('{{localizeFormat CategoryName KeyName}}');
                var result = template({ 'CategoryName': 'SomeCategory', 'KeyName': 'SomeKey' });

                expect(result).toEqual('SomeValue');
            });
        });

        describe('WHEN Localized value is found and empty', () => {
            it('SHOULD render empty', () => {
                var template: HandlebarsTemplateDelegate = Handlebars.compile('{{localizeFormat CategoryName KeyName}}');
                var result = template({ 'CategoryName': 'SomeCategory', 'KeyName': 'EmptyValue' });

                expect(result).toEqual('');
            });
        });

        describe('WHEN Localized value is found and whitespace', () => {
            it('SHOULD render whitespaces', () => {
                var template: HandlebarsTemplateDelegate = Handlebars.compile('{{localizeFormat CategoryName KeyName}}');
                var result = template({ 'CategoryName': 'SomeCategory', 'KeyName': 'WhitespaceValue' });

                expect(result).toEqual('    ');
            });
        });

        describe('WHEN ValueContains XHTML', () => {
            it('SHOULD use SafeString to render XHTML as is', () => {
                var template: HandlebarsTemplateDelegate = Handlebars.compile('{{localizeFormat CategoryName KeyName}}');
                var result = template({ 'CategoryName': 'SomeCategory', 'KeyName': 'HtmlValue' });

                expect(result).toEqual('This value is <strong>Strong</strong> and  <em>emphasized</em>');
            });
        });

        describe('WHEN Value Contains 1 option', () => {
            var testCases = [
              { format: '{0}',     arg0: 'Boby', expectedResult: 'Boby'},
              { format: '{{0}}',   arg0: 'Boby', expectedResult: '{0}'},
              { format: '{0}',     arg0: '{0}',  expectedResult: '{0}'},
              { format: '{0}',     arg0: '{1}',  expectedResult: '{1}'},
              { format: 'a{0}',    arg0: 'Boby', expectedResult: 'aBoby'},
              { format: 'a {0}',   arg0: 'Boby', expectedResult: 'a Boby'},
              { format: '{0}b',    arg0: 'Boby', expectedResult: 'Bobyb'},
              { format: '{0} b',   arg0: 'Boby', expectedResult: 'Boby b'},
              { format: 'a{0}b',   arg0: 'Boby', expectedResult: 'aBobyb'},
              { format: 'a {0} b', arg0: 'Boby', expectedResult: 'a Boby b'},
              { format: '{0}{0}',  arg0: 'Boby', expectedResult: 'BobyBoby'},
              { format: '{0} {0}', arg0: 'Boby', expectedResult: 'Boby Boby'}
            ];

            _.each(testCases, function(testCase: any) {
                it('SHOULD format ' + testCase.format + ' with arguments', () => {
                    //Set the mocked value into the localization tree
                    var tree = (<any>Orckestra.Composer.LocalizationProvider.instance())._localizationTree;
                    tree.LocalizedCategories['mockcategory'] = {
                        LocalizedValues: {
                            'MockKey': testCase.format
                        }
                    };

                    var template: HandlebarsTemplateDelegate = Handlebars.compile(
                      '{{localizeFormat "MockCategory" "MockKey" Arg0}}'
                    );
                    var result = template({ Arg0: testCase.arg0 });
                    expect(result).toEqual(testCase.expectedResult);
                });
            });
        });

        describe('WHEN value contains 2 options', () => {
            var testCases = [
                { format: '{0}{1}',         arg0: 'Boby', arg1: 'Cool', expectedResult: 'BobyCool'},
                { format: '{1}{0}',         arg0: 'Boby', arg1: 'Cool', expectedResult: 'CoolBoby'},
                { format: '{0} {1}',        arg0: 'Boby', arg1: 'Cool', expectedResult: 'Boby Cool'},
                { format: '{1} {0}',        arg0: 'Boby', arg1: 'Cool', expectedResult: 'Cool Boby'},
                { format: 'a{0}{1}',        arg0: 'Boby', arg1: 'Cool', expectedResult: 'aBobyCool'},
                { format: 'a{1}{0}',        arg0: 'Boby', arg1: 'Cool', expectedResult: 'aCoolBoby'},
                { format: 'a {0}{1}',       arg0: 'Boby', arg1: 'Cool', expectedResult: 'a BobyCool'},
                { format: 'a {1}{0}',       arg0: 'Boby', arg1: 'Cool', expectedResult: 'a CoolBoby'},
                { format: 'a{0} {1}',       arg0: 'Boby', arg1: 'Cool', expectedResult: 'aBoby Cool'},
                { format: 'a{1} {0}',       arg0: 'Boby', arg1: 'Cool', expectedResult: 'aCool Boby'},
                { format: 'a {0} {1}',      arg0: 'Boby', arg1: 'Cool', expectedResult: 'a Boby Cool'},
                { format: 'a {1} {0}',      arg0: 'Boby', arg1: 'Cool', expectedResult: 'a Cool Boby'},
                { format: 'a{0}{1}b',       arg0: 'Boby', arg1: 'Cool', expectedResult: 'aBobyCoolb'},
                { format: 'a{1}{0}b',       arg0: 'Boby', arg1: 'Cool', expectedResult: 'aCoolBobyb'},
                { format: 'a {0}{1} b',     arg0: 'Boby', arg1: 'Cool', expectedResult: 'a BobyCool b'},
                { format: 'a {1}{0} b',     arg0: 'Boby', arg1: 'Cool', expectedResult: 'a CoolBoby b'},
                { format: 'a{0} {1}b',      arg0: 'Boby', arg1: 'Cool', expectedResult: 'aBoby Coolb'},
                { format: 'a{1} {0}b',      arg0: 'Boby', arg1: 'Cool', expectedResult: 'aCool Bobyb'},
                { format: 'a {0} {1} b',    arg0: 'Boby', arg1: 'Cool', expectedResult: 'a Boby Cool b'},
                { format: 'a {1} {0} b',    arg0: 'Boby', arg1: 'Cool', expectedResult: 'a Cool Boby b'},
                { format: '{0}{1}b',        arg0: 'Boby', arg1: 'Cool', expectedResult: 'BobyCoolb'},
                { format: '{1}{0}b',        arg0: 'Boby', arg1: 'Cool', expectedResult: 'CoolBobyb'},
                { format: '{0}{1} b',       arg0: 'Boby', arg1: 'Cool', expectedResult: 'BobyCool b'},
                { format: '{1}{0} b',       arg0: 'Boby', arg1: 'Cool', expectedResult: 'CoolBoby b'},
                { format: '{0} {1}b',       arg0: 'Boby', arg1: 'Cool', expectedResult: 'Boby Coolb'},
                { format: '{1} {0}b',       arg0: 'Boby', arg1: 'Cool', expectedResult: 'Cool Bobyb'},
                { format: '{0} {1} b',      arg0: 'Boby', arg1: 'Cool', expectedResult: 'Boby Cool b'},
                { format: '{1} {0} b',      arg0: 'Boby', arg1: 'Cool', expectedResult: 'Cool Boby b'},
                { format: '{{0}}{{1}}',     arg0: 'Boby', arg1: 'Cool', expectedResult: '{0}{1}'},
                { format: '{0}{1} {1} {0}', arg0: 'Boby', arg1: 'Cool', expectedResult: 'BobyCool Cool Boby'},
                { format: '{0}{1}',         arg0: '{0}',  arg1: '{1}',  expectedResult: '{0}{1}'},
                { format: '{0}{1}',         arg0: '{1}',  arg1: '{0}',  expectedResult: '{1}{0}'}
            ];

            _.each(testCases, function(testCase: any) {
                it('SHOULD format ' + testCase.format + ' with arguments', () => {
                    //Set the mocked value into the localization tree
                    var tree = (<any>Orckestra.Composer.LocalizationProvider.instance())._localizationTree;
                    tree.LocalizedCategories['mockcategory'] = {
                        LocalizedValues: {
                            'MockKey': testCase.format
                        }
                    };

                    var template: HandlebarsTemplateDelegate = Handlebars.compile(
                        '{{localizeFormat "MockCategory" "MockKey" Arg0 Arg1}}'
                    );
                    var result = template({
                        Arg0: testCase.arg0,
                        Arg1: testCase.arg1
                    });
                    expect(result).toEqual(testCase.expectedResult);
                });
            });
        });

        describe('WHEN value contains 10 options', () => {
            var testCases = [
                { format: '{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}',          expectedResult: 'A0B1C2D3E4F5G6H7I8J9'},
                { format: '{9} {8} {7} {6} {5} {4} {3} {2} {1} {0}', expectedResult: 'J9 I8 H7 G6 F5 E4 D3 C2 B1 A0'},
            ];

            _.each(testCases, function(testCase: any) {
                it('SHOULD format ' + testCase.format + ' with arguments', () => {
                    //Set the mocked value into the localization tree
                    var tree = (<any>Orckestra.Composer.LocalizationProvider.instance())._localizationTree;
                    tree.LocalizedCategories['mockcategory'] = {
                        LocalizedValues: {
                            'MockKey': testCase.format
                        }
                    };

                    var template: HandlebarsTemplateDelegate = Handlebars.compile(
                        '{{localizeFormat "MockCategory" "MockKey" Arg0 Arg1 Arg2 Arg3 Arg4 Arg5 Arg6 Arg7 Arg8 Arg9}}'
                    );
                    var result = template({
                        Arg0: 'A0',
                        Arg1: 'B1',
                        Arg2: 'C2',
                        Arg3: 'D3',
                        Arg4: 'E4',
                        Arg5: 'F5',
                        Arg6: 'G6',
                        Arg7: 'H7',
                        Arg8: 'I8',
                        Arg9: 'J9'
                    });
                    expect(result).toEqual(testCase.expectedResult);
                });
            });
        });
    });
})();
