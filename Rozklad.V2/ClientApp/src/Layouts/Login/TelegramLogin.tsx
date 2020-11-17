import React, {useEffect, useState} from "react";
import TelegramLoginButton from "./TelegramLoginButton" ;
import {GetGroups} from "../../services/groupService";
import GroupSelect from "./GroupSelect";
import {UserFromTelegram} from "../../models/UserFromTelegram";
import StudentAuthService from "../../services/studentAuthService";
import {AuthRequestData} from "../../models/AuthRequestData";
import {Redirect} from "react-router-dom";
import {Button, Modal} from "react-bootstrap";
import TelegramLoginForSite from "./TelegramLoginForSite";


function TelegramLogin() {
    const [user, setUser] = useState<UserFromTelegram>()
    const [groups, setGroups] = useState<string[]>([])
    const [hiddenGroup, setHiddenGroup] = useState<boolean>(true)
    const [hiddenTelega, setHiddenTelega] = useState<boolean>(false)
    const [groupName, setGroupName] = useState<string>();
    const [loginType, setLoginType] = useState<"login" | "groupWatch">("login")
    const [redirect, setRedirect] = useState<boolean>(false)
    const [showModal, setShowModal] = useState<boolean>(false)
    const [showBack, setShowBack] = useState<boolean>(false)

    const dataOnAuth = (user: UserFromTelegram) => {
        console.log(user)
        setUser(user)
        setHiddenGroup(false)
        setHiddenTelega(true)
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
    const service = new StudentAuthService()
    const onGroupChoosen = (groupString: string) => {
        if (!groups.includes(groupString.trim().toLowerCase())) {
            // toast error 
            Show()
            return
        }
        setGroupName(groupString);
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
            const response = service.login(dataToAith).then(r => {
                setRedirect(true)
            }).catch(
                e => console.error(e)
            )
        }
    }
    const Show = () => {
        setShowModal(true)
    }
    const onHide = () => {
        setShowModal(false)
    }
    const group = service.getGroup()
    return (
        <div>
            <Modal show={showModal} onHide={onHide}>
                <Modal.Header closeButton>
                    <Modal.Title>Помилка</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <p>Данної групи немає в базі даних!</p>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={onHide}>
                        Закрити
                    </Button>
                </Modal.Footer>
            </Modal>
            {hiddenTelega ? <div/> :
                <div><TelegramLoginButton botName={"RozkladDevelopment_bot"}
                                          buttonSize="large"
                                          cornerRadius={2}
                                          dataOnauth={dataOnAuth}
                                          requestAccess={"write"}
                                          usePic={true}/>
                    <a className="btn btn-secondary" role="button" onClick={() => {
                        setHiddenTelega(true);
                        setHiddenGroup(false);
                        setLoginType("groupWatch")
                        setShowBack(true)
                    }}>
                <span className="text-white text" style={{display: "block"}}>
                    Переглянути без логіну
                </span>
                    </a>
                </div>
            }
            <hr/>
            {renderRedirect()}

            {groups.length === 0 || hiddenGroup ? <div/> :
                <GroupSelect groups={groups} onSubmit={onGroupChoosen} groupValue={group}/>}
            {showBack ? <button className="btn btn-circle btn-primary my-1" onClick={() => {
                setShowBack(false)
                setHiddenTelega(false)
                setLoginType("login")
            }}><i className="fa fa-arrow-up"/></button> : <div/>}
        </div>
    )
}

export default TelegramLogin