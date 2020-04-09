import React from "react";
import PropTypes from "prop-types";

const OrgCard = (props) => {
  let img =
    "https://amerikicklanghorne.com/wp-content/uploads/2017/04/default-image.jpg";

  if (props.member.logo && props.member.logo) {
    img = props.member.logo;
  } else {
    img =
      "https://amerikicklanghorne.com/wp-content/uploads/2017/04/default-image.jpg";
  }

  return (
    <div className="avatar-icon-wrapper rounded-circle d-80 mx-auto">
      <div className="d-block p-0 avatar-icon-wrapper rounded-circle m-0">
        <div className="rounded-circle overflow-hidden">
          <img alt="Follower" className="imgDashboard" src={img} />
        </div>
      </div>
      <div className="font-weight-bold mt-1">{`${props.member.name}`}</div>
    </div>
  );
};

OrgCard.propTypes = {
  member: PropTypes.shape({
    name: PropTypes.string,
    lastName: PropTypes.string,
    logo: PropTypes.string,
  }),
};

export default React.memo(OrgCard);
