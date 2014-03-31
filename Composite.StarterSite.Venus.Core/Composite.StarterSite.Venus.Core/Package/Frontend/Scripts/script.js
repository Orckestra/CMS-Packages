
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
                showOverlay();
        })

        $(".mega-menu").on("hide.bs.dropdown", function () {
                 hideOverlay();
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
    });

    function showOverlay() {
        $("body").addClass("overlay");
    }

    function hideOverlay() {
        $("body").removeClass("overlay");
    }

})(window);














