/// <reference path='../../../Typings/tsd.d.ts' />
/// <reference path='../../Mvc/Controller.ts' />
/// <reference path='../../Mvc/IControllerActionContext.ts' />
/// <reference path='../../Mvc/IControllerContext.ts' />
module Orckestra.Composer {
    'use strict';

    export class SearchSummaryController extends Orckestra.Composer.Controller {

        public initialize() {

            super.initialize();
            const vm = this.context.viewModel;

            if (vm.TotalCount === 0 && vm.Keywords) {
                this.eventHub.publish('noResultsFound', {
                    data: {
                        Keyword: vm.Keywords,
                        ListName: vm.ListName,
                    }
                }
                );
            }

            if (!_.isEmpty(vm.CorrectedSearchTerms)
                && vm.Keywords
                && vm.TotalCount !== 0) {
                this.eventHub.publish('searchTermCorrected', {
                    data: {
                        KeywordEntered: vm.Keywords,
                        KeywordCorrected: vm.CorrectedSearchTerms,
                        ListName: this.context.viewModel.ListName,
                    }
                }
                );
            }
        }
    }
}
