$(document).ready(function () {

    $('.carousel').each(function () {
        var self = $(this);
        var images = self.find('.cover').map(function () {
            return $(this).data('src')
        }).get();

        $.imgpreload(images[0], function () {
            self.removeClass('load-image');

            self.find('.item:first-child').addClass('active');
            images.shift();

            $.imgpreload(images, function () {
                self.carousel({
                    interval: self.data('interval')
                });

            });
        });
    });

})