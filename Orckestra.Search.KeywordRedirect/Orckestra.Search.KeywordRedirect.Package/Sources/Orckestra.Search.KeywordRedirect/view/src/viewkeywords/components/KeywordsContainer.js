
import React from 'react';
import { connect } from 'react-redux';
import Keywords from './Keywords.js';

const mapStateToProps = (state) => {
    return { 
        keywords: state.keywords.get("items")
    }
}

const mapDispatchToProps = (dispatch) => {
    return { }
}

const container = connect(mapStateToProps, mapDispatchToProps)(Keywords)

export default container