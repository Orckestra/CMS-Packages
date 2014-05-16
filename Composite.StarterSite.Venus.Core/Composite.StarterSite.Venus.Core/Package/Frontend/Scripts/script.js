
(function (window) {
    /* fix bootstrap responsive issue for IE 10 on Windows Phone 8: */
    if ("-ms-user-select" in document.documentElement.style && navigator.userAgent.match(/IEMobile\/10\.0/)) { 
        var msViewportStyle = document.createElement("style"); 
        msViewportStyle.appendChild( 
            document.createTextNode("@-ms-viewport{width:auto!important}") 
        ); 
        document.getElementsByTagName("head")[0].appendChild(msViewportStyle); 
    }
    /* end fix */

    $(document).ready(function () {

        $(".mega-menu").on("show.bs.dropdown", function () {
            if (!$(".navbar-collapse").hasClass("in")) {
                showOverlay();
            }
        })

        $(".mega-menu").on("hide.bs.dropdown", function () {
            if (!$(".navbar-collapse").hasClass("in")) {
                hideOverlay();
            }
        })
       
        $('.navbar-collapse').on('show.bs.collapse', function () {
            $(".navbar-toggle .icon-bar").addClass("hide");
            $(".navbar-toggle .icon-close").removeClass("hide");
            showOverlay();
        });
        $('.navbar-collapse').on('hide.bs.collapse', function () {
            $(".navbar-toggle .icon-bar").removeClass("hide");
            $(".navbar-toggle .icon-close").addClass("hide");
            hideOverlay();
        });

        $(".dropdown-toggle").dblclick(function (e) {
             window.location.href = $(this).attr("href");
        });

        // Mobile navbar links 
        var maxLinksInRow = 5;
        var linksCount = $(".mobile-navbar-link").length > 5 ? 5 : $(".mobile-navbar-link").length;
        var linksStyle = "width-" + Math.floor(100/linksCount);
        $(".mobile-navbar-link").addClass(linksStyle);

        //Profiles
        $(".profiles-list .row").each(function () {
            $(".thumbnail", $(this)).equalHeightColumns({minWidth: 767, extraHeight: 18});
        });
    });

    function showOverlay() {
        $("body").addClass("overlay");
    }

    function hideOverlay() {
        $("body").removeClass("overlay");
    }

})(window);

/* equalHeightColumns.js 1.1 */
(function ($) {
    $.fn.equalHeightColumns = function (options) {
        defaults = { minWidth: -1, maxWidth: 99999, setHeightOn: "min-height", defaultVal: 0, extraHeight: 0 }; var $this = $(this); options = $.extend({}, defaults, options); var resizeHeight = function () {
            var windowWidth = $(window).width(); if (options.minWidth < windowWidth && options.maxWidth > windowWidth) { var height = 0; var highest = 0; $this.css(options.setHeightOn, options.defaultVal); $this.each(function () { height = $(this).height(); if (height > highest) highest = height }); $this.css(options.setHeightOn, highest + options.extraHeight) } else $this.css(options.setHeightOn,
            options.defaultVal)
        }; resizeHeight(); $(window).resize(resizeHeight); $this.find("img").load(resizeHeight); if (typeof options.afterLoading !== "undefined") $this.find(options.afterLoading).load(resizeHeight); if (typeof options.afterTimeout !== "undefined") setTimeout(function () { resizeHeight(); if (typeof options.afterLoading !== "undefined") $this.find(options.afterLoading).load(resizeHeight) }, options.afterTimeout)
    }
})(jQuery);














