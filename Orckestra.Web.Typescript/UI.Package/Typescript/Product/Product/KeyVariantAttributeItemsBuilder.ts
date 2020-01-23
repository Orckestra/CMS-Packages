///<reference path='../../../Typings/tsd.d.ts' />
///<reference path='../../Mvc/IControllerContext.ts' />
///<reference path='../../Mvc/ComposerClient.ts' />
///<reference path='../../Events/EventHub.ts' />

module Orckestra.Composer {
    'use strict';

    //
    //Isolated logic, this class rebuild the keyVariantAttributeItems array
    //as expected by KvaItems.hbs
    //TODO: Instancier dans productService (passer dans le constructeur)
    export class KeyVariantAttributeItemsBuilder {

        private context: Orckestra.Composer.IControllerContext;

        //Passer viewModel
        constructor(context: Orckestra.Composer.IControllerContext) {
            if (!context) {
                throw new Error('Error: context is required');
            }
            if (!context.viewModel) {
                throw new Error('Error: context.viewModel is required');
            }

            this.context = context;
        }

        //Find possible kva value states bases on the given selection
        //
        //<returns>
        // KeyVariantAttributeItem ViewModel ready for render
        //</returns>
        public BuildKeyVariantAttributeItemsFor(selectedKvas: any) {
            selectedKvas = selectedKvas || {};

            //Get the last known KvaState and use it as a starting point
            var keyVariantAttributeItems = this.context.viewModel.keyVariantAttributeItems || [];

            //Initiate
            var reverseKvaLookup  = this.InitiateKVAStateFor(keyVariantAttributeItems, selectedKvas);

            //Find
            var reachableVariants = this.FindReachableVariantsFrom(keyVariantAttributeItems, selectedKvas);

            //Enable
            this.EnableKVAState(reverseKvaLookup, reachableVariants, selectedKvas);

            //Memoize the KvaState for later
            this.context.viewModel.keyVariantAttributeItems = keyVariantAttributeItems;
            //

            return keyVariantAttributeItems;
        }

        //Toggle the Selected state and Disable everything
        //After this initial state, the KVAs will either be
        // Selected or Disable
        //
        //<returns>
        //  A Lookup of all KVA for later easy access using
        //  lookup[propertyName][value]
        //</returns>
        private InitiateKVAStateFor(keyVariantAttributeItems: any, selectedKvas: any): any {
            var reverseLookup = {};

             _.each(keyVariantAttributeItems, function(kva: any) {
                 var propertyName = kva.PropertyName;
                 var selectedValue = selectedKvas[propertyName];

                 reverseLookup[propertyName] = {};
                 _.each(kva.Values, function(val: any) {
                     val.Selected = val.Value === selectedValue;
                     val.Disabled = true;

                     reverseLookup[propertyName][val.Value] = val;
                 });
             });

            return reverseLookup;
        }

        //Find all reachable variants from the given configuration
        //Those are variant that could possibly be reach by changing
        //one and only one KVA value
        //<returns>
        //  An array of Variants with all their properties
        //</returns>
        private FindReachableVariantsFrom(keyVariantAttributeItems: any, selectedKvas: any): any {
            var allVariants = (this.context.viewModel.allVariants || {});

            var possibleVariantsLookup = {};

            //Changing Selection
             _.each(selectedKvas, function(value: string, propertyName: string) {
                 var possibleMove = _.omit(selectedKvas, propertyName);

                 var v = _.each(_.filter(allVariants, { Kvas: possibleMove }), function(variant: any) {
                     possibleVariantsLookup[variant.Id] = variant;
                 });
             });

            //
            var variants = _.mapValues(possibleVariantsLookup, _.identity);

            return variants;
        }

        //Enable the KVAs based on the reachable variants
        //Using the reverse lookkup for faster access
        private EnableKVAState(reverseKvaLookup: any, reachableVariants: any, selectedKvas: any): any {
            //Enable reachable states
            _.each(reachableVariants, function(variant: any, variantId: string) {
                _.each(variant.Kvas, function(value: string, propertyName: string) {
                    var kva = reverseKvaLookup[propertyName] || [];
                    var val = kva[value] || {};
                    val.Disabled = false;
                });
            });

            //If the current selection match nothing, disable it.
            var selectedVariant = _.find(reachableVariants, { Kvas: selectedKvas });
            if (!selectedVariant) {
                _.each(selectedKvas, function(value: string, propertyName: string) {
                    var kva = reverseKvaLookup[propertyName] || [];
                    var val = kva[value] || {};
                    val.Disabled = true;
                });
            }
        }
    }
}
