import React from "react";
import locationService from "../../services/locationService";
import LocationCard from "./LocationCard";
import PropTypes from "prop-types";
import Pagination from "rc-pagination";
import "rc-pagination/assets/index.css";
import "./Location.css";
import swal from "sweetalert";
import Search from "../utility/Search";
import _logger from "sabio-debug";

class Location extends React.Component {
  state = {
    locations: [],
    mappedLocation: [],
    searchText: "",
    isSearching: false,
    pagination: {
      current: 1,
      totalCount: 0,
      pageSize: 3,
    },
  };

  componentDidMount() {
    this.getAllLocations(
      this.state.pagination.current - 1,
      this.state.pagination.pageSize
    );
  }

  mapLocation = (location) => (
    <LocationCard
      location={location}
      key={location.id}
      handleEdit={this.handleEdit}
      handleDelete={this.handleDelete}
    />
  );

  getAllLocations = (pageIndex, pageSize) => {
    locationService
      .paginate(pageIndex, pageSize)
      .then(this.onPaginateSuccess)
      .catch(this.onPaginateError);
  };

  onPaginateSuccess = (response) => {
    const locations = response.item.pagedItems;
    const mappedLocation = locations.map(this.mapLocation);

    const item = response.item;
    let pagination = {
      current: item.pageIndex + 1,
      totalCount: item.totalCount,
      pageSize: item.pageSize,
    };

    this.setState((prevState) => {
      return {
        ...prevState,
        locations,
        mappedLocation,
        pagination,
      };
    });
  };

  onPaginateError = () => {
    _logger("Paginate Error");
  };

  handleEdit = (location) => {
    this.props.history.push(`/location/${location.id}/edit/`, location);
  };

  handleDelete = (id) => {
    swal({
      title: "Are you sure?",
      text: "Once deleted, you will not be able to recover this location",
      icon: "warning",
      buttons: true,
      dangerMode: true,
    }).then((willDelete) => {
      if (willDelete) {
        this.deleteLocation(id);
        swal("Poof! Your location has been deleted!", {
          icon: "success",
        });
      } else {
        swal("Phew, that was close! Your location is safe with us!");
      }
    });
  };

  deleteLocation = (id) => {
    locationService
      .deleteById(id)
      .then(this.onDeleteSuccess(id))
      .catch(this.onDeleteError);
  };

  onDeleteSuccess = (id) => {
    const newMappedLocations = this.state.locations.filter((location) => {
      return location.id !== id;
    });
    this.setState((prevState) => {
      return {
        ...prevState,
        mappedLocation: newMappedLocations.map(this.mapLocation),
      };
    });
  };

  onDeleteError = () => {
    _logger("Delete Error");
  };

  onPageChange = (page) => {
    if (this.state.searchText && this.state.searchText.length > 0) {
      locationService
        .search(this.state.searchText, page - 1, this.state.pagination.pageSize)
        .then(this.searchSuccess)
        .catch(this.searchError);
    } else {
      this.setState(
        (prevState) => {
          return {
            ...prevState,
            pagination: { ...prevState.pagination, current: page - 1 },
          };
        },
        () => this.getAllLocations(page - 1, this.state.pagination.pageSize)
      );
    }
  };

  createLocation = () => {
    this.props.history.push("/location/new");
  };

  handleSearch = (query) => {
    this.setState({ searchText: query });

    const page = this.state.pagination;
    locationService
      .search(query, page.current - 1, page.pageSize)
      .then(this.searchSuccess)
      .catch(this.searchError);
  };
  searchSuccess = (res) => {
    _logger("search success");
    this.onPaginateSuccess(res);
  };

  searchError = (error) => {
    _logger("search error", error);
    this.resetState();
  };

  resetState = () => {
    this.setState((prevState) => {
      return {
        ...prevState,
        locations: [],
        mappedLocation: [],
        pagination: {
          current: 1,
          totalCount: 0,
          pageSize: 3,
        },
      };
    });
  };

  resetSearch = () => {
    this.setState({ searchText: "", isSearching: false }, () =>
      this.getAllLocations(0, this.state.pagination.pageSize)
    );
  };

  render() {
    return (
      <>
        <div className="row">
          <div className="col-8">
            <button
              className="btn btn-primary ml-5 mt-4 mb-2 float-left"
              type="button"
              onClick={this.createLocation}
            >
              Create
            </button>
          </div>
          <div className="col-lg-3 col-md-4 col-sm-4 mt-4 mb-2 float-right">
            <Search
              getAllPaginated={this.getAllLocations}
              searchBtnClick={this.handleSearch}
              updateSearchQuery={this.resetSearch}
              searchQuery={this.state.searchText}
              isSearching={this.state.isSearching}
            ></Search>
          </div>
        </div>
        <div className="row ml-1 mr-1 mt-2">
          {this.state.mappedLocation.length > 0 ? (
            <>{this.state.mappedLocation}</>
          ) : (
            <h1> No Records Found...</h1>
          )}
        </div>
        <div className="d-flex justify-content-center">
          <Pagination
            onChange={this.onPageChange}
            current={this.state.pagination.current}
            pageSize={this.state.pagination.pageSize}
            total={this.state.pagination.totalCount}
          />
        </div>
      </>
    );
  }
}

Location.propTypes = {
  history: PropTypes.shape({
    push: PropTypes.func.isRequired,
  }),
};

export default Location;
