///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../System/IDisposable.ts' />

module Orckestra.Composer {
    export interface IController extends IDisposable {
        /*
         * Initializes a view controller.
         */
        initialize(): void;

        /*
         * Disposes any resources used by a view controller.
         */
        dispose(): void;
    }
}
