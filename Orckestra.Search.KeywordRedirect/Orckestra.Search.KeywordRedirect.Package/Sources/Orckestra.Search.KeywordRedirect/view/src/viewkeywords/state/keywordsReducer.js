import Immutable from "immutable";
import _ from "lodash";
import * as Actions from "./keywordsActions.js";
import queryString from "querystring";

const initialState = Immutable.Map({
  isLoading: false,
  items: Immutable.Map(),
  homePageId: _.get(
    queryString.parse(_.trimStart(location.search, "?")),
    "homePageId"
  )
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
