import axios from "axios";
import * as helpers from "./serviceHelpers";

const locationUrl = helpers.API_HOST_PREFIX + "/api/locations";

const paginate = (pageIndex, pageSize) => {
  const config = {
    method: "GET",
    url: locationUrl + `/paginate?pageIndex=${pageIndex}&pageSize=${pageSize}`,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };
  return axios(config)
    .then(helpers.onGlobalSuccess)
    .catch(helpers.onGlobalError);
};

const getById = id => {
  const config = {
    method: "GET",
    url: locationUrl + "/" + id,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };
  return axios(config)
    .then(helpers.onGlobalSuccess)
    .catch(helpers.onGlobalError);
};

const search = (data, pageIndex, pageSize) => {
  const config = {
    method: "GET",
    url:
      locationUrl +
      `/search?pageIndex=${pageIndex}&pageSize=${pageSize}&search=${data}`,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };
  return axios(config)
    .then(helpers.onGlobalSuccess)
    .catch(helpers.onGlobalError);
};

const getByGeo = data => {
  const config = {
    method: "GET",
    url: locationUrl + "/search/",
    data: data,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };
  return axios(config)
    .then(helpers.onGlobalSuccess)
    .catch(helpers.onGlobalError);
};

const add = data => {
  const config = {
    method: "POST",
    url: locationUrl,
    data: data,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };
  return axios(config)
    .then(helpers.onGlobalSuccess)
    .catch(helpers.onGlobalError);
};

const update = data => {
  const config = {
    method: "PUT",
    url: locationUrl + "/" + data.id,
    data: data,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };
  return axios(config)
    .then(helpers.onGlobalSuccess)
    .catch(helpers.onGlobalError);
};

const deleteById = id => {
  const config = {
    method: "DELETE",
    url: locationUrl + "/" + id,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };
  return axios(config)
    .then(helpers.onGlobalSuccess)
    .catch(helpers.onGlobalError);
};

export default {
  paginate,
  getById,
  getByGeo,
  add,
  update,
  deleteById,
  search,
  helpers
};
