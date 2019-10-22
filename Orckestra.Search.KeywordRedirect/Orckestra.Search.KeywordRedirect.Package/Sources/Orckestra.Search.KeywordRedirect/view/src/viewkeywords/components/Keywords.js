import React from "react";
import PropTypes from "prop-types";
import Table from "../../components/presentation/Table.js";
import { getString } from "../../common/coreActions.js";

import styled from "styled-components";
import colors from "c1-cms/console/components/colors.js";

const Unpublished = styled.span`
  font-style: italic;
  color: ${colors.mutedTextColor};
  a {
    color: ${colors.mutedTextColor};
  }
`;

const Keywords = ({ keywords }) => (
  <div>
    <Table>
      <thead>
        <tr className="head">
          <th>{getString("KeywordLabel")}</th>
          <th>{getString("LandingPageLabel")}</th>
          <th>
            {getString(
              "Composite.Plugins.GeneratedDataTypesElementProvider:PublishDate.Label"
            )}
          </th>
          <th>
            {getString(
              "Composite.Plugins.GeneratedDataTypesElementProvider:UnpublishDate.Label"
            )}
          </th>
          <th></th>
        </tr>
      </thead>
      <tbody>
        {keywords.length &&
          keywords.map((item, index) => (
            <tr key={index}>
              <td>
                {item.keyword}{" "}
                {item.keywordUnpublished ? (
                  <Unpublished> ({item.keywordUnpublished}) </Unpublished>
                ) : null}
              </td>
              <td>
                <a href={item.landingPage} target="_blank">
                  {item.landingPage}
                </a>
                {item.landingPageUnpublished ? (
                  <Unpublished>
                    {" "}
                    <a href={item.landingPageUnpublished} target="_blank">
                      {item.landingPageUnpublished}
                    </a>
                  </Unpublished>
                ) : null}
              </td>
              <td>{item.publishDate}</td>
              <td>{item.unpublishDate}</td>
              <td></td>
            </tr>
          ))}
      </tbody>
    </Table>
  </div>
);

Keywords.propTypes = {
  keywords: PropTypes.arrayOf(
    PropTypes.shape({
      keyword: PropTypes.string.isRequired,
      landingPage: PropTypes.string.isRequired,
      landingPageUnpublished: PropTypes.string,
      publishDate: PropTypes.string,
      unpublishDate: PropTypes.string
    })
  ).isRequired
};

export default Keywords;
