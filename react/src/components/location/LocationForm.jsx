import React from "react";
import { Formik, Field } from "formik";
import { Form, FormGroup } from "reactstrap";
import locationService from "../../services/locationService";
import stateService from "../../services/stateService/stateService";
import _logger from "sabio-debug";
import PropTypes from "prop-types";
import { locationValidationSchema } from "./locationValidationSchema";
import Select from "react-select";
import lookUpService from "../../services/lookUpService/lookUpService";
import Autocomplete from "react-google-autocomplete";
import "./LocationForm.css";
import { MdDescription } from "react-icons/md";
import swal from "sweetalert";
import Geocode from "react-geocode";
import { TiLocation } from "react-icons/ti";
import { FaBuilding } from "react-icons/fa";
import * as helpers from "../../services/serviceHelpers";

class LocationForm extends React.Component {
  state = {
    formData: {
      locationTypeId: "",
      lineOne: "",
      lineTwo: "",
      city: "",
      zip: "",
      stateId: "",
      latitude: 0,
      longitude: 0,
      selectedState: [],
      selectedLocationType: [],
      mappedStates: [],
      mappedLocationTypes: []
    }
  };
  componentDidMount() {
    this.getStates();
  }

  getStates = () => {
    stateService
      .getAll()
      .then(this.getAllStatesSuccess)
      .catch(this.onLookTypeError);
  };

  getAllStatesSuccess = response => {
    const mappedStates = response.item.map(this.mapStates);

    lookUpService.getAll("LocationTypes").then(locations => {
      const mappedLocationTypes = locations.items.map(this.mapLocationType);
      this.setState({ mappedStates, mappedLocationTypes }, () =>
        this.getFormData()
      );
    });
  };

  onLookTypeError = error => {
    _logger("Get All States Error", error);
  };

  mapLocationType = locationType => {
    const result = {
      value: locationType.id,
      label: locationType.name
    };
    return result;
  };

  mapStates = state => {
    const result = {
      value: state.id,
      label: state.name
    };
    return result;
  };

  getFormData = () => {
    const locationId = this.props.match.params.id;
    const state = this.props.location.state;

    if (state) {
      const stateObj = {
        id: state.id,
        locationTypeId: state.locationTypeId,
        lineOne: state.lineOne,
        lineTwo: state.lineTwo,
        city: state.city,
        zip: state.zip,
        stateId: state.state.id,
        latitude: state.latitude,
        longitude: state.longitude,
        stateName: state.state.name,
        selectedState: this.state.mappedStates.filter(
          item => state.state.id === item.value
        ),
        selectedLocationType: this.state.mappedLocationTypes.filter(
          item => state.locationTypeId === item.value
        )
      };
      this.setState({
        formData: stateObj
      });
    } else if (locationId) {
      this.getById(locationId);
    } else {
      return this.state.formData;
    }
  };

  getById = id => {
    locationService
      .getById(id)
      .then(this.getByIdSuccess)
      .catch(this.getByIdError);
  };

  getByIdSuccess = response => {
    const newFormData = response.item;
    const stateObj = {
      id: newFormData.id,
      locationTypeId: newFormData.locationTypeId,
      lineOne: newFormData.lineOne,
      lineTwo: newFormData.lineTwo,
      city: newFormData.city,
      zip: newFormData.zip,
      stateId: newFormData.state.id,
      latitude: newFormData.latitude,
      longitude: newFormData.longitude,
      stateName: newFormData.state.name,

      selectedState: this.state.mappedStates.filter(
        item => newFormData.state.id === item.value
      ),
      selectedLocationType: this.state.mappedLocationTypes.filter(
        item => newFormData.locationTypeId === item.value
      )
    };
    this.setState({
      formData: stateObj
    });
  };

  getByIdError = error => {
    _logger("Get By Id Error", error);
  };

  handleSubmit = values => {
    _logger(values);
    const locationId = this.props.match.params.id;
    const geoAddress = values.lineOne + " " + values.city + " " + values.zip;

    this.getLatLong(geoAddress)
      .then(response => {
        values.latitude = response.results[0].geometry.location.lat;
        values.longitude = response.results[0].geometry.location.lng;
      })
      .catch("Error Submitting")
      .then(() => {
        if (!locationId) {
          locationService
            .add(values)
            .then(this.onAddSuccess)
            .catch(this.onAddError);
        } else {
          locationService
            .update(values)
            .then(this.onUpdateSuccess)
            .catch(this.onUpdateError);
        }
      });
  };

