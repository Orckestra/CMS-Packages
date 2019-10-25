import React from 'react';
import PropTypes from 'prop-types';
import { getString } from '../../common/coreActions.js';
import styled from 'styled-components';
import colors from 'c1-cms/console/components/colors.js';
import KeywordsGroup from './KeywordsGroup';

const Loading = styled.span`
  font-style: italic;
  font-size: 16px;
  color: ${colors.mutedTextColor};
  padding-top: 10px;
  padding-left: 20px;
  display: block;
`;

const KeywordsGroups = ({ keywordsGroups, isLoading }) => {
  if (isLoading) return <Loading>{getString('LoadingLabel')}</Loading>;

  return keywordsGroups.map(group => <KeywordsGroup {...group} />);
};

KeywordsGroups.propTypes = {
  keywordsGroups: PropTypes.array,
  isLoading: PropTypes.bool.isRequired,
};

export default KeywordsGroups;
