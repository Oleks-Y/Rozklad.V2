import React from 'react';
import {Route, Redirect} from "react-router-dom";
import StudentAuthService, {UsageTypes} from "./services/studentAuthService"
// @ts-ignore
export const PrivateRoute = ({component: Component,accessWithGroup, ...rest}) => (
        <Route {...rest} render={props => {
            const authenticationService = new StudentAuthService();
            const usageType = authenticationService.getUsageType();
            if(usageType == UsageTypes.Authentificated){
                return <Component {...props} />
            }else if(usageType == UsageTypes.ByGroup){
                if(accessWithGroup){
                    return <Component {...props} />
                }
               
                return <Redirect to={{ pathname: '/login?message="Для цієї дії необхідно авторизуватись"', state: { from: props.location } }} />
            }
            else{
                return <Redirect to={{ pathname: '/login', state: { from: props.location } }} />
            }
        }}/>
    )
;
