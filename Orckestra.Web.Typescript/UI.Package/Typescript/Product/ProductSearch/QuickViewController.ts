/// <reference path='../Product/ProductController.ts' />
///<reference path='./IQuickViewController.ts' />

module Orckestra.Composer {
    'use strict';

    export class QuickViewController  extends Orckestra.Composer.ProductController implements Orckestra.Composer.IQuickViewController {

        protected concern: string = 'productSearch';

        private selectedRecurringOrderFrequencyName: string;
        private recurringMode: string = 'single';

        public initialize() {
            this.setConcernWithContext();
            super.initialize();
        }

        protected setConcernWithContext(): void {
            var contextualConcern: string = this.context.container.closest('[data-concern]').data('concern');

            if (contextualConcern) {
                this.concern = contextualConcern;
            }
        }

        protected registerSubscriptions() {

            super.registerSubscriptions();
            this.eventHub.subscribe(this.concern + 'QuickBuyLoaded', e => this.onQuickBuyLoaded(e));
        }

        protected onQuickBuyLoaded(e: IEventInformation) {

            this.render('ProductQuickView', e.data);
            this.productService.showQuickView();
            this.context.viewModel = JSON.parse(e.data['JsonContext'] || '{}');
            this.context.viewModel.source = e.source;

            this.setVariantId(this.context.viewModel.displayedVariantId);

            var priceDisplayBusy: UIBusyHandle = this.asyncBusy({
                msDelay: 300,
                loadingIndicatorSelector: '.loading-indicator-pricediscount'
            });

            var addToCartBusy: UIBusyHandle = this.asyncBusy({
                msDelay: 300,
                loadingIndicatorSelector: '.loading-indicator-inventory'
            });

            var promise: Q.Promise<any> = Q.all([this.calculatePrice(), this.renderData()])
                .then(() => {
                    ErrorHandler.instance().removeErrors();
                }, (reason: any) => this.onLoadingFailed(reason))
                .fin(() => {
                    priceDisplayBusy.done();
                    addToCartBusy.done();
                });
        }

        protected onLoadingFailed(reason: any) {
            console.error('Failed loading the Product Quick View');

            ErrorHandler.instance().outputErrorFromCode('QuickViewLoadFailed');
        }

        private setVariantId(variantId: string) {

            var variant = (this.productService.getVariant(variantId) || {});
            this.productService.updateSelectedKvasWith(variant.Kvas, this.concern);
        }

        protected onSelectedVariantIdChanged(e: IEventInformation) {

            this.renderData()
                .then(() => this.onSelectedVariantIdChangedSuccess(),
                    (reason: any) => this.onSelectedVariantIdChangedFailed(reason))
                .done();
        }

        protected onSelectedVariantIdChangedSuccess(): void {
            ErrorHandler.instance().removeErrors();
        }

        protected onSelectedVariantIdChangedFailed(reason: any): void {
            console.error('Error while changing the selected variant.', reason);
            this.renderUnavailableAddToCart();
            this.renderUnavailableQuantity(this.getCurrentQuantity());

            ErrorHandler.instance().outputErrorFromCode('SelectedVariantChangeFailed');
        }

        protected onSelectedKvasChanged(e: IEventInformation) {

            this.render('ProductQuickViewKvaItems', { KeyVariantAttributeItems: e.data });
        }

        protected onImagesChanged(e: IEventInformation) {

            if (this.isProductWithVariants() && this.isSelectedVariantUnavailable()) {
                this.render('MainImageContent', this.getUnavailableMainImageContent(e));
            } else {
                this.render('MainImageContent', e.data);
            }
        }

        private getUnavailableMainImageContent(e: IEventInformation): any {

            return {
                DisplayName: e.data.DisplayName,
                SelectedImage: {
                    ImageUrl: e.data.FallbackImageUrl
                }
            };
        }

        protected onPricesChanged(e: IEventInformation) {

            if (this.isProductWithVariants() && this.isSelectedVariantUnavailable()) {
                this.render('PriceDiscount', null);
            } else {
                this.render('PriceDiscount', e.data);
            }
        }

        protected renderUnavailableAddToCart(): Q.Promise<void> {

            return Q.fcall(() => this.render('AddToCartQuickView', { IsUnavailable: true }));
        }

        protected renderAvailableAddToCart(): Q.Promise<void> {

            return this.inventoryService
                       .isAvailableToSell(this.context.viewModel.Sku)
                       .then(result => {
                           this.render('AddToCartQuickView', { IsAvailableToSell: result });
                           this.renderRecurringAddToCartProductDetailFrequency();
                       });
        }

        protected completeAddLineItem(quantityAdded: any): Q.Promise<void> {

            return Q.fcall(() => this.productService.closeQuickView());
        }

        protected getListNameForAnalytics(): string {
            return this.context.viewModel.source;
        }

        private renderRecurringAddToCartProductDetailFrequency() {
            let loc = LocalizationProvider.instance(),
                recurringBubblePitch = ((loc.getLocalizedString('ProductPage', 'L_RecurringBubblePitch'))),
                availableFrequencies = this.context.viewModel.RecurringOrderFrequencies || [];

            if (!this.selectedRecurringOrderFrequencyName && availableFrequencies.length > 0) {
                this.selectedRecurringOrderFrequencyName = availableFrequencies[0].RecurringOrderFrequencyName;
            }

            this.inventoryService.isAvailableToSell(this.context.viewModel.Sku)
                .then(result => {
                    if (this.context.viewModel.IsRecurringOrderEligible && result) {
                        this.render('ProductQuickViewRecurringFrequency', {
                            recurringMode: this.recurringMode,
                            isAvailableForRecurring: this.context.viewModel.IsRecurringOrderEligible,
                            availableFrequencies: availableFrequencies.map(freq => ({
                                recurringOrderFrequencyName: freq.RecurringOrderFrequencyName,
                                displayName: freq.DisplayName
                            })),
                            selectedRecurringOrderFrequencyName: this.selectedRecurringOrderFrequencyName,
                            recurringBubblePitch: recurringBubblePitch
                        });
                    }
                });
        }

        public onRecurringOrderFrequencySelectChanged(actionContext: IControllerActionContext) {
            let element = <any>actionContext.elementContext[0],
                option = element.options[element.selectedIndex];

            if (option) {
                this.selectedRecurringOrderFrequencyName = option.value === '' ? null : option.value;
            }
        }

        public changeRecurringMode(actionContext: IControllerActionContext) {
            let container$ = actionContext.elementContext.closest('.js-recurringModes');
            container$.find('.js-recurringModeRow.selected').removeClass('selected');
            actionContext.elementContext.closest('.js-recurringModeRow').addClass('selected');
            let modeElement = actionContext.elementContext.closest('.recurring-modes');
            modeElement.find('.recurring-frequencies').collapse('toggle');
            this.recurringMode = actionContext.elementContext.val();
        }

        public addToCartButtonClick(actionContext: IControllerActionContext) {
            let frequencyName = this.recurringMode === 'single' ? null : this.selectedRecurringOrderFrequencyName;
            this.addLineItem(actionContext, frequencyName, this.context.viewModel.RecurringOrderProgramName);
        }
    }
}
