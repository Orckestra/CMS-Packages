///<reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    export interface IScheduledCallback {

        (data: any): Q.Promise<any>;
    }
}