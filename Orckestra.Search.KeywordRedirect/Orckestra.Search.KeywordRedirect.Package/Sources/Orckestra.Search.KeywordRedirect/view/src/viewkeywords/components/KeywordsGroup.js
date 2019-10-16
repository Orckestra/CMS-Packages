import React, { PropTypes } from 'react';
import Table from '../../components/presentation/Table.js';
import { getString } from '../../common/coreActions.js';
import Keywords from './Keywords';

import styled from 'styled-components';
import colors from 'c1-cms/console/components/colors.js';

const Unpublished = styled.span`
  font-style: italic;
  color: ${colors.mutedTextColor};
  a {
    color: ${colors.mutedTextColor};
  }
`;

const KeywordsGroup = props => {
  return <div>!!!!!!!!!!</div>;
};

KeywordsGroup.propTypes = {};

export default KeywordsGroup;
