///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../../Typescript/Storage/StoragePolyFill.ts' />
///<reference path='../../Typescript/Storage/StorageType.ts' />

const localStorageType = Orckestra.Composer.StorageType.localStorage;
const sessionStorageType = Orckestra.Composer.StorageType.sessionStorage;

describe('WHEN creating a localStorage polyfill', () => {
    it('SHOULD always return the same instance.', () => {
        // Arrange
        var windowMock: any = { name: '' },
            expected = Orckestra.Composer.StoragePolyfill.create(windowMock, localStorageType),
            actual: Storage = null;

        // Act
        actual = Orckestra.Composer.StoragePolyfill.create(windowMock, localStorageType);

        // Assert
        expect(expected).toEqual(actual);
    });
    it('SHOULD always NOT return the same instance if storage type is different', () => {
        // Arrange
        var windowMock: any = { name: '' },
            expected = Orckestra.Composer.StoragePolyfill.create(windowMock, localStorageType),
            actual: Storage = null;

        // Act
        actual = Orckestra.Composer.StoragePolyfill.create(windowMock, sessionStorageType);
        actual.setItem('hello', 'Lionel');

        // Assert
        expect(expected).not.toEqual(actual);
    });
});

describe('WHEN using the storage polyfill', () => {
    it('SHOULD be able to set an item in the localStorage polyfill.', () => {
        // Arrange
        var storage = Orckestra.Composer.StoragePolyfill.create(<any>{ name: '' }, localStorageType),
            expected: string = 'Lionel Richie',
            actual = null;

        // Act
        storage.setItem('hello', expected);
        actual = storage.getItem('hello');

        // Assert
        expect(expected).toEqual(actual);
    });

    it('SHOULD be able to set an item in the storage polyfill, and be key separated between different type of storage', () => {
        // Arrange
        var localStorage = Orckestra.Composer.StoragePolyfill.create(<any>{ name: '' }, localStorageType),
            localStorageExpected: string = 'Lionel Richie from local Storage',
            localStorageActual = null;
        var sessionStorage = Orckestra.Composer.StoragePolyfill.create(<any>{ name: '' }, sessionStorageType),
            sessionStorageExpected: string = 'Lionel Richie from session Storage',
            sessionStorageActual = null;

        // Act
        localStorage.setItem('hello', localStorageExpected);
        sessionStorage.setItem('hello', sessionStorageExpected);
        localStorageActual = localStorage.getItem('hello');
        sessionStorageActual = sessionStorage.getItem('hello');

        // Assert
        expect(localStorageExpected).toEqual(localStorageActual);
        expect(sessionStorageExpected).toEqual(sessionStorageActual);
    });
});

describe('WHEN using the localStorage polyfill', () => {
    it('SHOULD be able to remove an item from the localStorage polyfill.', () => {
        // Arrange
        var storage = Orckestra.Composer.StoragePolyfill.create(<any>{ name: '' }, localStorageType),
            actual = null;

        // Act
        storage.setItem('hello', 'Lionel Richie');
        storage.removeItem('hello');

        // Assert
        actual = storage.getItem('hello');
        expect(actual).toBeNull();
    });

    it('SHOULD be able to remove an item from the localStorage polyfill and not from the session one.', () => {
        // Arrange
        var localStorage = Orckestra.Composer.StoragePolyfill.create(<any>{ name: '' }, localStorageType),
            sessionStorage = Orckestra.Composer.StoragePolyfill.create(<any>{ name: '' }, sessionStorageType),
            sessionStorageExpected: string = 'Lionel Richie',
            localStorageActual = null,
            sessionStorageActual = null;

        // Act
        localStorage.setItem('hello', sessionStorageExpected);
        sessionStorage.setItem('hello', sessionStorageExpected);
        localStorage.removeItem('hello');

        // Assert
        localStorageActual = localStorage.getItem('hello');
        sessionStorageActual = sessionStorage.getItem('hello');
        expect(localStorageActual).toBeNull();
        expect(sessionStorageActual).toEqual(sessionStorageExpected);
    });
});

describe('WHEN using the localStorage polyfill', () => {
    it('SHOULD be able to get an item from the localStorage polyfill.', () => {
        // Arrange
        var storage = Orckestra.Composer.StoragePolyfill.create(<any>{ name: '' }, localStorageType),
            actual = null;

        // Act
        storage.setItem('hello', 'Lionel Richie');

        // Assert
        actual = storage.getItem('hello');
        expect(actual).toEqual('Lionel Richie');
    });
});

describe('WHEN using the storage polyfill', () => {
    it('SHOULD be able to clear all items in the localStorage polyfill.', () => {
        // Arrange
        var storage = Orckestra.Composer.StoragePolyfill.create(<any>{ name: '{}' }, localStorageType),
            actual = null;

        // Act
        storage.setItem('hello', 'Lionel');
        storage.clear();

        // Assert
        expect(Object.keys(storage).length).toEqual(0);
    });

    it('SHOULD be able to clear all items in the localStorage polyfill without clearing the one from the sessionStorage.', () => {
        // Arrange
        var localStorage = Orckestra.Composer.StoragePolyfill.create(<any>{ name: '{}' }, localStorageType),
            sessionStorage = Orckestra.Composer.StoragePolyfill.create(<any>{ name: '{}' }, sessionStorageType),
            actual = null;

        // Act
        localStorage.setItem('hello', 'Lionel');
        sessionStorage.setItem('hello', 'Lionel');
        localStorage.clear();

        // Assert
        expect(Object.keys(localStorage).length).toEqual(0);
        expect(Object.keys(sessionStorage).length).toEqual(1);
    });

    afterAll(function() {
        var localStorage = Orckestra.Composer.StoragePolyfill.create(<any>{ name: '{}' }, localStorageType),
            sessionStorage = Orckestra.Composer.StoragePolyfill.create(<any>{ name: '{}' }, sessionStorageType);
            localStorage.clear();
            sessionStorage.clear();
    });
});