(function ($) {
    var acceptAlertCookieName = 'Composite.Web.Html.AcceptAlert';

    $(document).ready(function () {

        $(".accept-alert").each(function () {
            var acceptAlert = $(this).appendTo($("body"));
            var cookiesName = acceptAlertCookieName + acceptAlert.data('cookieskey');
            var acceptAlertCookie = $.cookie(cookiesName);
            if (acceptAlertCookie != undefined) {
                acceptAlert.remove();
                return;
            }
            var okBtn = acceptAlert.find("button");
            okBtn.on("click", function () {
                $.cookie(cookiesName, 'OK', { expires: 365 });
                acceptAlert.animate({ height: '0' }, 500);
            });
            setTimeout(function () { acceptAlert.animate({ height: acceptAlert.find(".container").innerHeight() }, 1000); }, 1000);

        });

        $(window).on("resize", function () {
            $(".accept-alert").each(function () {
                var acceptAlert = $(this);
                acceptAlert.css("height", acceptAlert.find(".container").innerHeight());
            });

        });
    });
})(jQuery)