/// <reference path='../../Typings/tsd.d.ts' />

module Orckestra.Composer {

    'use strict';

    export class Utils {
         /**
         * Scroll the screen to the specified element
         * @param element The element to scroll to
         * @param offsetDiff the different applied to the top offset position of the element
         */
        public static scrollToElement(element: JQuery, offsetDiff: number = 100) {
            if (!_.isUndefined(element) && element.length > 0) {
                $('html, body').animate({
                    scrollTop: $(element).offset().top - offsetDiff
                }, 10);
            }
        }

        /**
         * Get current website id
         */
        public static getWebsiteId () {
            return $('html').data('website');
        }
    }
}
