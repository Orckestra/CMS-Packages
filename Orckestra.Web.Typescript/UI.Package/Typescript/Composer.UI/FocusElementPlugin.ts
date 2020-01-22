/// <reference path='./IPlugin.ts' />

module Orckestra.Composer {
    export class FocusElementPlugin implements IPlugin {
        public initialize(window: Window, document: HTMLDocument) {
            /**
             * On click, scroll to field and focus in it.
             */
            $('body', document).on('click', '[data-focus-element]', function(e) {
                var target = $(this).data('focus-element');
                $('body, html').scrollTop($(target).offset().top - 20);
                $(target).focus();

                e.preventDefault();
            });
        }
    }
}
