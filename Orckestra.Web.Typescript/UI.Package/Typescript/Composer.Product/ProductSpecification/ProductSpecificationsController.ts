///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/Controller.ts' />
///<reference path='../../Events/IEventInformation.ts' />
///<reference path='./ProductSpecificationsService.ts' />

module Orckestra.Composer {
    export class ProductSpecificationsController extends Orckestra.Composer.Controller {

        protected service: ProductSpecificationsService = new ProductSpecificationsService();

        public initialize() {

            super.initialize();
            this.registerSubscriptions();
            this.renderData(this.context.viewModel.variantId);
        }

        private registerSubscriptions(): void {

            this.eventHub.subscribe('productDetailSelectedVariantIdChanged', e => this.renderData(e.data.selectedVariantId));
        }

        private renderData(variantId: string): void {

            var handle: number = setTimeout(() => this.render('Attributes', { IsLoading: true }), 300);

            this.getProductSpecifications(variantId)
                .done(result => {

                    clearTimeout(handle);

                    this.render('Attributes', result);
                    this.eventHub.publish('productSpecificationsChanged', result);

                }, reason => this.handleError(reason));
        }

        private getProductSpecifications(variantId: string): Q.Promise<any> {

            var productId: string = this.context.viewModel.productId;
            return this.service.getProductSpecifications(productId, variantId);
        }

        private handleError(reason : any) {

            console.error('The selected variant changed, ' +
                'but we were unable to get the product specifications associated with that variant', reason);
        }
    }
}
