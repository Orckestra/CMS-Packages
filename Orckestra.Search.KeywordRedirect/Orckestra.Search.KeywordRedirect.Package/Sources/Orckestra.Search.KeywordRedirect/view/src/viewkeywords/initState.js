import {
  loadKeywordsFromServer,
  subscribeForUpdates
} from "./state/keywordsActions.js";

export default function initState(store) {
  store.dispatch(loadKeywordsFromServer());
  store.dispatch(subscribeForUpdates());
}
