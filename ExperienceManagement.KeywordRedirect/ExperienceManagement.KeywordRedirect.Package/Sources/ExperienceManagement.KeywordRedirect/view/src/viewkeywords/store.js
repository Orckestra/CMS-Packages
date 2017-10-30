import { createStore, applyMiddleware, compose, combineReducers } from 'redux';
import ReduxThunk from 'redux-thunk';
import initState from './initState.js';

import keywords from './state/keywordsReducer.js';

let reducers = {
	keywords
};

const consoleReducers = combineReducers(reducers);

export default function configureStore(initialState, reducers = consoleReducers) {
		let store = createStore(reducers, initialState, compose(
			applyMiddleware(ReduxThunk),
			window.devToolsExtension ? window.devToolsExtension() : f => f
		));
		initState(store);
	return store;
}
