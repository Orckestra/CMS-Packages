(function ($) {
    var countryCookieName = 'Composite.Web.Html.CountrySpecificContent.ClientCountry';

    $(document).ready(function () {
        var activeCountry = $.cookie(countryCookieName);
        if (activeCountry == undefined) {
            $.ajax({
                url: '//freegeoip.net/json/',
                dataType: 'json',
                async: false,
                success: function (json) {
                    activeCountry = json.country_code;
                    $.cookie(countryCookieName, activeCountry);
                }
            });
        }

        $(".content-for-country").each(function () {
            var countries = $(this).data("countries").split(',');
            if ($.inArray(activeCountry, countries) >= 0) {
                $(this).show();
            } else {
                 $(this).remove();
            }
        });
    });
})(jQuery)