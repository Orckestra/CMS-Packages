import Immutable from "immutable";
import * as Actions from "./keywordsActions.js";

const initialState = Immutable.Map({
  isLoading: false,
  items: Immutable.Map()
});

const getProductsReducer = (state = initialState, action) => {
  switch (action.type) {
    case Actions.STORE_LOADING_KEYWORDS:
      return state.set("isLoading", true);
    case Actions.STORE_LOAD_KEYWORDS:
      return state
        .set("isLoading", false)
        .set("items", Immutable.List(action.keywords));
    default:
      return state;
  }
};

export default getProductsReducer;
