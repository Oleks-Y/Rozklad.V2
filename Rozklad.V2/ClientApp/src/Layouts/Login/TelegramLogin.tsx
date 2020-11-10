import React, {useEffect, useState} from "react";
import TelegramLoginButton from "./TelegramLoginButton" ;
import {GetGroups} from "../../services/groupService";
import GroupSelect from "./GroupSelect";
import {UserFromTelegram} from "../../models/UserFromTelegram";
import StudentAuthService from "../../services/studentAuthService";
import {AuthRequestData} from "../../models/AuthRequestData";
import {Redirect} from "react-router-dom";


function TelegramLogin() {
    const [user, setUser] = useState<UserFromTelegram>()
    const [groups, setGroups] = useState<string[]>([])
    const [hiddenGroup, setHiddenGroup] = useState<boolean>(true)
    const [hiddenTelega, setHiddenTelega] = useState<boolean>(false)
    const [groupName, setGroupName] = useState<string>();
    const [loginType, setLoginType] = useState<"login" | "groupWatch">("login")
    const [redirect, setRedirect] = useState<boolean>(false)
    const dataOnAuth = (user: UserFromTelegram) => {
        console.log(user)
        setUser(user)
        setHiddenGroup(false)
    }
    useEffect(() => {
        if (groups.length == 0) {
            GetGroups().then(
                (result) => {
                    setGroups(result)
                    console.log(groups)
                }
            )
                .catch(e => console.error(e))
        }

    })
    const renderRedirect = () => {
        if (redirect) {
            return <Redirect to="/site"/>;
        }
    };
    const onGroupChoosen = (groupString: string) => {
        if(!groups.includes(groupString.trim().toLowerCase())){
            // toast error 
            alert("Даної групи немає в базі даних!")
            return
        }
        setGroupName(groupString);
        const service = new StudentAuthService()
        service.logout()
        if (user == null) {
            // login with group
            service.groupLogin(groupString)
            setRedirect(true)
        } else {
            // normal login
            const dataToAith: AuthRequestData = {
                telegramUser: user!,
                group: groupString
            }
            const response = service.login(dataToAith).then(r =>{
                setRedirect(true)
            }).catch(
                e=>console.error(e)
            )
        }
    }
    return (
        <div>
            {hiddenTelega ? <div/> :
                <div><TelegramLoginButton botName={"Rozklad_KpiBot"}
                                          buttonSize="large"
                                          cornerRadius={2}
                    // dataAuthUrl={`${window.location.protocol}//${window.location.host}/site`}
                                          dataOnauth={dataOnAuth}
                                          requestAccess={"write"}
                                          usePic={true}/>
                    <a className="btn btn-secondary" role="button" onClick={() => {
                        setHiddenTelega(true);
                        setHiddenGroup(false);
                        setLoginType("groupWatch")
                    }}>
                <span className="text-white text" style={{display: "block"}}>
                    Переглянути без логіну
                </span>
                    </a>
                </div>
            }
            <hr/>
            {renderRedirect()}
            {/* todo pass ref here*/}
            {groups.length === 0 || hiddenGroup ? <div/> : <GroupSelect groups={groups} onSubmit={onGroupChoosen}/>}
            {/*<GroupSelect groups={groups} onSubmit={onGroupChoosen} />*/}
        </div>
    )
}

export default TelegramLogin