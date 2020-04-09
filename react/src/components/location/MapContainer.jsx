import React from "react";
import PropTypes from "prop-types";
import GoogleMapReact from "google-map-react";
import MapMarker from "./MapMarker";
import { FaMapMarker } from "react-icons/fa";
import { API_GOOGLE_MAPS_KEY } from "../../services/serviceHelpers";

const MapContainer = props => {
  const center = {
    lat: props.location.latitude || 34.0407,
    lng: props.location.longitude || -118.2468
  };
  return (
    <div style={{ height: "40vh", width: "100%" }}>
      <GoogleMapReact
        bootstrapURLKeys={{ key: API_GOOGLE_MAPS_KEY }}
        defaultCenter={center}
        defaultZoom={12}
      >
        <MapMarker
          lat={props.location.latitude}
          lng={props.location.longitude}
          text={<FaMapMarker style={{ color: "black" }} />}
        />
      </GoogleMapReact>
    </div>
  );
};

export default MapContainer;

MapContainer.propTypes = {
  location: PropTypes.shape({
    latitude: PropTypes.number.isRequired,
    longitude: PropTypes.number.isRequired
  })
};
