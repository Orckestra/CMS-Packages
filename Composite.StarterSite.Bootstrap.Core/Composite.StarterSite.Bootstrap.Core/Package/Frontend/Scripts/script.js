(function () {
    // Fix responsive issue for IE 10 on Windows Phone 8
    // Issue: https://github.com/twbs/bootstrap/issues/10497
    if ("-ms-user-select" in document.documentElement.style && navigator.userAgent.match(/IEMobile\/10\.0/)) {
        var msViewportStyle = document.createElement("style");
        msViewportStyle.appendChild(
            document.createTextNode("@-ms-viewport{width:auto!important}")
        );
        document.getElementsByTagName("head")[0].appendChild(msViewportStyle);
    }
    // end fix
})();







