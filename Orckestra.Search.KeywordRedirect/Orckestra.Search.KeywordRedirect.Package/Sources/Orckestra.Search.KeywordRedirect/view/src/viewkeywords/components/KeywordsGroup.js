import React from 'react';
import PropTypes from 'prop-types';
import styled from 'styled-components';
import Keywords from './Keywords';

const HomePageHeader = styled.div`
  cursor: pointer;
  padding: 10px 0 6px 12px;
  display: block;
  color: #777;
  :hover {
    color: #000;
  }
`;

const HomePageHeaderLabel = styled.span`
  margin-left: 5px;
  font-size: 16px;
`;

const HomePageHeaderExpanded = styled.span`
  font-size: 14px;
  ::after {
    content: '▼';
  }
`;

const HomePageHeaderCollapsed = styled.span`
  font-size: 14px;
  ::after {
    content: '▲';
  }
`;

class KeywordsGroup extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      expanded: true,
    };
    this.handleClick = this.handleClick.bind(this);
  }

  static propTypes = {
    homePage: PropTypes.string.isRequired,
    keywords: PropTypes.array,
  };

  handleClick() {
    this.setState(state => {
      return {
        ...state,
        expanded: !state.expanded,
      };
    });
  }

  render() {
    const { homePage, keywords } = this.props;
    const { expanded } = this.state;

    return (
      <div>
        <HomePageHeader onClick={this.handleClick}>
          {expanded ? <HomePageHeaderExpanded /> : <HomePageHeaderCollapsed />}
          <HomePageHeaderLabel>{homePage}</HomePageHeaderLabel>
        </HomePageHeader>
        {expanded && <Keywords keywords={keywords} />}
      </div>
    );
  }
}

export default KeywordsGroup;
