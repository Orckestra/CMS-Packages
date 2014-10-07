(function ($) {
    var countryCookieName = 'Composite.Web.Html.CountrySpecificContent.ClientCountry';

    function getActiveCountry() {
        var country = $.cookie(countryCookieName);
        if (country == undefined) {
            $.ajax({
                async: false,
                url: "http://www.telize.com/geoip",
                dataType: 'json',
                success: function (json) {
                    country = json.country_code;
                    $.cookie(countryCookieName, country);
                }
            });
        }
        return country;
    }

    $(document).ready(function () {
        var activeCountry = getActiveCountry();
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