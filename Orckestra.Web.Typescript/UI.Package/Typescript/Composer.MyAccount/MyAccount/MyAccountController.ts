///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../JQueryPlugins/ISerializeObjectJqueryPlugin.ts' />

module Orckestra.Composer {

    export class MyAccountController extends Orckestra.Composer.Controller {

        public initialize() {
            super.initialize();
        }

        protected getFormData(actionContext: IControllerActionContext): any {

            return (<ISerializeObjectJqueryPlugin>actionContext.elementContext).serializeObject();
        }

        protected renderFormErrorMessages(reason: any): void {

            this.render('FormErrorMessages', reason);
            this.context.container.find('input[type="password"]').val('');
            this.registerFormsForValidation(this.context.container.find('form'), {
                serverValidationContainer: '[data-templateid="FormErrorMessages"]'
            });
        }
    }
}