  onAddSuccess = () => {
    swal({
      title: "Success",
      text: "You successfully added a location!",
      icon: "success",
      button: "Boom!"
    });
    this.props.history.push("/locations");
  };
  onAddError = error => {
    this.props.history.push("/location/new");
    _logger("Add Error", error);
  };

  onUpdateSuccess = () => {
    swal({
      title: "Success",
      text: "You successfully updated the location!",
      icon: "success",
      button: "Boom!"
    });
    this.props.history.push("/locations");
  };

  onUpdateError = error => {
    this.props.history.push("/locations");
    _logger("Update Error", error);
  };

  handlePlaceSelect = place => {
    _logger(place);
    const address = this.getAddressObj(place.address_components);

    const newLineOne = address.street_number + " " + address.route;
    const newLineTwo = address.subpremise;
    const newCity = address.locality;
    const newZip = address.postal_code;
    const newState = address.administrative_area_level_1;
    const anObjState = this.state.mappedStates.filter(
      x => x.label === newState
    );

    this.setState(prevState => {
      const newPlace = { ...prevState.formData };
      newPlace.lineOne = newLineOne ? newLineOne : "";
      newPlace.lineTwo = newLineTwo ? newLineTwo : "";
      newPlace.city = newCity ? newCity : "";
      newPlace.zip = newZip ? newZip : "";
      newPlace.selectedState = anObjState ? anObjState : "";

      newPlace.stateName = anObjState[0].label ? anObjState[0].label : "";
      newPlace.stateId = anObjState[0].value ? anObjState[0].value : "";

      return { formData: newPlace };
    });
  };

  getLatLong = address => {
    const googleAPIKey = helpers.API_GOOGLE_MAPS_KEY;
    Geocode.setApiKey(googleAPIKey);
    Geocode.setLanguage("en");
    Geocode.enableDebug();

    return Geocode.fromAddress(address);
  };

  getAddressObj = components => {
    if (Array.isArray(components) && components.length > 0) {
      let addressObj = {};
      components.forEach(item => {
        item.types.forEach(type => {
          addressObj[type] = item.long_name;
        });
      });
      _logger(addressObj);
      return addressObj;
    } else {
      return null;
    }
  };

  handleStateChange = (value, setFieldValue) => {
    const selectedState = this.state.mappedStates.filter(
      state => state.value === value.value
    );
    setFieldValue("selectedState", selectedState);
    setFieldValue("stateId", value.value);
  };

  handleLocationTypeChange = (value, setFieldValue) => {
    const selectedLocationType = this.state.mappedLocationTypes.filter(
      locationType => locationType.value === value.value
    );
    setFieldValue("selectedLocationType", selectedLocationType);
    setFieldValue("locationTypeId", value.value);
  };

