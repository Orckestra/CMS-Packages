/// <reference path='./IPlugin.ts' />
/// <reference path='../JQueryPlugins/IPopOverJqueryPlugin.ts' />

module Orckestra.Composer {
    export class HelpBubblesPlugin implements IPlugin {
        public initialize(window: Window, document: HTMLDocument) {
            /**
             * Will calculate if there is enough space for the popover on the right
             * else will put it in the bottom.
             */
            function popoverPlacement(popover, triggeringElement) {
                var triggeringElementWidth = $(triggeringElement).outerWidth();
                var placement = 'bottom';

                // we will consider that if the trrigering element can't be doubled
                // we won't have enough space to display the popover on the right
                if ((window.innerWidth - triggeringElementWidth) > triggeringElementWidth) {
                    placement = 'right';
                }

                return placement;
            }

            //  As discussed with Sam, this is in the app.ts for now. Because we don't have a strategy
            //  yet for generic presentation/plugins javascript.
            //  Pop over initialization.
            (<Orckestra.Composer.IPopOverJqueryPlugin>$('body')).popover({
                html: true,
                placement: popoverPlacement,
                selector: '[data-toggle=popover]',
                trigger: 'focus',
                content: function() {
                    return $('#popover-content').html();
                }
            });

            /**
             * Needs the select block of a same group to have data-parent defined
             * OR that they be in the same form.
             */
            $('body').on('change', '.select-block', function(){
                var input = $(this).find('.input');
                var type = input.attr('type');
                var name = input.attr('name');

                // if checkbox check current state of prop
                if (type === 'checkbox') {
                    if (input.prop('checked')) {
                        $(this).addClass('active');
                    } else {
                        $(this).removeClass('active');
                    }
                }

                if (type === 'radio') {
                    var parentSelector = $(this).data('parent');
                    var parentElement;

                    if (parentSelector) {
                        parentElement = input.closest(parentSelector);
                    } else {
                        // if no parent specified, default to form and fallback to body
                        parentElement = input.closest('form');

                        if (parentElement.length === 0) {
                            parentElement = $('body');
                        }
                    }

                    parentElement
                        .find('.select-block:has(:radio[name="' + name + '"])')
                        .removeClass('active');

                    $(this).addClass('active');
                }
            });
        }
    }
}
