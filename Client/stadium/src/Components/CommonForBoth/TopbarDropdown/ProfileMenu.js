import React, { useState, useEffect } from 'react';
import { Dropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';

//i18n
import { withTranslation } from 'react-i18next';
// Redux
import { withRouter, Link } from 'react-router-dom';


// users
import user1 from '../../../assets/images/users/avatar-1.jpg';


const ProfileMenu = (props) => {
   // Declare a new state variable, which we'll call "menu"
   const [menu, setMenu] = useState(false);
  
   const [username, setusername] = useState("Admin");

  return (
    <React.Fragment>
                <Dropdown isOpen={menu} toggle={() => setMenu(!menu)} className="d-inline-block" >
                    <DropdownToggle className="btn header-item waves-effect" id="page-header-user-dropdown" tag="button">
                        <img className="rounded-circle header-profile-user " src={user1} alt="" />
                        <span className="d-none d-xl-inline-block ml-2 mr-1">{username}</span>
                        <i className="mdi mdi-chevron-down d-none d-xl-inline-block"></i>
                    </DropdownToggle>
                    <DropdownMenu right>
                        <DropdownItem tag="a"  href="/profile"> <i className="bx bx-user font-size-16 align-middle mr-1"></i>{props.t('Profile')}  </DropdownItem>
                        <DropdownItem tag="a" href="/crypto-wallet"><i className="bx bx-wallet font-size-16 align-middle mr-1"></i>{props.t('My Wallet')}</DropdownItem>
                        <DropdownItem tag="a" href="#"><span className="badge badge-success float-right">11</span><i className="mdi mdi-settings font-size-17 align-middle mr-1"></i>{props.t('Settings')}</DropdownItem>
                        <DropdownItem tag="a" href="auth-lock-screen"><i className="bx bx-lock-open font-size-16 align-middle mr-1"></i>{props.t('Lock screen')}</DropdownItem>
                        <div className="dropdown-divider"></div>
                        <Link to="/logout" className="dropdown-item">
                            <i className="bx bx-power-off font-size-16 align-middle mr-1 text-danger"></i>
                            <span>{props.t('Logout')}</span>
                        </Link>
                    </DropdownMenu>
                </Dropdown>
            </React.Fragment>
  );
}

export default withRouter(withTranslation()(ProfileMenu));

