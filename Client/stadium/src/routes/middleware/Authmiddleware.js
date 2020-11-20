import React, { useEffect } from "react";
import { Route,Redirect,withRouter } from "react-router-dom";

const Authmiddleware = ({
	component: Component
}) => {
	return (
		<Route
			render={props => {
			
			// here you can apply condition
			if (!localStorage.getItem("stadium--xx-xx-xx-10/20/20--authUser")) {
					return (
						<Redirect to={{ pathname: "/login", state: { from: props.location } }} />
					);
				}

				return <Component {...props} />;
			}}
		/>
	);
}

export default withRouter(Authmiddleware);

