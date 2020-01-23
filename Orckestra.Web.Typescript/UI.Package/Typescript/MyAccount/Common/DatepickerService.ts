///<reference path='../../../Typings/tsd.d.ts' />

module Orckestra.Composer {
    'use strict';

    ///Docs : https://bootstrap-datepicker.readthedocs.io/en/latest/
    export class DatepickerService {

        /**
        * Display the datepicker with specified options
        * @param elementId Html element Id of the datepicker input.
        * @param date The earliest date that may be selected; all earlier dates will be disabled. Optional.
        * @param language Language of the datepicker labels. Optional. default data-datepicker-language attribute
        */
        public static renderDatepicker(elementId : string, minDate?: Date, language?: string) {

            if (minDate === undefined) {
                minDate = new Date();
                minDate.setDate(minDate.getDate() + 1);
            }

            //By default, only french(fr) and english(en-CA) are supported.
            //To add more locales: https://github.com/uxsolutions/bootstrap-datepicker/tree/master/js/locales
            if (language === undefined) {
                language = document.getElementsByTagName('html')[0].getAttribute('data-datepicker-language');
            }

            var options = { year: 'numeric', month: '2-digit', day: '2-digit', timezone: 'UTC' };

            $(elementId).datepicker({
                format: 'yyyy/mm/dd',
                // format: {
                //     toDisplay: function (date, format, language) {
                //         var d = new Date(date);
                //         var utc = new Date(d.getUTCFullYear(), d.getUTCMonth(), d.getUTCDate());
                //         return utc.toLocaleDateString(culture, options);
                //     },
                //     toValue: function (date, format, language) {
                //         var d = new Date(date);
                //         //return new Date(d.toLocaleString(culture, options));
                //         return d;
                //     }
                // },
                startDate: minDate,
                language: language,
                todayBtn: true,
                todayHighlight: true
            });
        }
    }
}