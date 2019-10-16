import { connect } from 'react-redux';
import KeywordsGroup from './KeywordsGroup';
import _ from 'lodash/fp';

const mapStateToProps = state => {
  const items = state.keywords.get('items').toArray();
  console.log(items);

  const keywordsGroups = _.flow(
    _.groupBy('homePage'),
    _.toPairs,
    _.map(([key, value]) => ({ homePage: key, keywords: value }))
  )(items);

  console.log(keywordsGroups);

  return {
    keywordsGroups
  };
};

const container = connect(mapStateToProps)(KeywordsGroup);

export default container;
