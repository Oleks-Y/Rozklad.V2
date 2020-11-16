import React from "react";
import Navbar from "./SiteComponents/Navbar";
import Dashboard from "./SiteComponents/Dashboard";
import Footer from "./SiteComponents/Footer";
import DedlinesPreview from "./SiteComponents/DedlinesPreview";
import {Switch, Route, Redirect, BrowserRouter} from "react-router-dom";
import Timetable from "../../Timetable/Timetable";
import NotFound from "../../NotFound";
import SubjectsLayout from "../../Subjects/SubjectsLayout"
import {PrivateRoute} from "../../GuardedRoute";

function Site() {
    return (
        <div>
            <div>
                <Navbar/>
            </div>
            <div id="wrapper">
                <div className="d-flex flex-column" id="content-wrapper">
                    <div id="content">
                        <Switch>
                            <PrivateRoute path="/site/timetable" component={Timetable} accessWithGroup={true}/>
                            <PrivateRoute path="/site/subjects" accessWithGroup={false} component={SubjectsLayout}/>
                            <Route path="/site">
                                <Redirect to="/site/timetable"/>
                            </Route>
                            <Route>
                                <NotFound/>
                            </Route>
                        </Switch>
                        <Footer/>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Site;
