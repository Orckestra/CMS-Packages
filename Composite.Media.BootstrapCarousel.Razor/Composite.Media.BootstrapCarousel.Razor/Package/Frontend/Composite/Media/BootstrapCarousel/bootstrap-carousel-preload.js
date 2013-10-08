$(document).ready(function () {

    $('.carousel').each(function () {
        var self = $(this);
        var images = self.find('.cover').map(function () {
            return $(this).data('src')
        }).get();
        var indicators = $(this).find('.carousel-indicators');

        $.imgpreload(images[0], function () { //preload first image
            self.removeClass('load-image');

            self.find('.item:first-child').addClass('active');
            images.shift();

            $.imgpreload(images, function () { //preload all images and run carousel
                self.carousel({
                    interval: self.data('interval')
                });

            });
        });
        
        if (indicators.length == 0) {
            $(this).addClass('without-indicators'); // add class to carousel without indicators
        }
    });

})