(function ($) {

    $(document).ready(function () {

        $(window).load(function() {

            var $f = $(".filter");
            var $filterType = $('[data-filter]', $f);
            var $collection = $('.collection', $f);

            $collection.shuffle({
                speed: 400,
                itemSelector: '.collection-item'
            });

            $filterType.on('click', function(e) {
                e.preventDefault();
                var filterType = $(this).data('filter');
                $collection.shuffle('shuffle', function($el, shuffle) {
                    if (filterType == 'all') {
                        return true;
                    }
                    return $el.data('groups').indexOf(filterType) !== -1;
                });
            });

        });

    });
})(jQuery);
