import React from "react";
import TelegramLoginButton from "./TelegramLoginButton" ;

function TelegramLogin() {
    const dataOnAuth = (user: any) => {
        console.log(user)
    }
    return (
        <TelegramLoginButton botName={"Rozklad_KpiBot"}
                             buttonSize="large" 
                             cornerRadius={2}
                             // dataAuthUrl={`${window.location.protocol}//${window.location.host}/site`}
                            dataOnauth={dataOnAuth}
                            requestAccess={"write"}
                            usePic={true}/>
    )
}

export default TelegramLogin