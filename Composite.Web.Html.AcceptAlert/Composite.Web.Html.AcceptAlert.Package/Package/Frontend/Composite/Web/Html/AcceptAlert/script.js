(function ($) {
    var acceptAlertCookieName = 'Composite.Web.Html.AcceptAlert';

    $(document).ready(function () {

        $(".accept-alert").each(function () {
            var acceptAlert = $(this).appendTo($("body"));
            var cookiesName = acceptAlertCookieName + acceptAlert.data('cookieskey');
            var acceptAlertCookies = getCookies(cookiesName);

            if (acceptAlertCookies == "OK") {
                acceptAlert.remove();
                return;
            }

            var okBtn = acceptAlert.find("button");
            okBtn.on("click", function () {
                setCookies(cookiesName, "OK");
                acceptAlert.animate({ height: '0' }, 500);
            });

            setTimeout(function () { acceptAlert.animate({ height: acceptAlert.find(".container").innerHeight() }, 1000); }, 1000);
        });

        $(window).on("resize", function () {
            $(".accept-alert").each(function () {
                var acceptAlert = $(this);
                var cookiesName = acceptAlertCookieName + acceptAlert.data('cookieskey');
                var acceptAlertCookie = getCookies(cookiesName);
                if (acceptAlertCookie != "OK") {
                    acceptAlert.css("height", acceptAlert.find(".container").innerHeight());
                }
            });
        });

        function getCookies(cookiesName) {
            if (typeof (Storage) !== "undefined") {
                return localStorage.getItem(cookiesName);
            }
            return "false";
        }

        function setCookies(cookiesName, value) {
            if (typeof (Storage) !== "undefined") {
                localStorage.setItem(cookiesName, value);
            }
        }
    });
})(jQuery)