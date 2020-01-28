/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../../Typescript/Composer.Product/Product/ProductFormatter.ts' />
/// <reference path='../../Mocks/MockControllerContext.ts' />
/// <reference path='../../Mocks/MockJqueryEventObject.ts' />
/// <reference path='../../../Typescript/Events/EventHub.ts' />
/// <reference path='../../../Typescript/Events/IEventHub.ts' />
/// <reference path='../../../Typescript/Events/IEventInformation.ts' />
/// <reference path='../../../Typescript/Mvc/IControllerActionContext.ts' />

(() => {
    'use strict';

    var formatter: Orckestra.Composer.ProductFormatter;

    describe('WHEN calling the ProductFormatter.convertToStronglyTyped method', () => {
        beforeEach(() => {
            formatter = new Orckestra.Composer.ProductFormatter();
        });

        describe('WITH Boolean conversion params', () => {
            it('SHOULD convert the litteral \'true\' to a typeof Boolean true', () => {
                //Arrange
                var strValue = 'true';
                var propertyDataType = 'Boolean';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe(true);
                expect(typeof(value)).toBe(typeof(true));
            });

            it('SHOULD convert the litteral \'false\' to a typeof Boolean false', () => {
                //Arrange
                var strValue = 'false';
                var propertyDataType = 'Boolean';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe(false);
                expect(typeof(value)).toBe(typeof(false));
            });
        });

        describe('WITH Decimal conversion params', () => {
            it('SHOULD convert to a typeof Number with decimals', () => {
                //Arrange
                var strValue = '1234.5678';
                var propertyDataType = 'Decimal';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe(1234.5678);
                expect(typeof(value)).toBe(typeof(1.1));
            });

            it('SHOULD convert to a typeof Number with decimals', () => {
                //Arrange
                var strValue = '0.1';
                var propertyDataType = 'Decimal';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe(0.1);
                expect(typeof(value)).toBe(typeof(1.1));
            });

            it('SHOULD convert to a typeof Number with decimals', () => {
                //Arrange
                var strValue = '.167';
                var propertyDataType = 'Decimal';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe(0.167);
                expect(typeof(value)).toBe(typeof(1.1));
            });

            it('SHOULD convert to a typeof Number with decimals', () => {
                //Arrange
                var strValue = '.0';
                var propertyDataType = 'Decimal';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe(.0);
                expect(typeof(value)).toBe(typeof(1.1));
            });

            it('SHOULD convert to a typeof Number with decimals', () => {
                //Arrange
                var strValue = '1.0000';
                var propertyDataType = 'Decimal';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe(1.0);
                expect(typeof(value)).toBe(typeof(1.1));
            });
        });

        describe('WITH Number conversion params', () => {
            it('SHOULD convert to a typeof Number', () => {
                //Arrange
                var strValue = '12345';
                var propertyDataType = 'Number';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe(12345);
                expect(typeof(value)).toBe(typeof(1));
            });

            it('SHOULD convert to a typeof Number', () => {
                //Arrange
                var strValue = '0';
                var propertyDataType = 'Decimal';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe(0);
                expect(typeof(value)).toBe(typeof(1));
            });


            it('SHOULD convert to a typeof Number', () => {
                //Arrange
                var strValue = '9.0';
                var propertyDataType = 'Decimal';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe(9);
                expect(typeof(value)).toBe(typeof(1));
            });
        });

        describe('WITH Text conversion params', () => {
            it('SHOULD preserve typeof string', () => {
                //Arrange
                var strValue = 'bob';
                var propertyDataType = 'Text';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe('bob');
                expect(typeof(value)).toBe(typeof('str'));
            });

            it('SHOULD preserve typeof string on empty strings', () => {
                //Arrange
                var strValue = '';
                var propertyDataType = 'Text';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe('');
                expect(typeof(value)).toBe(typeof('str'));
            });

            it('SHOULD preserve typeof string on whitespace strings', () => {
                //Arrange
                var strValue = ' \t\r\n';
                var propertyDataType = 'Text';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe(' \t\r\n');
                expect(typeof(value)).toBe(typeof('str'));
            });

            it('SHOULD preserve typeof string on numbers', () => {
                //Arrange
                var strValue = '2';
                var propertyDataType = 'Text';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe('2');
                expect(typeof(value)).toBe(typeof('str'));
            });

            it('SHOULD preserve typeof string on decimals', () => {
                //Arrange
                var strValue = '123.0000';
                var propertyDataType = 'Text';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe('123.0000');
                expect(typeof(value)).toBe(typeof('str'));
            });

            it('SHOULD preserve typeof string on zero', () => {
                //Arrange
                var strValue = '0';
                var propertyDataType = 'Text';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe('0');
                expect(typeof(value)).toBe(typeof('str'));
            });
        });

        describe('WITH Lookup conversion params', () => {
            it('SHOULD preserve typeof string', () => {
                //Arrange
                var strValue = 'bob';
                var propertyDataType = 'Text';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe('bob');
                expect(typeof(value)).toBe(typeof('str'));
            });

            it('SHOULD preserve typeof string on empty strings', () => {
                //Arrange
                var strValue = '';
                var propertyDataType = 'Text';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe('');
                expect(typeof(value)).toBe(typeof('str'));
            });

            it('SHOULD preserve typeof string on whitespace strings', () => {
                //Arrange
                var strValue = ' \t\r\n';
                var propertyDataType = 'Text';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe(' \t\r\n');
                expect(typeof(value)).toBe(typeof('str'));
            });

            it('SHOULD preserve typeof string on numbers', () => {
                //Arrange
                var strValue = '2';
                var propertyDataType = 'Text';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe('2');
                expect(typeof(value)).toBe(typeof('str'));
            });

            it('SHOULD preserve typeof string on decimals', () => {
                //Arrange
                var strValue = '123.0000';
                var propertyDataType = 'Text';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe('123.0000');
                expect(typeof(value)).toBe(typeof('str'));
            });

            it('SHOULD preserve typeof string on zero', () => {
                //Arrange
                var strValue = '0';
                var propertyDataType = 'Text';

                //Act
                var value = formatter.convertToStronglyTyped(strValue, propertyDataType);

                //Assert
                expect(value).toBe('0');
                expect(typeof(value)).toBe(typeof('str'));
            });
        });
    });
})();
