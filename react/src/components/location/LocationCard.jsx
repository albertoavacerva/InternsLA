import React from "react";
import PropTypes from "prop-types";
import "./LocationCard.css";
import { FaEdit, FaBuilding, FaTrashAlt } from "react-icons/fa";
import { TiLocation } from "react-icons/ti";
import MapContainer from "./MapContainer";

const LocationCard = (props) => {
  const editLocation = () => {
    props.handleEdit(props.location);
  };

  const deleteLocation = () => {
    props.handleDelete(props.location.id);
  };

  return (
    <div className="col-lg-6 col-xl-4 col-sm-9 mx-auto">
      <div className="mb-5 maincard ml-3 mt-2 mr-3">
        <MapContainer location={props.location} />
        <div className="card-body">
          <h5 className="card-title font-weight-bold font-size-lg text-center">
            {props.location.locationTypeName}
            <FaBuilding />
          </h5>
          <div className="text-center">
            <TiLocation />
          </div>
          <p className="text-center">{`${props.location.lineOne} ${
            props.location.lineTwo ? props.location.lineTwo : ""
          }`}</p>
          <p className="text-center">
            {`${props.location.city}, ${props.location.state.name},
            ${props.location.zip}`}
          </p>
          <button type="button" onClick={editLocation} className="btn btn-edit">
            <FaEdit size="25" />
          </button>
          <button
            type="button"
            className="btn btn-delete float-right "
            onClick={deleteLocation}
          >
            <FaTrashAlt size="20" />
          </button>
        </div>
      </div>
    </div>
  );
};

LocationCard.propTypes = {
  location: PropTypes.shape({
    id: PropTypes.number,
    locationTypeName: PropTypes.string.isRequired,
    lineOne: PropTypes.string.isRequired,
    lineTwo: PropTypes.string,
    city: PropTypes.string.isRequired,
    zip: PropTypes.string.isRequired,
    state: PropTypes.shape({
      name: PropTypes.string,
    }),
  }),
  handleEdit: PropTypes.func.isRequired,
  handleDelete: PropTypes.func.isRequired,
};
export default React.memo(LocationCard);
