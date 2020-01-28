///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../../Typescript/Storage/StorageType.ts' />
///<reference path='../../Typescript/Storage/StorageFactory.ts' />
///<reference path='../../Typescript/Storage/StoragePolyFill.ts' />

describe('WHEN trying to create client-side storage that is not supported', () => {
    it('SHOULD throw', () => {
        // Arrange
        // Act
        // Assert
        expect(() => {
            Orckestra.Composer.StorageFactory.create(<any>'nope', <any>{ name: '' });
        }).toThrow();
    });
});

describe('WHEN trying to create client-side storage that is supported', () => {
    it('SHOULD not throw', () => {
        // Arrange
        // Act
        // Assert
        expect(() => {
            Orckestra.Composer.StorageFactory.create(Orckestra.Composer.StorageType.localStorage, <any>{ name: '' });
        }).not.toThrow();
    });
});

describe('WHEN localStorage is not supported', () => {
    it('SHOULD return the localStorage polyfill', () => {
        // Arrange
        var expected: Storage = undefined,
            actual: Storage = null;

        spyOn(Orckestra.Composer.StoragePolyfill, 'create');

        // Act
        actual = Orckestra.Composer.StorageFactory.create(Orckestra.Composer.StorageType.localStorage, <any>{ name: '' });

        // Assert
        expect(Orckestra.Composer.StoragePolyfill.create).toHaveBeenCalled();
    });
});

describe('WHEN localStorage is supported but disabled', () => {
    it('SHOULD return the localStorage polyfill', () => {
        // Arrange
        var windowMock: any = {
            name: '',
            localStorage: {
                setItem: function(key: string, data: string) {
                    throw { name: 'SomethingHorribleException', message: 'Uh oh' };
                }
            }
        },
            expected: Storage = undefined,
            actual: Storage = null;
            spyOn(Orckestra.Composer.StoragePolyfill, 'create');

        // Act
        actual = Orckestra.Composer.StorageFactory.create(Orckestra.Composer.StorageType.localStorage, windowMock);

        // Assert
        expect(Orckestra.Composer.StoragePolyfill.create).toHaveBeenCalled();
    });
});

describe('WHEN localStorage is supported and not disabled', () => {
    it('SHOULD return localStorage', () => {
        // Arrange
        var windowMock: any = {
            localStorage: {
                setItem: function(key: string, data: string) {
                    console.log('set item');
                },

                removeItem: function(key: string) {
                    console.log('remove item');
                }
            }
        },
            expected = windowMock.localStorage,
            actual: Storage = null;

        // Act
        actual = Orckestra.Composer.StorageFactory.create(Orckestra.Composer.StorageType.localStorage, windowMock);

        // Assert
        expect(expected).toEqual(actual);
    });
});

describe('WHEN sessionStorage is supported and not disabled', () => {
    it('SHOULD return sessionStorage', () => {
        // Arrange
        var windowMock: any = {
            sessionStorage: {
                setItem: function(key: string, data: string) {
                    console.log('set item');
                },

                removeItem: function(key: string) {
                    console.log('remove item');
                }
            }
        },
            expected = windowMock.sessionStorage,
            actual: Storage = null;

        // Act
        actual = Orckestra.Composer.StorageFactory.create(Orckestra.Composer.StorageType.sessionStorage, windowMock);

        // Assert
        expect(expected).toEqual(actual);
    });
});

describe('WHEN sessionStorage is not supported', () => {
    it('SHOULD return the sessionStorage polyfill', () => {
        // Arrange
        var actual: Storage = null,
        window: any = { name: ''};

        spyOn(Orckestra.Composer.StoragePolyfill, 'create');
        // Act
        actual = Orckestra.Composer.StorageFactory.create(Orckestra.Composer.StorageType.sessionStorage, window);

        // Assert
        expect(Orckestra.Composer.StoragePolyfill.create).toHaveBeenCalledWith(window, Orckestra.Composer.StorageType.sessionStorage);
    });
});


describe('WHEN sessionStorage is supported but disabled', () => {
    it('SHOULD return the sessionStorage polyfill', () => {
        // Arrange
        var windowMock: any = {
            name: '',
            sessionStorage: {
                setItem: function(key: string, data: string) {
                    throw { name: 'SomethingHorribleException', message: 'Uh oh' };
                }
            }
        },
            actual: Storage = null;
            spyOn(Orckestra.Composer.StoragePolyfill, 'create');

        // Act
        actual = Orckestra.Composer.StorageFactory.create(Orckestra.Composer.StorageType.sessionStorage, windowMock);

        // Assert
        expect(Orckestra.Composer.StoragePolyfill.create).toHaveBeenCalledWith(windowMock, Orckestra.Composer.StorageType.sessionStorage);
    });
});