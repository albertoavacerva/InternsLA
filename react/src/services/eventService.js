import axios from "axios";
import * as serviceHelper from "./serviceHelpers";

let paginate = (pageIndex, pageSize) => {
  const config = {
    method: "GET",
    url:
      "https://localhost:50001/api/events/paginate?pageIndex=" +
      pageIndex +
      "&pageSize=" +
      pageSize,

    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };
  return axios(config)
    .then(serviceHelper.onGlobalSuccess)
    .catch(serviceHelper.onGlobalError);
};

let add = data => {
  const config = {
    method: "POST",
    data: data,
    url: "https://localhost:50001/api/events",

    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };
  return axios(config)
    .then(serviceHelper.onGlobalSuccess)
    .catch(serviceHelper.onGlobalError);
};

let addParticipant = data => {
  const config = {
    method: "POST",
    data: data,
    url: "https://localhost:50001/api/events/participant",

    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };
  return axios(config)
    .then(serviceHelper.onGlobalSuccess)
    .catch(serviceHelper.onGlobalError);
};

let update = (data, id) => {
  const config = {
    method: "PUT",
    data: data,
    url: "https://localhost:50001/api/events/" + id,

    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };
  return axios(config)
    .then(serviceHelper.onGlobalSuccess)
    .catch(serviceHelper.onGlobalError);
};

let search = (search, pageIndex, pageSize) => {
  const config = {
    method: "GET",
    url:
      "https://localhost:50001/api/events/search?search=" +
      search +
      "&pageIndex=" +
      pageIndex +
      "&pageSize=" +
      pageSize,

    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };
  return axios(config)
    .then(serviceHelper.onGlobalSuccess)
    .catch(serviceHelper.onGlobalError);
};

let getById = id => {
  const config = {
    method: "GET",
    url: "https://localhost:50001/api/events/" + id,

    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };
  return axios(config)
    .then(serviceHelper.onGlobalSuccess)
    .catch(serviceHelper.onGlobalError);
};

let getParticipantByUserId = id => {
  const config = {
    method: "GET",
    url: "https://localhost:50001/api/events/participant/" + id,

    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };
  return axios(config)
    .then(serviceHelper.onGlobalSuccess)
    .catch(serviceHelper.onGlobalError);
};

let updateStatus = (data, id) => {
  const config = {
    method: "PUT",
    data: data,
    url: "https://localhost:50001/api/events/status/" + id,

    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" }
  };
  return axios(config)
    .then(serviceHelper.onGlobalSuccess)
    .catch(serviceHelper.onGlobalError);
};

export {
  paginate,
  add,
  update,
  search,
  getById,
  addParticipant,
  getParticipantByUserId,
  updateStatus
};
