import React from "react";
import { Redirect } from "react-router-dom";

import Home from "../pages/Home/Home";
import Login from "../pages/Authentication/Login";
import Create from "../pages/Authentication/Create";
import Competiton from "../pages/Football/Competition";
import Teams from "../pages/Football/Teams";

const userRoutes = [

	{ path: "/home", component: Home },	

	//Football
	{ path: "/football-competitions", component: Competiton},
	{ path: "/football-teams", component: Teams},

	// this route should be at the end of all other routes
	{ path: "/", exact: true, component: () => <Redirect to="/home" /> }
];

const authRoutes = [
	{ path: "/login", component: Login },
	{ path: "/register", component: Create},
];

export { userRoutes, authRoutes };
