import React from "react";
import PropTypes from "prop-types";
import { getString } from "../../common/coreActions.js";
import Keywords from "./Keywords";

import styled from "styled-components";
import colors from "c1-cms/console/components/colors.js";

const Loading = styled.span`
  font-style: italic;
  font-size: 16px;
  color: ${colors.mutedTextColor};
  padding-top: 10px;
  padding-left: 20px;
  display: block;
`;

const HomePageHeader = styled.span`
  font-size: 16px;
  padding: 10px 0 6px 17px;
  display: block;
`;

const KeywordsGroup = ({ keywordsGroups, isLoading }) => {
  if (isLoading) return <Loading>{getString("LoadingLabel")}</Loading>;

  return keywordsGroups.map(group => (
    <div>
      <HomePageHeader>{group.homePage}</HomePageHeader>
      <Keywords keywords={group.keywords} />
    </div>
  ));
};

KeywordsGroup.propTypes = {
  keywordsGroups: PropTypes.arrayOf(
    PropTypes.shape({
      homePage: PropTypes.string.isRequired,
      keywords: PropTypes.array
    })
  ),
  isLoading: PropTypes.bool.isRequired
};

export default KeywordsGroup;
