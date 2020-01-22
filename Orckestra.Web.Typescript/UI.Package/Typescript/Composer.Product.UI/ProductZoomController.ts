///<reference path='./ProductController.ts' />

module Orckestra.Composer {
    export class ProductZoomController extends Orckestra.Composer.ProductController {

        private allImages = {};

        public initialize() {

            super.initialize();
            this.initZoom();

            this.eventHub.subscribe('productDetailSelectedVariantIdChanged', (e) => this.updateModalImages(e));
            let vm = this.context.viewModel;
            for (let variant of vm.allVariants) {
                this.allImages[variant.Id] = variant.Images;
            }
        }

        protected openZoom(event: JQueryEventObject) {
            let context$: JQuery = $(event.target),
                index: number = parseInt($('.js-thumbnail.active').attr('data-index'), 10);

            $('.js-zoom-thumbnail').eq(index).click();
            event.preventDefault();
            $('.modal-fullscreen').modal();
        }

        protected changeZoomedImage(event: JQueryEventObject) {
            let context$: JQuery = $(event.target),
                largeImage: HTMLImageElement = <HTMLImageElement>document.querySelector('.js-zoom-image'),
                selector: string = event.target.tagName, // Clicked HTML element
                $largeImage: JQuery = $(largeImage);

            if (selector.toLocaleLowerCase() !== 'a') {
                context$ = context$.parent();
            }

            $('.js-zoom-thumbnail').removeClass('active');

            context$.addClass('active');
            $largeImage.attr('src', context$.attr('data-image'));
        }

        protected errorZoomedImage(event: JQueryEventObject) {
            var $element = $(event.target),
                fallbackImageUrl = $element.attr('data-fallback-image-url');

            $element.attr('src', fallbackImageUrl);
        }

        protected initZoom() {
            $(document).on('click', '.js-zoom', (event: JQueryEventObject) => this.openZoom(event));
            $(document).on('click', '.js-zoom-thumbnail', (event: JQueryEventObject) => this.changeZoomedImage(event));
            $('.js-zoom-image').on('error', (event: JQueryEventObject) => this.errorZoomedImage(event));

            // Select first thumbnail
            $('.js-zoom-thumbnail').eq(0).click();
        }

        protected updateModalImages(e) {
            if (e.data.initialSelectedVariantId !== e.data.selectedVariantId) {
                let modalThumbs = $('.js-zoom-thumbnail');
                let variantImages = this.allImages[e.data.selectedVariantId];
                if (!_.isNull(variantImages) && !_.isEmpty(variantImages)) {
                    // by default the platform sets 4 pictures for each product
                    modalThumbs.each(function (i) {
                        let link = $(this);
                        let image = $(this).find('img');
                        let idx = link.attr('data-index');
                        link.css({ 'display': 'none' });
                        // images are already ordered by sequence number
                        link.attr('data-image', variantImages[idx].ProductZoomImageUrl);
                        image.attr('src', variantImages[idx].ThumbnailUrl);
                    });
                }
            }
        }
    }
}
