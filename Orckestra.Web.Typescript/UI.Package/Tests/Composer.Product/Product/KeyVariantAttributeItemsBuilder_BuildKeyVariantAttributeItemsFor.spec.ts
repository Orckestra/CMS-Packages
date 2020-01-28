/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../Mocks/MockControllerContext.ts' />
/// <reference path='../../Mocks/MockJqueryEventObject.ts' />
/// <reference path='../../../Typescript/Events/EventHub.ts' />
/// <reference path='../../../Typescript/Events/IEventHub.ts' />
/// <reference path='../../../Typescript/Events/IEventInformation.ts' />
/// <reference path='../../../Typescript/Mvc/IControllerActionContext.ts' />
/// <reference path='../../../Typescript/Composer.Product/Product/KeyVariantAttributeItemsBuilder.ts' />

(() => {
    'use strict';

    describe('WHEN calling the KeyVariantAttributeItemsBuilder.BuildKeyVariantAttributeItemsFor method', () => {
        describe('WITH the context.viewModel of a Product without variant', () => {
            var context = <Orckestra.Composer.IControllerContext>{
                viewModel: {
                    /*Without Variants*/
                },
                templateName: null,
                dataItemId: null,
                container: null,
                window: null
            };

            it('SHOULD return an empty ViewModel', () => {
                //Arrange
                var builder = new Orckestra.Composer.KeyVariantAttributeItemsBuilder(context);
                var selectedKvas = {};

                //Act
                var keyVariantAttributeItems = builder.BuildKeyVariantAttributeItemsFor(selectedKvas);

                //Assert
                expect(keyVariantAttributeItems).toEqual([]);
            });
        });

        describe('WITH the context.viewModel of a Product without variant', () => {
            var context = <Orckestra.Composer.IControllerContext>{
                viewModel: {
                    'keyVariantAttributeItems': []
                },
                templateName: null,
                dataItemId: null,
                container: null,
                window: null
            };

            it('SHOULD return an empty ViewModel', () => {
                //Arrange
                var builder = new Orckestra.Composer.KeyVariantAttributeItemsBuilder(context);
                var selectedKvas = {};

                //Act
                var keyVariantAttributeItems = builder.BuildKeyVariantAttributeItemsFor(selectedKvas);

                //Assert
                expect(keyVariantAttributeItems).toEqual([]);
            });
        });

        describe('WITH the context.viewModel of a Product with only 1 KVA', () => {
            var context = <Orckestra.Composer.IControllerContext>{
                viewModel: {
                    'allVariants': [
                        { 'Id': '87403173', 'Kvas': { 'Size': 'large' } },
                        { 'Id': '87403165', 'Kvas': { 'Size': 'medium'} },
                        { 'Id': '87403178', 'Kvas': { 'Size': 'small' } }
                    ],
                    'keyVariantAttributeItems': [
                        { 'PropertyName': 'Size', 'PropertyDataType': 'Lookup', 'Values': [
                            {'Value': 'small' },
                            {'Value': 'medium'},
                            {'Value': 'large' }
                        ] }
                    ]
                },
                templateName: null,
                dataItemId: null,
                container: null,
                window: null
            };

            it('SHOULD return the ViewModel with the selected variant (small)', () => {
                //Arrange
                var builder = new Orckestra.Composer.KeyVariantAttributeItemsBuilder(context);
                var selectedKvas = {'Size': 'small'};

                //Act
                var keyVariantAttributeItems = builder.BuildKeyVariantAttributeItemsFor(selectedKvas);

                //Assert
                var kvaSize = <any>_.find(keyVariantAttributeItems, { 'PropertyName': 'Size' });
                var kvaSizeSmall  = <any>_.find(kvaSize.Values, { 'Value': 'small' });
                var kvaSizeMedium = <any>_.find(kvaSize.Values, { 'Value': 'medium' });
                var kvaSizeLarge  = <any>_.find(kvaSize.Values, { 'Value': 'large' });
                expect(kvaSizeSmall.Selected).toBeTruthy();
                expect(kvaSizeMedium.Selected).toBeFalsy();
                expect(kvaSizeLarge.Selected).toBeFalsy();

                expect(kvaSizeSmall.Disabled).toBeFalsy();
                expect(kvaSizeMedium.Disabled).toBeFalsy();
                expect(kvaSizeLarge.Disabled).toBeFalsy();
            });

            it('SHOULD return the ViewModel with the selected variant (large)', () => {
                //Arrange
                var builder = new Orckestra.Composer.KeyVariantAttributeItemsBuilder(context);
                var selectedKvas = {'Size': 'large'};

                //Act
                var keyVariantAttributeItems = builder.BuildKeyVariantAttributeItemsFor(selectedKvas);

                //Assert
                var kvaSize = <any>_.find(keyVariantAttributeItems, { 'PropertyName': 'Size' });
                var kvaSizeSmall  = <any>_.find(kvaSize.Values, { 'Value': 'small' });
                var kvaSizeMedium = <any>_.find(kvaSize.Values, { 'Value': 'medium' });
                var kvaSizeLarge  = <any>_.find(kvaSize.Values, { 'Value': 'large' });
                expect(kvaSizeSmall.Selected).toBeFalsy();
                expect(kvaSizeMedium.Selected).toBeFalsy();
                expect(kvaSizeLarge.Selected).toBeTruthy();

                expect(kvaSizeSmall.Disabled).toBeFalsy();
                expect(kvaSizeMedium.Disabled).toBeFalsy();
                expect(kvaSizeLarge.Disabled).toBeFalsy();
            });

            it('SHOULD return the ViewModel with the selected variant (nothing)', () => {
                //Arrange
                var builder = new Orckestra.Composer.KeyVariantAttributeItemsBuilder(context);
                var selectedKvas = {'Size': 'somebadvalue'};

                //Act
                var keyVariantAttributeItems = builder.BuildKeyVariantAttributeItemsFor(selectedKvas);

                //Assert
                var kvaSize = <any>_.find(keyVariantAttributeItems, { 'PropertyName': 'Size' });
                var kvaSizeSmall  = <any>_.find(kvaSize.Values, { 'Value': 'small' });
                var kvaSizeMedium = <any>_.find(kvaSize.Values, { 'Value': 'medium' });
                var kvaSizeLarge  = <any>_.find(kvaSize.Values, { 'Value': 'large' });
                expect(kvaSizeSmall.Selected).toBeFalsy();
                expect(kvaSizeMedium.Selected).toBeFalsy();
                expect(kvaSizeLarge.Selected).toBeFalsy();

                expect(kvaSizeSmall.Disabled).toBeFalsy();
                expect(kvaSizeMedium.Disabled).toBeFalsy();
                expect(kvaSizeLarge.Disabled).toBeFalsy();
            });
        });

        describe('WITH the context.viewModel of a Product with 2 KVAs', () => {
            var context = <Orckestra.Composer.IControllerContext>{
                viewModel: {
                    'allVariants': [
                        { 'Id': '87403173', 'Kvas': { 'Size': 'large',  'Colour': 'sunburst'      } },
                        { 'Id': '87403165', 'Kvas': { 'Size': 'medium', 'Colour': 'blue_radiance' } },
                        { 'Id': 'TestLS',   'Kvas': { 'Size': 'large',  'Colour': 'shell_coral'   } },
                        { 'Id': 'TestMS',   'Kvas': { 'Size': 'medium',  'Colour': 'shell_coral'  } },
                        { 'Id': 'TestSS',   'Kvas': { 'Size': 'small',  'Colour': 'shell_coral'   } }
                    ],
                    'keyVariantAttributeItems': [
                        { 'PropertyName': 'Size', 'PropertyDataType': 'Lookup', 'Values': [
                            {'Value': 'small' },
                            {'Value': 'medium'},
                            {'Value': 'large' }
                        ] },
                        { 'PropertyName': 'Colour', 'PropertyDataType': 'Lookup', 'Values': [
                            {'Value': 'sunburst' },
                            {'Value': 'blue_radiance'},
                            {'Value': 'shell_coral' }
                        ] }
                    ]
                },
                templateName: null,
                dataItemId: null,
                container: null,
                window: null
            };

            it('SHOULD return the ViewModel with the selected variant (large,sunburst)', () => {
                //Arrange
                var builder = new Orckestra.Composer.KeyVariantAttributeItemsBuilder(context);
                var selectedKvas = {'Size': 'large', 'Colour': 'sunburst'};

                //Act
                var keyVariantAttributeItems = builder.BuildKeyVariantAttributeItemsFor(selectedKvas);

                //Assert
                //Size
                var kvaSize = <any>_.find(keyVariantAttributeItems, { 'PropertyName': 'Size' });
                var kvaSizeSmall  = <any>_.find(kvaSize.Values, { 'Value': 'small' });
                var kvaSizeMedium = <any>_.find(kvaSize.Values, { 'Value': 'medium' });
                var kvaSizeLarge  = <any>_.find(kvaSize.Values, { 'Value': 'large' });

                expect(kvaSizeSmall.Selected).toBeFalsy();
                expect(kvaSizeMedium.Selected).toBeFalsy();
                expect(kvaSizeLarge.Selected).toBeTruthy();

                expect(kvaSizeSmall.Disabled).toBeTruthy();
                expect(kvaSizeMedium.Disabled).toBeTruthy();
                expect(kvaSizeLarge.Disabled).toBeFalsy();

                //Colour
                var kvaColour = <any>_.find(keyVariantAttributeItems, { 'PropertyName': 'Colour' });
                var kvaColourSunburst     = <any>_.find(kvaColour.Values, { 'Value': 'sunburst' });
                var kvaColourBlueRadiance = <any>_.find(kvaColour.Values, { 'Value': 'blue_radiance' });
                var kvaColourShellCoral   = <any>_.find(kvaColour.Values, { 'Value': 'shell_coral' });
                expect(kvaColourSunburst.Selected).toBeTruthy();
                expect(kvaColourBlueRadiance.Selected).toBeFalsy();
                expect(kvaColourShellCoral.Selected).toBeFalsy();

                expect(kvaColourSunburst.Disabled).toBeFalsy();
                expect(kvaColourBlueRadiance.Disabled).toBeTruthy();
                expect(kvaColourShellCoral.Disabled).toBeFalsy();
            });

            it('SHOULD return the ViewModel with the selected variant (medium,blue_radiance)', () => {
                //Arrange
                var builder = new Orckestra.Composer.KeyVariantAttributeItemsBuilder(context);
                var selectedKvas = {'Size': 'medium', 'Colour': 'blue_radiance'};

                //Act
                var keyVariantAttributeItems = builder.BuildKeyVariantAttributeItemsFor(selectedKvas);

                //Assert
                //Size
                var kvaSize = <any>_.find(keyVariantAttributeItems, { 'PropertyName': 'Size' });
                var kvaSizeSmall  = <any>_.find(kvaSize.Values, { 'Value': 'small' });
                var kvaSizeMedium = <any>_.find(kvaSize.Values, { 'Value': 'medium' });
                var kvaSizeLarge  = <any>_.find(kvaSize.Values, { 'Value': 'large' });

                expect(kvaSizeSmall.Selected).toBeFalsy();
                expect(kvaSizeMedium.Selected).toBeTruthy();
                expect(kvaSizeLarge.Selected).toBeFalsy();

                expect(kvaSizeSmall.Disabled).toBeTruthy();
                expect(kvaSizeMedium.Disabled).toBeFalsy();
                expect(kvaSizeLarge.Disabled).toBeTruthy();

                //Colour
                var kvaColour = <any>_.find(keyVariantAttributeItems, { 'PropertyName': 'Colour' });
                var kvaColourSunburst     = <any>_.find(kvaColour.Values, { 'Value': 'sunburst' });
                var kvaColourBlueRadiance = <any>_.find(kvaColour.Values, { 'Value': 'blue_radiance' });
                var kvaColourShellCoral   = <any>_.find(kvaColour.Values, { 'Value': 'shell_coral' });

                expect(kvaColourSunburst.Selected).toBeFalsy();
                expect(kvaColourBlueRadiance.Selected).toBeTruthy();
                expect(kvaColourShellCoral.Selected).toBeFalsy();

                expect(kvaColourSunburst.Disabled).toBeTruthy();
                expect(kvaColourBlueRadiance.Disabled).toBeFalsy();
                expect(kvaColourShellCoral.Disabled).toBeFalsy();
            });

            it('SHOULD return the ViewModel with the selected variant out of stock (large,blue_radiance)', () => {
                //Arrange
                var builder = new Orckestra.Composer.KeyVariantAttributeItemsBuilder(context);
                var selectedKvas = {'Size': 'large', 'Colour': 'blue_radiance'};

                //Act
                var keyVariantAttributeItems = builder.BuildKeyVariantAttributeItemsFor(selectedKvas);

                //Assert
                //Size
                var kvaSize = <any>_.find(keyVariantAttributeItems, { 'PropertyName': 'Size' });
                var kvaSizeSmall  = <any>_.find(kvaSize.Values, { 'Value': 'small' });
                var kvaSizeMedium = <any>_.find(kvaSize.Values, { 'Value': 'medium' });
                var kvaSizeLarge  = <any>_.find(kvaSize.Values, { 'Value': 'large' });
                expect(kvaSizeSmall.Selected).toBeFalsy();
                expect(kvaSizeMedium.Selected).toBeFalsy();
                expect(kvaSizeLarge.Selected).toBeTruthy();

                expect(kvaSizeSmall.Disabled).toBeTruthy();
                expect(kvaSizeMedium.Disabled).toBeFalsy();
                expect(kvaSizeLarge.Disabled).toBeTruthy();

                //Colour
                var kvaColour = <any>_.find(keyVariantAttributeItems, { 'PropertyName': 'Colour' });
                var kvaColourSunburst     = <any>_.find(kvaColour.Values, { 'Value': 'sunburst' });
                var kvaColourBlueRadiance = <any>_.find(kvaColour.Values, { 'Value': 'blue_radiance' });
                var kvaColourShellCoral   = <any>_.find(kvaColour.Values, { 'Value': 'shell_coral' });

                expect(kvaColourSunburst.Selected).toBeFalsy();
                expect(kvaColourBlueRadiance.Selected).toBeTruthy();
                expect(kvaColourShellCoral.Selected).toBeFalsy();

                expect(kvaColourSunburst.Disabled).toBeFalsy();
                expect(kvaColourBlueRadiance.Disabled).toBeTruthy();
                expect(kvaColourShellCoral.Disabled).toBeFalsy();
            });
        });

        describe('WITH the context.viewModel of a Product with many KVAs', () => {
            var context = <Orckestra.Composer.IControllerContext>{
                viewModel: {
                    'allVariants': [
                        { 'Id': 'SomeVariantA1t1f', 'Kvas': {
                            'KvaNumeric': 1,
                            'KVAText': 'A',
                            'KVABool': true,
                            'KVADecimal': 1.10000,
                            'KVALookup': 'first'
                        } },
                        { 'Id': 'SomeVariantA2t1f', 'Kvas': {
                            'KvaNumeric': 2,
                            'KVAText': 'A',
                             'KVABool': true,
                             'KVADecimal': 1.10000,
                             'KVALookup': 'first'
                        } },
                        { 'Id': 'SomeVariantA3t1f', 'Kvas': {
                            'KvaNumeric': 3,
                            'KVAText': 'A',
                            'KVABool': true,
                            'KVADecimal': 1.10000,
                            'KVALookup': 'first'
                        } },
                        { 'Id': 'SomeVariantB3t1f', 'Kvas': {
                            'KvaNumeric': 3,
                            'KVAText': 'B',
                            'KVABool': true,
                            'KVADecimal': 1.10000,
                            'KVALookup': 'first'
                        } },
                        { 'Id': 'SomeVariantB3f1f', 'Kvas': {
                            'KvaNumeric': 3,
                            'KVAText': 'B',
                            'KVABool': false,
                            'KVADecimal': 1.10000,
                            'KVALookup': 'first'
                        } },
                        { 'Id': 'SomeVariantB3f8f', 'Kvas': {
                            'KvaNumeric': 3,
                            'KVAText': 'B',
                            'KVABool': false,
                            'KVADecimal': 8.30000,
                            'KVALookup': 'first'
                        } },
                        { 'Id': 'SomeVariantB3f11f', 'Kvas': {
                            'KvaNumeric': 3,
                            'KVAText': 'B',
                            'KVABool': false,
                            'KVADecimal': 11.90000,
                            'KVALookup': 'first'
                             } },
                        { 'Id': 'SomeVariantB3f11m', 'Kvas': {
                            'KvaNumeric': 3,
                            'KVAText': 'B',
                            'KVABool': false,
                            'KVADecimal': 11.90000,
                            'KVALookup': 'middle'
                             } },
                        { 'Id': 'SomeVariantB3f11l', 'Kvas': {
                            'KvaNumeric': 3,
                            'KVAText': 'B',
                            'KVABool': false,
                            'KVADecimal': 11.90000,
                            'KVALookup': 'last'
                        } },
                     ],
                    'keyVariantAttributeItems': [
                      { 'PropertyName': 'KvaNumeric', 'PropertyDataType': 'Number', 'Values': [
                          { 'Value': 1 },
                          { 'Value': 2 },
                          { 'Value': 3 }
                      ]},
                      { 'PropertyName': 'KVAText', 'PropertyDataType': 'Text', 'Values': [
                          { 'Value': 'A' },
                          { 'Value': 'B' },
                          { 'Value': 'C' }
                      ]},
                      { 'PropertyName': 'KVABool', 'PropertyDataType': 'Boolean', 'Values': [
                          { 'Value': false },
                          { 'Value': true  }
                      ]},
                      { 'PropertyName': 'KVADecimal', 'PropertyDataType': 'Decimal', 'Values': [
                          { 'Value': 1.10000  },
                          { 'Value': 8.30000  },
                          { 'Value': 11.90000 }
                      ]},
                      { 'PropertyName': 'KVALookup', 'PropertyDataType': 'Lookup', 'Values': [
                          { 'Value': 'first'  },
                          { 'Value': 'middle' },
                          { 'Value': 'last'   }
                      ]}
                    ]
                },
                templateName: null,
                dataItemId: null,
                container: null,
                window: null
            };

            it('SHOULD return the ViewModel with the selected variant', () => {
                //Arrange
                var builder = new Orckestra.Composer.KeyVariantAttributeItemsBuilder(context);
                var selectedKvas = {
                    'KvaNumeric': 3,
                    'KVAText': 'A',
                    'KVABool': true,
                    'KVADecimal': 1.10000,
                    'KVALookup': 'first'
                };

                //Act
                var keyVariantAttributeItems = builder.BuildKeyVariantAttributeItemsFor(selectedKvas);

                //Assert
                //Numeric
                var KvaNumeric = <any>_.find(keyVariantAttributeItems, { 'PropertyName': 'KvaNumeric' });
                expect((<any>_.find(KvaNumeric.Values, { 'Value': 1 })).Selected).toBeFalsy();
                expect((<any>_.find(KvaNumeric.Values, { 'Value': 2 })).Selected).toBeFalsy();
                expect((<any>_.find(KvaNumeric.Values, { 'Value': 3 })).Selected).toBeTruthy();

                expect((<any>_.find(KvaNumeric.Values, { 'Value': 1 })).Disabled).toBeFalsy();
                expect((<any>_.find(KvaNumeric.Values, { 'Value': 2 })).Disabled).toBeFalsy();
                expect((<any>_.find(KvaNumeric.Values, { 'Value': 3 })).Disabled).toBeFalsy();

                //Text
                var KvaText = <any>_.find(keyVariantAttributeItems, { 'PropertyName': 'KVAText' });
                expect((<any>_.find(KvaText.Values, { 'Value': 'A' })).Selected).toBeTruthy();
                expect((<any>_.find(KvaText.Values, { 'Value': 'B' })).Selected).toBeFalsy();
                expect((<any>_.find(KvaText.Values, { 'Value': 'C' })).Selected).toBeFalsy();

                expect((<any>_.find(KvaText.Values, { 'Value': 'A' })).Disabled).toBeFalsy();
                expect((<any>_.find(KvaText.Values, { 'Value': 'B' })).Disabled).toBeFalsy();
                expect((<any>_.find(KvaText.Values, { 'Value': 'C' })).Disabled).toBeTruthy();

                //Bool
                var KvaBool = <any>_.find(keyVariantAttributeItems, { 'PropertyName': 'KVABool' });
                expect((<any>_.find(KvaBool.Values, { 'Value': true  })).Selected).toBeTruthy();
                expect((<any>_.find(KvaBool.Values, { 'Value': false })).Selected).toBeFalsy();

                expect((<any>_.find(KvaBool.Values, { 'Value': true  })).Disabled).toBeFalsy();
                expect((<any>_.find(KvaBool.Values, { 'Value': false })).Disabled).toBeTruthy();

                //Decimal
                var KvaDecimal = <any>_.find(keyVariantAttributeItems, { 'PropertyName': 'KVADecimal' });
                expect((<any>_.find(KvaDecimal.Values, { 'Value': 1.10000  })).Selected).toBeTruthy();
                expect((<any>_.find(KvaDecimal.Values, { 'Value': 8.30000  })).Selected).toBeFalsy();
                expect((<any>_.find(KvaDecimal.Values, { 'Value': 11.90000 })).Selected).toBeFalsy();

                expect((<any>_.find(KvaDecimal.Values, { 'Value': 1.10000  })).Disabled).toBeFalsy();
                expect((<any>_.find(KvaDecimal.Values, { 'Value': 8.30000  })).Disabled).toBeTruthy();
                expect((<any>_.find(KvaDecimal.Values, { 'Value': 11.90000 })).Disabled).toBeTruthy();

                //Lookup
                var KvaLookup = <any>_.find(keyVariantAttributeItems, { 'PropertyName': 'KVALookup' });
                expect((<any>_.find(KvaLookup.Values, { 'Value': 'first'  })).Selected).toBeTruthy();
                expect((<any>_.find(KvaLookup.Values, { 'Value': 'middle' })).Selected).toBeFalsy();
                expect((<any>_.find(KvaLookup.Values, { 'Value': 'last'   })).Selected).toBeFalsy();

                expect((<any>_.find(KvaLookup.Values, { 'Value': 'first'  })).Disabled).toBeFalsy();
                expect((<any>_.find(KvaLookup.Values, { 'Value': 'middle' })).Disabled).toBeTruthy();
                expect((<any>_.find(KvaLookup.Values, { 'Value': 'last'   })).Disabled).toBeTruthy();
            });

            it('SHOULD return the ViewModel with the selected variant', () => {
                //Arrange
                var builder = new Orckestra.Composer.KeyVariantAttributeItemsBuilder(context);
                var selectedKvas = {
                    'KvaNumeric': 3,
                    'KVAText': 'B',
                    'KVABool': false,
                    'KVADecimal': 11.90000,
                    'KVALookup': 'first'
                };

                //Act
                var keyVariantAttributeItems = builder.BuildKeyVariantAttributeItemsFor(selectedKvas);

                //Assert
                //Numeric
                var KvaNumeric = <any>_.find(keyVariantAttributeItems, { 'PropertyName': 'KvaNumeric' });
                expect((<any>_.find(KvaNumeric.Values, { 'Value': 1 })).Selected).toBeFalsy();
                expect((<any>_.find(KvaNumeric.Values, { 'Value': 2 })).Selected).toBeFalsy();
                expect((<any>_.find(KvaNumeric.Values, { 'Value': 3 })).Selected).toBeTruthy();

                expect((<any>_.find(KvaNumeric.Values, { 'Value': 1 })).Disabled).toBeTruthy();
                expect((<any>_.find(KvaNumeric.Values, { 'Value': 2 })).Disabled).toBeTruthy();
                expect((<any>_.find(KvaNumeric.Values, { 'Value': 3 })).Disabled).toBeFalsy();

                //Text
                var KvaText = <any>_.find(keyVariantAttributeItems, { 'PropertyName': 'KVAText' });
                expect((<any>_.find(KvaText.Values, { 'Value': 'A' })).Selected).toBeFalsy();
                expect((<any>_.find(KvaText.Values, { 'Value': 'B' })).Selected).toBeTruthy();
                expect((<any>_.find(KvaText.Values, { 'Value': 'C' })).Selected).toBeFalsy();

                expect((<any>_.find(KvaText.Values, { 'Value': 'A' })).Disabled).toBeTruthy();
                expect((<any>_.find(KvaText.Values, { 'Value': 'B' })).Disabled).toBeFalsy();
                expect((<any>_.find(KvaText.Values, { 'Value': 'C' })).Disabled).toBeTruthy();

                //Bool
                var KvaBool = <any>_.find(keyVariantAttributeItems, { 'PropertyName': 'KVABool' });
                expect((<any>_.find(KvaBool.Values, { 'Value': true  })).Selected).toBeFalsy();
                expect((<any>_.find(KvaBool.Values, { 'Value': false })).Selected).toBeTruthy();

                expect((<any>_.find(KvaBool.Values, { 'Value': true  })).Disabled).toBeTruthy();
                expect((<any>_.find(KvaBool.Values, { 'Value': false })).Disabled).toBeFalsy();

                //Decimal
                var KvaDecimal = <any>_.find(keyVariantAttributeItems, { 'PropertyName': 'KVADecimal' });
                expect((<any>_.find(KvaDecimal.Values, { 'Value': 1.10000  })).Selected).toBeFalsy();
                expect((<any>_.find(KvaDecimal.Values, { 'Value': 8.30000  })).Selected).toBeFalsy();
                expect((<any>_.find(KvaDecimal.Values, { 'Value': 11.90000 })).Selected).toBeTruthy();

                expect((<any>_.find(KvaDecimal.Values, { 'Value': 1.10000  })).Disabled).toBeFalsy();
                expect((<any>_.find(KvaDecimal.Values, { 'Value': 8.30000  })).Disabled).toBeFalsy();
                expect((<any>_.find(KvaDecimal.Values, { 'Value': 11.90000 })).Disabled).toBeFalsy();

                //Lookup
                var KvaLookup = <any>_.find(keyVariantAttributeItems, { 'PropertyName': 'KVALookup' });
                expect((<any>_.find(KvaLookup.Values, { 'Value': 'first'  })).Selected).toBeTruthy();
                expect((<any>_.find(KvaLookup.Values, { 'Value': 'middle' })).Selected).toBeFalsy();
                expect((<any>_.find(KvaLookup.Values, { 'Value': 'last'   })).Selected).toBeFalsy();

                expect((<any>_.find(KvaLookup.Values, { 'Value': 'first'  })).Disabled).toBeFalsy();
                expect((<any>_.find(KvaLookup.Values, { 'Value': 'middle' })).Disabled).toBeFalsy();
                expect((<any>_.find(KvaLookup.Values, { 'Value': 'last'   })).Disabled).toBeFalsy();
            });
        });
    });
})();
