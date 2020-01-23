///<reference path='../../Typings/tsd.d.ts' />
///<reference path='../App.ts' />
///<reference path='../Services/AutocompleteSearchService.ts' />

module Orckestra.Composer {
    export class AutocompleteSearchBoxController extends SearchBoxController {

        private renderedSuggestions;
        private searchService: AutocompleteSearchService;
        private searchTerm: string;

        public initialize() {
            super.initialize();
            this.searchService = new AutocompleteSearchService(EventHub.instance(), window);
            this.searchService.initialize({
                correctedSearchTerm: '',
                facetRegistry: {}
            });

            this.searchService['_baseSearchUrl'] = $('#frm-search-box').attr('action');

            let searchBox = $('#search-box');
            let datasetList = [];
            let rightSuggestionCount = 0;

            let products = this.getBloodhoundInstance('products', searchBox.data('autocomplete-limit'), '/api/search/autocomplete');
            products.initialize();
            datasetList.push(this.getDataSetInst('Products', products, 'SearchSuggestions', 'SearchSuggestionsEmpty'));

            if (searchBox.data('search-terms-enable') === 'True') {
                let searchTerms = this.getBloodhoundInstance('searchTerms', searchBox.data('search-terms-limit'), '/api/search/suggestTerms');
                searchTerms.initialize();
                datasetList.push(this.getDataSetInst('SearchTerms', searchTerms, 'SearchTermsSuggestions', 'SearchTermsSuggestionsEmpty'));
                rightSuggestionCount++;
            }

            if (searchBox.data('categories-enable') === 'True') {
                let categories = this.getBloodhoundInstance('categories', searchBox.data('categories-limit'), '/api/search/suggestCategories');
                categories.initialize();
                datasetList.push(this.getDataSetInst('Categories', categories, 'CategorySuggestions', 'CategorySuggestionsEmpty'));
                rightSuggestionCount++;
            }

            if (searchBox.data('brands-enable') === 'True') {
                let brands = this.getBloodhoundInstance('brands', searchBox.data('brand-limit'), '/api/search/suggestBrands');
                brands.initialize();
                datasetList.push(this.getDataSetInst('Brands', brands, 'BrandSuggestions', 'BrandSuggestionsEmpty'));
                rightSuggestionCount++;
            }

            $('#search-box .js-typeahead').typeahead({
                    minLength: 3,
                    highlight: true,
                    hint: true,
                    //async: true
                },
                ...datasetList
            ).on('typeahead:render', (evt, suggestions) => {
                //cache the rendered suggestion at the render complete
                this.renderedSuggestions = suggestions;

                if ($('.js-suggestion-empty').length === rightSuggestionCount) {
                    $('.tt-menu').addClass('right-empty');
                } else {
                    $('.tt-menu').removeClass('right-empty');
                }

            }).on('typeahead:asyncreceive', (evt) => {
                this.searchTerm = (evt.currentTarget as HTMLInputElement).value;

                if (this.renderedSuggestions === undefined) {
                    this.resultsNotFound(evt);
                }
            });

            $('.tt-dataset').wrapAll('<div class="suggestions-wrapper"></div>');
            $('.tt-menu .tt-dataset:not(:first)').wrapAll('<div class="suggestion-right-col"></div>');
        }

        private getBloodhoundInstance (name, limit, url, collectionName = 'Suggestions'): Bloodhound<any> {
            return new Bloodhound({
                name,
                limit,
                remote: {
                    url: `${url}?limit=${limit}`,
                    prepare: ComposerClient.prepareBloodhound,
                    transform: (response) => {
                        const suggestions = response[collectionName];
                        return Array.isArray(suggestions) && suggestions.length > 0 ? { suggestions } : {};
                    }
                },
                datumTokenizer: (datum) => Bloodhound.tokenizers.obj.whitespace((<any>datum).val),
                queryTokenizer: Bloodhound.tokenizers.whitespace
            });
        }

        private getDataSetInst(name: string, bloodhound: Bloodhound<any>, template: string, templateEmpty: string): Twitter.Typeahead.Dataset<any> {
            return {
                name,
                display: 'DisplayName',
                source: bloodhound.ttAdapter(),
                templates: {
                    notFound: (<any>Orckestra.Composer).Templates[templateEmpty],
                    suggestion: (<any>Orckestra.Composer).Templates[template]
                }
            };
        }

        private resultsNotFound(evt) {
            let element: any = evt.currentTarget;
        }

        public selectedProduct(actionContext: Orckestra.Composer.IControllerActionContext) {
            let suggestionIndex;
            let selectedSuggestion: Object;

            //sort the object to retrieve the matching sku
            $.each(this.renderedSuggestions, function (index, obj) {
                if (obj.Sku === actionContext.elementContext.data('sku').toString()) {
                    suggestionIndex = index;
                    selectedSuggestion = obj;
                }
            });
        }

        public selectedSearchTermsSuggestion(actionContext: Orckestra.Composer.IControllerActionContext) {
            let suggestion = actionContext.elementContext.data('suggestion').toString();
            $('#search-box #search-input').val(suggestion);
            $('#search-box form').submit();
        }

        public selectedCategorySuggestion(actionContext: Orckestra.Composer.IControllerActionContext) {
            let suggestion = actionContext.elementContext.data('suggestion').toString();
            let parents = actionContext.elementContext.data('parents').toString().split(',').filter((parent) => parent);
            EventHub.instance().publish('categorySuggestionClicked', {
                data: {
                    suggestion,
                    parents
                }
            });
        }

        public selectedBrandSuggestion(actionContext: Orckestra.Composer.IControllerActionContext) {
            let suggestion = actionContext.elementContext.data('suggestion').toString();
            EventHub.instance().publish('brandSuggestionClicked', {
                data: {
                    suggestion
                }
            });
        }

        public showMoreResults() {
            $('#search-box #search-input').val(this.searchTerm);
            $('#frm-search-box').submit();
        }
    }
}
