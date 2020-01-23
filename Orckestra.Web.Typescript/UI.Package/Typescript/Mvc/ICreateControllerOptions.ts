///<reference path='../../Typings/tsd.d.ts' />
///<reference path='./IControllerContext.ts' />
///<reference path='../Events/IEventHub.ts' />
///<reference path='../IComposerContext.ts' />
///<reference path='../IComposerConfiguration.ts' />

module Orckestra.Composer {
    export interface ICreateControllerOptions {
        controllerName: string;
        context: Orckestra.Composer.IControllerContext;
        eventHub: Orckestra.Composer.IEventHub;
        composerContext: IComposerContext;
        composerConfiguration: IComposerConfiguration;
    }
}
