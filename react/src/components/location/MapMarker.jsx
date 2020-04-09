import React from "react";
import PropTypes from "prop-types";
const MapMarker = props => {
  return <div>{props.text}</div>;
};
MapMarker.propTypes = {
  text: PropTypes.shape({})
};

export default MapMarker;
