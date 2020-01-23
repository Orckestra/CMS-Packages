/// <reference path='../../Typings/tsd.d.ts' />
/// <reference path='./IPlugin.ts' />
///<reference path='../Events/EventHub.ts' />

module Orckestra.Composer {
    export class SlickCarouselPlugin implements IPlugin {

        public initialize(window: Window, document: HTMLDocument) {
            this.subscriptEvents();
            this.initSlick();
        };

        public initSlick(): void {

            $.each($('.js-slick-carousel'), (index, element) => {
                let slickInstance = $(element);
                let slickOptions: any = {
                    arrows: true,
                    responsive: [{
                        dots: false,
                        breakpoint: 1024,
                        settings: {
                            slidesToShow: 3,
                            infinite: true
                        }
                    }]
                };
                if (!$(slickInstance).hasClass('slick-initialized')) {
                    if (slickInstance.data('slick').mobileCarousel) {
                        let nbSlideToShow = slickInstance.data('slick').mobileSlidesToShow;
                        nbSlideToShow = ( nbSlideToShow ) ? nbSlideToShow : 2;
                        let nbSlideToScroll = slickInstance.data('slick').mobileSlidesToScroll;
                        nbSlideToScroll = ( nbSlideToScroll ) ? nbSlideToScroll : 2;

                        slickOptions.responsive.push({
                            breakpoint: 768,
                            arrows: false,
                            settings: {
                                slidesToShow: nbSlideToShow,
                                slidesToScroll: nbSlideToScroll,
                                dots: true,
                                infinite: true
                            }
                        });
                    } else {
                        slickOptions.responsive.push({
                            breakpoint: 768,
                            arrows: false,
                            settings: 'unslick' // destroys slick
                        });
                    }

                    slickInstance.slick(slickOptions);
                }
            });
        };

        private subscriptEvents(): void {
            var self = this;

            $(window).on('resize', () => {
                if ($(window).width() > 768) {
                    this.initSlick();
                }
            });

            EventHub.instance().subscribe('iniCarousel', (data) => {
                this.initSlick();
            });
        }

    }
}
