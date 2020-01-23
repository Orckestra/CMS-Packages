/// <reference path="../../Typings/tsd.d.ts" />
/// <reference path='../Generics/Collections/IHashTable.ts' />
/// <reference path='../Events/IEventHub.ts' />
/// <reference path='./IController.ts' />
/// <reference path='./IControllerContext.ts' />
/// <reference path='./IRegisterActionOptions.ts' />
/// <reference path='./IControllerActionContext.ts' />
///<reference path='../Templating/IComposerTemplates.ts' />
///<reference path='../Templating/IComposerTemplates.ts' />
/// <reference path='../JQueryPlugins/IParsleyJqueryPlugin.ts' />
/// <reference path='../Validation/IParsley.ts' />
/// <reference path='../UI/UIBusyParam.ts' />
/// <reference path='../UI/UIBusyHandle.ts' />
/// <reference path='../IComposerContext.ts' />
/// <reference path='../IComposerConfiguration.ts' />

module Orckestra.Composer {
    'use strict';

    /**
    * Provides methods that respond to client-side requests.
    */
    export class Controller implements IController {
        private _composerEventPostfix = '.composer';
        private _defaultEventsToMonitor: string[] =
        ['click', 'mouseover', 'mouseout', 'contextmenu', 'submit', 'focus', 'blur', 'change']; // 'dblclick'
        private _unregister: {
            (): void;
        };

        // Classes that extend Controller should really laser focus and choose which events to Monitor.
        // The default events are here for magic mode.
        protected eventsToMonitor: string[];

        constructor(protected context: IControllerContext,
            protected eventHub: IEventHub,
            protected composerContext: IComposerContext,
            protected composerConfiguration: IComposerConfiguration
        ) {

            if (_.isEmpty(context)) {
                throw new Error('context is required');
            }

            if (_.isEmpty(eventHub)) {
                throw new Error('eventHub is required');
            }

            if (_.isEmpty(composerContext)) {
                throw new Error('composerContext is required');
            }
        }

        public static registerAction(classToRegisterActionOn: any, registerActionOptions: IRegisterActionOptions) {
            var classPrototype = classToRegisterActionOn.prototype;

            if (!Controller.prototype.isPrototypeOf(classPrototype)) {
                throw new Error(
                    'The class you are trying to register the action on is not a controller.'
                );
            }

            if (_.isFunction(registerActionOptions.actionDelegate)) {
                if (!classPrototype.hasOwnProperty(registerActionOptions.actionName) ||
                    registerActionOptions.overwrite &&
                    classPrototype.hasOwnProperty(registerActionOptions.actionName) &&
                    _.isFunction(classPrototype[registerActionOptions.actionName])) {

                    classPrototype[registerActionOptions.actionName] = registerActionOptions.actionDelegate;
                } else {
                    throw new Error(`You cannot overwrite the action named "${registerActionOptions.actionName}" \
without specifying overwrite = true in the registerActionOptions.`);
                }
            } else {
                throw new Error(`Unable to register action ${registerActionOptions.actionName}. The action delegate is not a function.`);
            }
        }

        public initialize() {
            if (_.isEmpty(this.eventsToMonitor)) {
                this.eventsToMonitor = this._defaultEventsToMonitor;
            }

            this.registerDomEvents();
        }

        public dispose() {
            this.unregisterDomEvents();
        }

        public asyncBusy(options: UIBusyParam = {}): UIBusyHandle {
            options = _.merge(<UIBusyParam>{
                elementContext: this.context.container,
                containerContext: this.context.container,
                loadingIndicatorSelector: '.loading-indicator',
                msDelay: 0
            }, options);

            var loadingIndicatorContext = options.elementContext.find(options.loadingIndicatorSelector);
            var handle = new UIBusyHandle(loadingIndicatorContext, options.containerContext, options.msDelay);
            return handle;
        }

        /*
         * Prevents a form from submitting.
         * @param context A controller action context.
         */
        public preventFormSubmit(context: IControllerActionContext) {
            context.event.preventDefault();
        }

