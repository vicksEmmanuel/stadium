import React from 'react';
import { Switch, BrowserRouter as Router,Route } from "react-router-dom";

import { userRoutes , authRoutes } from "./routes/allRoutes";
import Authmiddleware from "./routes/middleware/Authmiddleware";
import NonAuthmiddleware from "./routes/middleware/NonAuthMiddleware";

// import logo from './logo.svg';
import './App.css';
// import TestingSpeech from './components/TestingSpeech';

function App() {


  return (
    <React.Fragment>
					<Router>
						<Switch>
							{authRoutes.map((route, idx) => (
								<NonAuthmiddleware
									path={route.path}
									component={route.component}
									key={idx}
								/>
							))}

							{userRoutes.map((route, idx) => (
								<Authmiddleware
									path={route.path}
									component={route.component}
									key={idx}
								/>
							))}

						</Switch>
					</Router>
				</React.Fragment>
   );
}

export default App;
