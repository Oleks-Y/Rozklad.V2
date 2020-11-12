import React, {useEffect, useState} from "react";
import TelegramLoginForSite from "../Layouts/Login/TelegramLoginForSite";
import {Button, Modal} from "react-bootstrap"; 
interface ModalProps{
    show: boolean
    title : string
    bodyText : string
    body : React.ReactNode 
}

function ModalWindow(props : ModalProps){
    const [showModal, setShowModal] = useState<boolean>(true)
    const loginModal = ()=>{
        setShowModal(true)
    }
    const onHide = ()=>{
        setShowModal(false)
    }
    return (
        <Modal show={showModal} onHide={onHide}>
            <Modal.Header closeButton>
                <Modal.Title>{props.title}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <p>{props.bodyText}</p>
                {props.body}
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={onHide}>
                    Закрити
                </Button>
            </Modal.Footer>
        </Modal>
    )
}


export default ModalWindow