        protected render(templateId: string, viewModel: any, parentSelector?: string) {
            var container = this.context.container;

            if (!_.isEmpty(parentSelector)) {
                container = this.context.container.find(parentSelector);
            }

            var elements = container.find(`[data-templateid="${templateId}"]`);

            if (_.isEmpty(elements)) {
                console.warn(`Could not find the template '${templateId}' inside its container.`, container);
            }

            elements.each((index: Number, item: HTMLElement) => {
                var renderedTemplate = this.getRenderedTemplateContents(templateId, viewModel);

                if (renderedTemplate !== null) {
                    item.outerHTML = renderedTemplate;
                }
            });
        }

        protected getRenderedTemplateContents(templateId: string, viewModel: any): string {
            var template = (<any>Orckestra.Composer).Templates[templateId];

            if (!_.isFunction(template)) {
                console.error(`Template '${templateId}' not found in compiled templates.`);
                return null;
            }

            try {
                return template(viewModel);
            } catch (error) {
                // catch handlebars rendering errors mostly
                console.error(`${error.name}: ${error.message} in template '${templateId}'.`, viewModel);
            }
        }

        protected registerFormsForValidation(context: JQuery, customOptions: any = {}): Orckestra.Composer.IParsley[] {
            var formValidators: Orckestra.Composer.IParsley[] = [];

            var options = {
                trigger: 'focusout change',
                focus: 'first',
                errorTemplate: '<li></li>',
                classHandler: function(fieldInstance) {
                    var handleSelector = fieldInstance.$element.data('parsleyClassHandlerSelector');

                    // returning undefined will make parsley use the default classHandler
                    if (_.isEmpty(handleSelector)) {
                        return undefined;
                    }
                    var classHandler = fieldInstance.$element.closest(handleSelector);
                    return classHandler;
                }
            };

            _.assign(options, customOptions);

            context.each((index, element) => {
                formValidators.push((<Orckestra.Composer.IParsley>(<Orckestra.Composer.IParsleyJqueryPlugin>$(element)).parsley(options)));
            });

            if (customOptions.serverValidationContainer) {
                this.hideServerValidationMessageOnClientValidation(formValidators, customOptions.serverValidationContainer);
            }

            return formValidators;
        }

        /**
         * Hide any messages from previous server validation
         * when the new form is used to prevent mixing up
         * client side validation message with server side
         * messages.
         *
         * @param formValidators all the parlsey forms to manage
         * @param serverValidationContainer the jQuery selector to find messages to empty
         */
        private hideServerValidationMessageOnClientValidation(formValidators: Orckestra.Composer.IParsley[], serverValidationContainer: any) {
            _.each(formValidators, function(parsley: any) {
                parsley.subscribe('parsley:field:validate', function() {
                    parsley.$element.find(serverValidationContainer).empty();
                    _.defer(function() { parsley.unsubscribe('parsley:field:validate'); });
                });
            });
        }

        private registerDomEvents() {
            var parseAction = this.parseAction.bind(this);

            this._unregister = () => {
                this.context.container.off(this._composerEventPostfix, parseAction);
            };

            this.eventsToMonitor.forEach((item: string) => {
                this.context.container.on(`${item}${this._composerEventPostfix}`, parseAction);
            });
        }

        private unregisterDomEvents() {
            if (this._unregister !== void 0) {
                this._unregister();
            }
        }

        private parseAction(e: JQueryEventObject) {
            this.applyControllerAction($(e.target), e);
        }

        private applyControllerAction(context: JQuery, e: JQueryEventObject) {
            var controllerActions: string[],
                eventAttribute: string = `oc-${e.type}`,
                rawActions: string = context.data(eventAttribute);

            if (_.isEmpty(rawActions)) {
                if (context.length > 0 && context[0] !== this.context.container[0]) {
                    this.applyControllerAction(context.parent(), e);
                }

                return;
            }

            controllerActions = rawActions.replace(/\s+/g, '').split(',');
            this.applyControllerActions(context, e, controllerActions);
        }

        private applyControllerActions(context: JQuery, e: JQueryEventObject, controllerActions: string[]) {
            controllerActions.forEach((controllerAction) => {
                var controllerActionContext: Orckestra.Composer.IControllerActionContext;

                if (_.isFunction(this[controllerAction])) {

                    controllerActionContext = {
                        elementContext: context,
                        event: e
                    };

                    this[controllerAction].apply(this, [controllerActionContext]);
                }
            });
        }
    }
}
