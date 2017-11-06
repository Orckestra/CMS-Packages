import Immutable from 'immutable';
import * as Actions from './keywordsActions.js';

const initialState = Immutable.Map({
        items: Immutable.Map()
})

const getProductsReducer = (state = initialState, action) => {
    switch (action.type) {
        case Actions.STORE_LOAD_KEYWORDS:
            return state.set("items", Immutable.List(action.keywords));
        default:
            return state;
    }
}


export default getProductsReducer