  backToLocations = () => {
    this.props.history.push("/locations");
  };
  render() {
    return (
      <Formik
        enableReinitialize={true}
        validationSchema={locationValidationSchema}
        initialValues={this.state.formData}
        onSubmit={this.handleSubmit}
      >
        {props => {
          const {
            values,
            touched,
            errors,
            handleSubmit,
            isValid,
            isSubmitting,
            setFieldValue
          } = props;
          return (
            <Form
              onSubmit={handleSubmit}
              className="locationcard col-lg-7 col-md-10 col-sm-8 p-4 ml-auto mr-auto mt-5"
            >
              <div className="row">
                <div className="col-4 mt-5">
                  <div className="locationcard ml-3">
                    <div className="locationtext text-dark text-center">
                      {values.locationTypeName ? (
                        <h2 className="font-weight-bolder">
                          {values.locationTypeName} <FaBuilding />
                        </h2>
                      ) : (
                        " "
                      )}
                      <TiLocation />
                      <h5>
                        <div>{values.lineOne}</div>
                        <div>{values.lineTwo}</div>
                        <div>
                          {values.city && values.stateName && values.zip
                            ? `${values.city}, ${values.stateName}, ${values.zip}`
                            : null}
                        </div>
                      </h5>
                    </div>
                  </div>
                </div>
                <div className="col-8">
                  <div className="m-4 pt-3">
                    {this.state.formData && this.props.match.params.id ? (
                      <h1 className="text-center p-3 text-dark">
                        Edit Location <MdDescription />
                      </h1>
                    ) : (
                      <h1 className="text-center p-3 text-dark">
                        Create Location <MdDescription />
                      </h1>
                    )}
                    <h4>Search Address</h4>
                    <Autocomplete
                      className="rounded mb-2"
                      id="google-search"
                      onPlaceSelected={this.handlePlaceSelect}
                      types={["establishment"]}
                      componentRestrictions={{ country: "us" }}
                    />
                    <div className="row ml-2">
                      <h4>Location Type</h4>
                      <span className="required">*</span>
                    </div>
                    <Select
                      name="locationTypeId"
                      placeholder="Location Type"
                      type="number"
                      value={
                        values.selectedLocationType
                          ? values.selectedLocationType[0]
                          : [{}]
                      }
                      onChange={value =>
                        this.handleLocationTypeChange(value, setFieldValue)
                      }
                      options={this.state.mappedLocationTypes}
                    />
                    <FormGroup>
                      <div className="row ml-2">
                        <h4>Address</h4>
                        <span className="required">*</span>
                      </div>
                      <Field
                        id="autocomplete"
                        name="lineOne"
                        type="text"
                        values={values.lineOne}
                        placeholder="Address"
                        autoComplete="off"
                        className={
                          errors.lineOne && touched.lineOne
                            ? "form-control error"
                            : "form-control"
                        }
                      />
                      {errors.lineOne && touched.lineOne && (
                        <span className="input-feedback text-danger">
                          {errors.lineOne}
                        </span>
                      )}
                    </FormGroup>
                    <FormGroup>
                      <h4>Suite/Floor</h4>

                      <Field
                        name="lineTwo"
                        type="text"
                        values={values.lineTwo}
                        placeholder="Suite/Floor"
                        autoComplete="off"
                        className={
                          errors.lineTwo && touched.lineTwo
                            ? "form-control error"
                            : "form-control"
                        }
                      />
                      {errors.lineTwo && touched.lineTwo && (
                        <span className="input-feedback text-danger">
                          {errors.lineTwo}
                        </span>
                      )}
                    </FormGroup>
                    <FormGroup>
                      <div className="row ml-2">
                        <h4>City</h4>
                        <span className="required">*</span>
                      </div>
                      <Field
                        name="city"
                        type="text"
                        values={values.city}
                        placeholder="City"
                        autoComplete="off"
                        className={
                          errors.city && touched.city
                            ? "form-control error"
                            : "form-control"
                        }
                      />
                      {errors.city && touched.city && (
                        <span className="input-feedback text-danger">
                          {errors.city}
                        </span>
                      )}
                    </FormGroup>
                    <FormGroup>
                      <div className="row ml-2">
                        <h4>Zip Code</h4>
                        <span className="required">*</span>
                      </div>
                      <Field
                        name="zip"
                        type="text"
                        values={values.zip}
                        placeholder="Zip Code"
                        autoComplete="off"
                        className={
                          errors.zip && touched.zip
                            ? "form-control error"
                            : "form-control"
                        }
                      />
                      {errors.zip && touched.zip && (
                        <span className="input-feedback text-danger">
                          {errors.zip}
                        </span>
                      )}
                    </FormGroup>
                    <div className="row ml-2">
                      <h4>State</h4>
                      <span className="required">*</span>
                    </div>
                    <Select
                      name="stateId"
                      type="number"
                      placeholder="State"
                      value={
                        values.selectedState ? values.selectedState[0] : [{}]
                      }
                      onChange={value =>
                        this.handleStateChange(value, setFieldValue)
                      }
                      options={this.state.mappedStates}
                    />
                    <div className="p-4 d-flex">
                      <button
                        type="submit"
                        className="btn btn-warning mr-auto"
                        onClick={this.backToLocations}
                      >
                        Back
                      </button>
                      {isValid ? (
                        <button
                          type="submit"
                          className="ml-auto btn btn-primary btn-lg"
                          disabled={!isValid || isSubmitting}
                        >
                          Submit
                        </button>
                      ) : null}
                    </div>
                  </div>
                </div>
              </div>
            </Form>
          );
        }}
      </Formik>
    );
  }
}

LocationForm.propTypes = {
  location: PropTypes.shape({
    state: PropTypes.object
  }),
  match: PropTypes.shape({
    params: PropTypes.object.isRequired
  }),
  history: PropTypes.shape({
    push: PropTypes.func.isRequired
  })
};

export default LocationForm;
