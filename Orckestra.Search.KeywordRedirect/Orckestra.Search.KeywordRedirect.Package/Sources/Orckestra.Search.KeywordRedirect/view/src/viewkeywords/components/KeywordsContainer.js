import { connect } from "react-redux";
import KeywordsGroups from "./KeywordsGroups";
import _ from "lodash";
import {
  isLoadingSelector,
  keywordsGroupsSelector
} from "../state/keywordsSelectors";

const mapStateToProps = state => {
  return {
    keywordsGroups: keywordsGroupsSelector(state),
    isLoading: isLoadingSelector(state)
  };
};

const container = connect(mapStateToProps)(KeywordsGroups);

export default container;
