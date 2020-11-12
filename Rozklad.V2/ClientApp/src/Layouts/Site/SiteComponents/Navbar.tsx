import React, {CSSProperties, useState} from "react";
import {Link} from "react-router-dom";
import StudentAuthService, {UsageTypes} from "../../../services/studentAuthService";
import {Button, Modal} from "react-bootstrap";
import TelegramLoginForSite from "../../Login/TelegramLoginForSite";
import ModalWindow from "../../../shared/Modal";

function Navbar() {
    const [showModal, setShowModal] = useState<boolean>(false)
    const navStyle: CSSProperties = {
        backgroundColor: "#37434d",
        color: "#ffffff",
    };
    const navItemStyle: CSSProperties = {
        color: "#ffffff",
    };
    const service = new StudentAuthService();
    const logout = () => {
        service.logout()
        window.location.reload()
    }
    const [isAuthentificated, setIsAuthentificated] = useState<boolean>(service.getUsageType() === UsageTypes.Authentificated)
    const loginModal = ()=>{
        setShowModal(true)
    }
    const onHide = ()=>{
        setShowModal(false)
    }
    const onHideFromTelegram = ()=>{
        setShowModal(false)
        setIsAuthentificated(true)
    }
    return (
        <nav
            className="navbar navbar-light navbar-expand-md sticky-top bg-primary navigation-clean-button"
            style={navStyle}
        >
            <Modal show={showModal} onHide={onHide}>
                <Modal.Header closeButton>
                    <Modal.Title>Помилка</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <p>Для доступу до цієї функції потрібна авторизація !</p>
                    <TelegramLoginForSite closeFunc={onHideFromTelegram}/>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={onHide}>
                        Закрити
                    </Button>
                </Modal.Footer>
            </Modal>
            {/*<ModalWindow show={showModal} title="Помилка" bodyText="Для доступу до цієї функції потрібна авторизація !" body={<TelegramLoginForSite closeFunc={onHideFromTelegram}/>}/>*/}
            <div className="container-fluid">
                <a className="navbar-brand" href="#">
                    <i className="fa fa-table"/>
                    &nbsp;Rozklad
                </a>
                <button
                    data-toggle="collapse"
                    className="navbar-toggler"
                    data-target="#navcol-1"
                >
                    <span className="sr-only">Toggle navigation</span>
                    <span className="navbar-toggler-icon"/>
                </button>
                <div className="collapse navbar-collapse" id="navcol-1">
                    <ul className="nav navbar-nav ml-auto">
                        
                        <li className="nav-item" role="button">
                            <Link
                                to={`timetable`}
                                className="nav-link"
                                style={navItemStyle}
                            >
                                <i className="fa fa-calendar"/>
                                &nbsp;Розклад
                            </Link>
                        </li>
                        <li className="nav-item" role="presentation">
                            {/*if isAuthentificated - button; if not - a link with modal*/}
                            {isAuthentificated ?
                                <Link to={`subjects`} className="nav-link" style={navItemStyle} href="#">
                                    <i className="fa fa-list-alt"/>
                                    &nbsp;Дисципліни
                                </Link> :
                                <a className="nav-link" style={navItemStyle} onClick={loginModal}>
                                    <i className="fa fa-list-alt"/>
                                    &nbsp;Дисципліни
                                </a>}

                        </li>
                        <li className="nav-item" role="button">
                            <Link
                                to={``}
                                className="nav-link"
                                style={navItemStyle}
                            >
                                <i className="fa fa-bell"/>
                                &nbsp;Нагадування
                            </Link>
                        </li>
                        <li>
                            <a onClick={logout} className=" btn mb-2"><i className="fas fa-sign-out-alt"/></a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    )

        ;
}

export default Navbar;
