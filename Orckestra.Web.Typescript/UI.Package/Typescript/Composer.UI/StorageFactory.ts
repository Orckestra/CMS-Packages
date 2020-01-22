///<reference path='./StoragePolyFill.ts' />
///<reference path='./StorageType.ts' />

module Orckestra.Composer {

    //We need to defer the evaluation of the storage since on google chrome / incognito mode,
    //it raise a security error exception / access denied
    function getStorage(storageCallback : () => Storage, storageType : StorageType, window : Window): Storage {
        var dummyData: string = '__composer__data__bidon__';
        try {
            var storageToUse = storageCallback();
            if (storageToUse !== void 0) {
                //Hack to support safari browser / anonymous mode since it support local storage but with a max size of 0kb,
                //thus we try to insert dummy data to check if it break or not.
                storageToUse.setItem(dummyData, dummyData);
                storageToUse.removeItem(dummyData);
                return storageToUse;
            }
        } catch (e) {
            console.log('Storage is not supported or is disabled. window.name will be used instead.');
        }
            return StoragePolyfill.create(window, storageType);
    }

    export var StorageFactory = {
        create: function (storageType: StorageType, window: Window): Storage {

            switch (storageType) {
                    case StorageType.localStorage:
                        return getStorage(() => window.localStorage, storageType, window);

                    case StorageType.sessionStorage:
                        return getStorage(() => window.sessionStorage, storageType, window);

                    default:
                        throw {
                            name: 'StorageTypeException',
                            message: `The storage type "${storageType}" is currently not supported.`
                        };
                }
        }
    };
}
