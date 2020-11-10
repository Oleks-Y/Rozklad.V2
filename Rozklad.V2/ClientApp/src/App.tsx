import React from "react";
import logo from "./logo.svg";
import "./App.css";
import { BrowserRouter, Route, Redirect } from "react-router-dom";
import LoginLayout from "./Layouts/Login/LoginLayout";
import Site from "./Layouts/Site/Site";
import StudentAuthService from "./services/studentAuthService";
import {PrivateRoute} from "./GuardedRoute";
function App() {
  const loginService = new StudentAuthService();
  const isAuthentificated = loginService.isAuthentificated();
  console.log(isAuthentificated);
  return (
    <BrowserRouter>
      <Route exact path="/login" component={LoginLayout} />
      {/*<Route exact path="/site" component ={Site}/>*/}
      <PrivateRoute path="/site" accessWithGroup={true} component={Site} auth={isAuthentificated} />
      <Route exact path="/">
        <Redirect to="/site" />
      </Route>
    </BrowserRouter>
  );
    
}

export default App;
