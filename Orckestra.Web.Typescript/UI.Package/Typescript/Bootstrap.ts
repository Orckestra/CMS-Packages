///<reference path='../Typings/tsd.d.ts' />
///<reference path='./Mvc/ControllerRegistry.ts' />
///<reference path='./Mvc/ControllerFactory.ts' />
///<reference path='./Templating/IComposerTemplates.ts' />
///<reference path='./Templating/IComposerTemplates.ts' />
///<reference path='./Mvc/Localization/LocalizationProvider.ts' />
///<reference path='./Mvc/Localization/ILocalizationProvider.ts' />
///<reference path='./Validation/IParsleyValidator.ts' />
///<reference path='./ComposerContext.ts' />
///<reference path='./IComposerConfiguration.ts' />
///<reference path='./Mvc/IControllerConfiguration.ts' />
///<reference path='./Plugins/IPlugin.ts' />
///<reference path='./Events/EventHub.ts' />

module Orckestra.Composer {

    'use strict';

    function loadPlugins(plugins: string[], window: Window, document: HTMLDocument) {
        if (!_.isEmpty(plugins)) {
            plugins.forEach((rawPluginName) => {
                var pluginName = `${rawPluginName}Plugin`,
                    plugin: IPlugin;

                if (Orckestra.Composer.hasOwnProperty(pluginName) && _.isFunction(Orckestra.Composer[pluginName])) {
                    plugin = (<IPlugin>Object.create(Orckestra.Composer[pluginName]).prototype);
                    plugin.initialize(window, document);
                }
            });
        }
    }

    function loadControllers(controllerRegistry: Orckestra.Composer.ControllerRegistry, controllers: IControllerConfiguration[]) {
        if (!_.isEmpty(controllers)) {
            controllers.forEach((controllerConfiguration) => {
                controllerRegistry.register(controllerConfiguration.name, controllerConfiguration.controller);
            });
        }
    }

    function raiseLanguageSwitchEventIfNeeded(cacheProvider: ICacheProvider, eventHub: IEventHub) {
        let cacheKey = 'languageSwitchEvent';
        return cacheProvider.defaultCache.get(cacheKey).then(function (value) {
            eventHub.publish('languageSwitched', null);
            cacheProvider.defaultCache.clear(cacheKey);
        });
    }

    export var bootstrap = (window: Window, document: HTMLDocument, composerConfiguration: IComposerConfiguration) => {
        var controllerRegistry: Orckestra.Composer.ControllerRegistry = new Orckestra.Composer.ControllerRegistry(),
            controller: Orckestra.Composer.IController,
            eventHub: Orckestra.Composer.IEventHub = Orckestra.Composer.EventHub.instance(),
            cacheProvider: ICacheProvider = CacheProvider.instance(),
            localizationProvider: Orckestra.Composer.ILocalizationProvider = Orckestra.Composer.LocalizationProvider.instance(),
            composerContext: Orckestra.Composer.IComposerContext = new ComposerContext();

        // TODO: Need a better solution that <any>.
        (<any>Handlebars).partials = (<any>Orckestra.Composer).Templates;
        (<any>Handlebars).localizationProvider = localizationProvider;

        localizationProvider.initialize(composerContext).fail(function() {
            console.log('Failed to initialize the localization provider');
        }).then(() => {
            var blades = $('[data-oc-controller]'),
                controllers: IController[] = [];

            loadPlugins(composerConfiguration.plugins, window, document);
            loadControllers(controllerRegistry, composerConfiguration.controllers);

            // Need a better query selector as this one is shit
            blades.each((index: number, item: HTMLElement) => {
                var bladeName: string = item.getAttribute('data-oc-controller'),
                    context: Orckestra.Composer.IControllerContext;

                if (controllerRegistry.isRegistered(bladeName)) {
                    context = {
                        container: $(item),
                        dataItemId: item.getAttribute('data-item-id'),
                        templateName: bladeName,
                        viewModel: JSON.parse(item.getAttribute('data-context') || window[item.getAttribute('data-context-var')] || '{}'),
                        window: window
                    };

                    controller = Orckestra.Composer.ControllerFactory.createController({
                        controllerName: bladeName,
                        context: context,
                        eventHub: eventHub,
                        composerContext: composerContext,
                        composerConfiguration: composerConfiguration
                    });

                    controller.initialize();
                    controllers.push(controller);
                }
            });

            eventHub.publish('allControllersInitialized', null);

            raiseLanguageSwitchEventIfNeeded(cacheProvider, eventHub);

            /**
             * This part of the code is mostly created to clean up dom events
             * it was created to workaround a bug with IE8 that had memory leaks
             * when a circular reference was made with a dom element (ie event + update of the dom element)
             * see : http://com.hemiola.com/2009/11/23/memory-leaks-in-ie8/
             * and : http://stackoverflow.com/questions/3083196/in-internet-explorer-why-does-memory-leak-stay-even-when-navigating-away-from 
             */
            $(window).on('beforeunload', () => {
                controllers.forEach(controller => controller.dispose());
            });
        }).done();
    };
}
