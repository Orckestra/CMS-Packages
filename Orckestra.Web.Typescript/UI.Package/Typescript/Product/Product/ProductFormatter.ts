///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/ComposerClient.ts' />
///<reference path='../../Events/EventHub.ts' />

module Orckestra.Composer {
    'use strict';

    /**
      * Module helper to format string value using the Overture convertion rules.
      */
    export class ProductFormatter {

        /**
         * convert a ProductProperty string value into the right strongly typed variable.
         *
         *     formatter.convertToStronglyTyped(actionContext.elementContext.val(), 'Decimal');
         *
         * @param strValue         The ProductProperty value to convert
         * @param propertyDataType the ProductProperty.DataType to induce the type
         */
        public convertToStronglyTyped(strValue: string, propertyDataType: string): any {
            var value: any;

            if (propertyDataType === 'Decimal') {
                value = parseFloat(strValue);
            } else if (propertyDataType === 'Number') {
                value = parseInt(strValue, 10);
            } else if (propertyDataType === 'Boolean') {
                value = (strValue === 'true');
            } else if (propertyDataType === 'Text') {
                value = strValue + '';
            } else if (propertyDataType === 'Lookup') {
                value = strValue + '';
            } else {
                value = strValue;
            }

            return value;
        }
    }
}
