/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='./IPlugin.ts' />

module Orckestra.Composer {
    export class StickyAffixPlugin implements IPlugin {
        public initialize(window: Window, document: HTMLDocument) {
            $('[data-sticky-top]').each(function() {
                var stickyOffset = $(this).data('sticky-top-offset');
                stickyOffset = stickyOffset ? stickyOffset : 0;

                $(this).affix({
                    offset: {
                        top: function(element) {
                            return $(element).parent().offset().top - stickyOffset;
                        }
                    }
                });
            });
        }
    }
}
