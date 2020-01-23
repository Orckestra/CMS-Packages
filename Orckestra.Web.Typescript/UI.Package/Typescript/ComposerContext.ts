///<reference path='../Typings/tsd.d.ts' />
///<reference path='./IComposerContext.ts' />

module Orckestra.Composer {
    export class ComposerContext implements IComposerContext {

        public language: string = (() => {
            return document.getElementsByTagName('html')[0].getAttribute('lang');
        })();
    }
}
