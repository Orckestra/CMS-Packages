(function ($, undefined) {

    $(document).ready(function () {
     
         $(".youtube-channel-player").on("click", ".list-group-item", function (e) {
            $(this).parent().find(".list-group-item").removeClass("active");
            $(this).addClass("active");
            $(".media-heading",  ".youtube-channel").text($(this).data("title"));
        });
    });
})(jQuery);