(function ($) {
    var countryCookiesName = 'Composite.Web.Html.CountrySpecificContent.ClientCountry';

    function getActiveCountry() {
        var country = getCookies(countryCookiesName);
        if (country == undefined) {
            $.ajax({
                async: false,
                url: "http://www.telize.com/geoip",
                dataType: 'json',
                success: function (json) {
                    country = json.country_code;
                    setCookies(countryCookiesName, country);
                }
            });
        }
        return country;
    }

    function getCookies(cookiesName) {
        if (typeof (Storage) !== "undefined") {
            return sessionStorage.getItem(cookiesName);
        }
        return undefined;
    }

    function setCookies(cookiesName, value) {
        if (typeof (Storage) !== "undefined") {
            sessionStorage.setItem(cookiesName, value);
        }
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