import React, { Component } from "react";

import { withRouter } from "react-router-dom";

// Layout Related Components
import Sidebar from "./Sidebar";
import Footer from "./Footer";

const initLayoutValue = {
	layoutType: "vertical",
	layoutWidth: "fluid",
	leftSideBarTheme: "dark",
	leftSideBarType: "default",
	topbarTheme: "light",
	isPreloader: false,
	showRightSidebar: false,
	isMobile: false,
	showSidebar : true,
	leftMenu : true
}

class Layout extends Component {
  constructor(props) {
    super(props);
    this.state = {
      isMobile: /iPhone|iPad|iPod|Android/i.test(navigator.userAgent)
    };
    this.toggleMenuCallback = this.toggleMenuCallback.bind(this);
  }

  capitalizeFirstLetter = string => {
    return string.charAt(1).toUpperCase() + string.slice(2);
  };

  componentDidMount() {

    if (initLayoutValue.isPreloader === true) {
      document.getElementById('preloader').style.display = "block";
      document.getElementById('status').style.display = "block";

      setTimeout(function () {

        document.getElementById('preloader').style.display = "none";
        document.getElementById('status').style.display = "none";

      }, 2500);
    }
    else {
      document.getElementById('preloader').style.display = "none";
      document.getElementById('status').style.display = "none";
    }

    // Scroll Top to 0
    window.scrollTo(0, 0);
    let currentage = this.capitalizeFirstLetter(this.props.location.pathname);

    document.title =
      currentage + " | Stadium";

  }
  toggleMenuCallback = () => {
    if (initLayoutValue.leftSideBarType === "default") {
      initLayoutValue.leftSideBarType = "default";
      initLayoutValue.isMobile = this.state.isMobile;
    } else if (this.props.leftSideBarType === "condensed") {
      initLayoutValue.leftSideBarType = "condensed";
      initLayoutValue.isMobile = this.state.isMobile;
    }
  };

  render() {
    return (
      <React.Fragment>
        <div id="preloader">
          <div id="status">
            <div className="spinner-chase">
              <div className="chase-dot"></div>
              <div className="chase-dot"></div>
              <div className="chase-dot"></div>
              <div className="chase-dot"></div>
              <div className="chase-dot"></div>
              <div className="chase-dot"></div>
            </div>
          </div>
        </div>

        <div id="layout-wrapper">
          <Sidebar theme={initLayoutValue.leftSideBarTheme}
            type={initLayoutValue.leftSideBarType}
            isMobile={this.state.isMobile} />
          <div className="main-content">
            {this.props.children}
          </div>
          <Footer />
        </div>
      </React.Fragment>
    );
  }
}

export default withRouter(Layout);
