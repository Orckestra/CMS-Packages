module Orckestra.Composer {

    'use strict';

    export var urlHelper = {
       getURLParameter: function(url, name) : string {
        return decodeURIComponent((new RegExp('[?|&]' + name
            + '=' + '([^&;]+?)(&|#|;|$)').exec(url) || [, ''])[1].replace(/\+/g, '%20')) || null;
        }
    };
}
