import React from "react";
import PropTypes from "prop-types";
import "../organizations/Organization.css";

const OrgMainCard = props => {
  const editorg = () => {
    props.handleInfo(props);
  };

  let img =
    "https://amerikicklanghorne.com/wp-content/uploads/2017/04/default-image.jpg";

  if (props.org.logo && props.org.logo[0]) {
    img = props.org.logo;
  } else {
    img =
      "https://amerikicklanghorne.com/wp-content/uploads/2017/04/default-image.jpg";
  }

  return (
    <React.Fragment>
      <div onClick={editorg} type="button" className="card orgCard">
        <div className="card-body ">
          <img src={props.org.logo} height="250px" width="350px" alt="avatar" />
        </div>
        <div className="card-body text-center">
          <div>
            <h4 className="card-title font-weight-bold mb-2">
              {props.org.name}
            </h4>
            <p className="card-text">{props.org.headline}</p>
          </div>
        </div>

        <div className="card-footer text-center">
          <button type="button" className="btn btn-edit ">
            Dashboard
          </button>
        </div>
      </div>
    </React.Fragment>
  );
};

OrgMainCard.propTypes = {
  org: PropTypes.shape({
    id: PropTypes.number,
    name: PropTypes.string,
    headline: PropTypes.string,
    logo: PropTypes.string
  }),
  handleInfo: PropTypes.func
};

export default React.memo(OrgMainCard);
