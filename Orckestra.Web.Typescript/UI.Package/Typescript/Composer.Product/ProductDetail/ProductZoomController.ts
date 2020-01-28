///<reference path='../Product/ProductController.ts' />

module Orckestra.Composer {
    export class ProductZoomController extends Orckestra.Composer.ProductController {

        private allImages = {};

        public initialize() {

            super.initialize();
            this.initZoom();

            this.eventHub.subscribe('productDetailSelectedVariantIdChanged', (e) => this.updateModalImages(e));
        }

        protected openZoom(event: JQueryEventObject) {
            event.preventDefault();
            $('.modal-fullscreen').modal();
        }

        protected changeZoomedImage(event: JQueryEventObject) {
            let context$: JQuery = $(event.target),
                largeImage: HTMLImageElement = <HTMLImageElement>document.querySelector('.js-zoom-image'),
                selector: string = event.target.tagName, // Clicked HTML element
                $largeImage: JQuery = $(largeImage);
            event.preventDefault();

            if (selector.toLocaleLowerCase() === 'img') {
                var src = context$.attr('data-zoom-src');
                $('.js-zoom-thumbnails').find('a').removeClass('active');
                context$.parent().addClass('active');
                $largeImage.attr('src', src);
            }
        }

        protected errorZoomedImage(event: JQueryEventObject) {
            var $element = $(event.target),
                fallbackImageUrl = $element.attr('data-fallback-image-url');

            $element.attr('src', fallbackImageUrl);
        }

        protected initZoom() {
            $(document).on('click', '.js-zoom', (event: JQueryEventObject) => this.openZoom(event));
            $(document).on('click', '.js-zoom-thumbnails', (event: JQueryEventObject) => this.changeZoomedImage(event));
            $('.js-zoom-image').on('error', (event: JQueryEventObject) => this.errorZoomedImage(event));
        }

        protected updateModalImages(e) {
            $('.js-zoom-thumbnails').html('');
            $('.js-thumbnails[data-variant="' + e.data.selectedVariantId + '"]').find('a').each((index, el) => {
                $(el).clone().appendTo('.js-zoom-thumbnails');
                if ($(el).hasClass('active')) {
                    $(el).find('img').click();
                }
             });
        }
    }
}
