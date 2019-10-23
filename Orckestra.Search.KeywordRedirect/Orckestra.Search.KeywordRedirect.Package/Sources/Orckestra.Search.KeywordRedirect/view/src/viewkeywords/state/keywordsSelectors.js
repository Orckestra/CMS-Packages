import { createSelector } from 'reselect';
import _ from 'lodash/fp';

export const isLoadingSelector = createSelector(
  state => state.keywords,
  keywords => keywords.get('isLoading')
);

export const homePageIdSelector = createSelector(
  state => state.keywords,
  keywords => keywords.get('homePageId')
);

const itemsSelector = createSelector(
  state => state.keywords,
  keywords => keywords.get('items').toArray()
);

const filteredItemsSelector = createSelector(
  itemsSelector,
  homePageIdSelector,
  (items, homePageId) => (homePageId ? _.filter({ homePageId }, items) : items)
);

export const keywordsGroupsSelector = createSelector(
  filteredItemsSelector,
  _.flow(
    _.groupBy(item => item.homePage),
    _.toPairs,
    _.map(([key, keywords]) => ({
      homePage: key,
      keywords: _.sortBy('keyword', keywords),
    })),
    _.sortBy('homePage')
  )
);
