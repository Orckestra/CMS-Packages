import styled from 'styled-components';
import colors from 'c1-cms/console/components/colors.js';

const Table = styled.table`
        width: 100%;
        max-width: 100%;
        margin: 0;
        border-collapse: collapse;
        border-spacing: 0;

        td, th {
                overflow: hidden;
                user-select: none;
                white-space: nowrap;
                border-bottom: 1px solid ${colors.buttonDropShadowColor} !important;
                padding: 11px 6px 12px 18px !important; // use !important to overwrite old package's CSS styles.
                text-align: left;

                &.icon-cell {
                        width: 60px;
                }

                &:last-child{
                    width: 100%;
                }
        }

        th {
                font-size: 13px;
        }

        td{
                font-size: 14px;
        }

        th {
                padding: 6px 6px 6px 18px !important;
        }

        .head {
                th {
                        background: ${colors.fieldsetBackgroundColor} !important; // use !important to overwrite old package's CSS styles.
                        border-color: ${colors.buttonDropShadowColor} !important;
                }
        }

        tr.primary {
                td, th {
                        color: ${colors.buttonHighlightColor};
                }
        }

        .text-center {
                td, th {
                        text-align: center;
                }
        }
`;

export default Table;
