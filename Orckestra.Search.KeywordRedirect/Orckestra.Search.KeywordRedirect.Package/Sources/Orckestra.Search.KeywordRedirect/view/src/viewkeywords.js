import 'es6-promise/auto';
import 'url-polyfill';
import React from 'react';
import ReactDOM, { render } from 'react-dom';
import { Provider } from 'react-redux';
import configureStore from './viewkeywords/store.js';
import Keywords from './viewkeywords/components/KeywordsContainer.js';
import colors from 'c1-cms/console/components/colors.js';
import { injectGlobal } from 'styled-components';

injectGlobal`
*:focus {
        outline: 0;
}

::-webkit-scrollbar {
  width: 13px;
  height: 13px;
  background: ${colors.scrollbarTrackColor};
}

::-webkit-scrollbar-thumb {
  background: ${colors.scrollbarThumbColor};
  border: 3px solid ${colors.scrollbarTrackColor};
  border-radius: 7px;
}

::-webkit-scrollbar-thumb:hover {
  background: ${colors.buttonHighlightColor};
}

html, body {
        margin: 0;
        padding: 0;
        overflow: hidden;
        height: 100%;
        width: 100%;
}

div.entry, div.page {
        width: inherit;
        height: inherit;
}

body, input, textarea, select, button {
        font-size: 12px;
        font-family: "Segoe UI", Tahoma, sans-serif;
        color: ${colors.baseFontColor};
}
`;

const initialState = {};
const store = configureStore(initialState);

ReactDOM.render(
  <Provider store={store}>
    <Keywords />
  </Provider>,
  document.querySelector('body > div.entry')
);
