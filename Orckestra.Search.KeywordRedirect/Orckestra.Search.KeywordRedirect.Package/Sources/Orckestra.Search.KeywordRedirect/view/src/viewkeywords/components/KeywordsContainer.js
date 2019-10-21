import { connect } from "react-redux";
import KeywordsGroup from "./KeywordsGroup";
import _ from "lodash/fp";

const mapStateToProps = state => {
  const items = state.keywords.get("items").toArray();
  const keywordsGroups = _.flow(
    _.groupBy(item => item.homePage || item.homePageUnpublished),
    _.toPairs,
    _.map(([key, keywords]) => ({
      homePage: key,
      keywords: _.sortBy("keyword", keywords)
    })),
    _.sortBy("homePage")
  )(items);

  return {
    keywordsGroups,
    isLoading: state.keywords.get("isLoading")
  };
};

const container = connect(mapStateToProps)(KeywordsGroup);

export default container;
