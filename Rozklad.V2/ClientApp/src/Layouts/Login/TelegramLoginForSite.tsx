import React, {useEffect, useState} from "react";
import TelegramLoginButton from "./TelegramLoginButton";
import {UserFromTelegram} from "../../models/UserFromTelegram";
import StudentAuthService from "../../services/studentAuthService";
import {AuthRequestData} from "../../models/AuthRequestData";

function TelegramLoginForSite(props : {closeFunc : Function}){
    const onAuth=(user: UserFromTelegram)=>{
        const service = new StudentAuthService()
        const group = service.getGroup()
        const dataToAith: AuthRequestData = {
            telegramUser: user!,
            group: group!
        }
        const response = service.login(dataToAith).then(r =>{
            props.closeFunc()
        }).catch(
            e=>console.error(e)
        )
    }
    return (
        <TelegramLoginButton botName={"Rozklad_KpiBot"}
                             buttonSize="large"
                             cornerRadius={2}
                             dataOnauth={onAuth}
                             requestAccess={"write"}
                             usePic={true}/>
    )
}

export  default  TelegramLoginForSite