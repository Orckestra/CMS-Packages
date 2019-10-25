import WAMPClient from 'c1-cms/console/access/wampClient.js';

/* Action types */
const prefix = 'KEYWORDS.';
export const STORE_LOADING_KEYWORDS = prefix + 'LOADING_KEYWORDS';
export const STORE_LOAD_KEYWORDS = prefix + 'LOAD_KEYWORDS';

/* Action creators */
function loadingKeywords() {
  return { type: STORE_LOADING_KEYWORDS };
}

function loadKeywords(keywords) {
  return { type: STORE_LOAD_KEYWORDS, keywords };
}

const keywordEndpointURI = 'keywords.get';
const keywordTopic = 'keywords.new';

export function loadKeywordsFromServer() {
  return dispatch => {
    dispatch(loadingKeywords());
    WAMPClient.call(keywordEndpointURI).then(response => {
      var keywords = response.argsList[0];
      dispatch(loadKeywords(keywords));
    });
  };
}

export function subscribeForUpdates() {
  return dispatch => {
    WAMPClient.subscribe(keywordTopic, () => {
      dispatch(loadKeywordsFromServer());
    });
  };
}
